/*
 * LevelGenerator.cs
 *
 * Se genera el nivel en cada partida(generación procedural) segun la dificultad y la cantidad de jugadores. Tamaño de mapa, obstaculos, spawn, meta, torres y enemigos. Valida si existen caminos en el NavMesh.
 *
 * Autor: Pablo G. Ruiz Orozco (Scuuz)
 * Fecha: 26/09/2026
 * Proyecto: TanquesPrototipo - Programación 2
 */



using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

[DefaultExecutionOrder(-100)]
[RequireComponent(typeof(NavMeshSurface))]

public class LevelGenerator : MonoBehaviour
{
    [Header("Difficulty")]
    [SerializeField] private DifficultyConfig _defaultDifficulty;

    [Header("Seed")]
    [SerializeField] private bool _useRandomSeed = true;
    [SerializeField] private int _seed = 12345;

    [Header("Map Size (ancho X, largo Z)")]
    [SerializeField] private Vector2 _minSize = new Vector2(60f, 70f);
    [SerializeField] private Vector2 _maxSize = new Vector2(90f, 110f);

    [Header("Terrain")]
    [SerializeField] private Material _groundMaterial;
    [SerializeField] private float _wallHeight = 5f;
    [SerializeField] private GameObject _cliffPrefab;

    [SerializeField] private float _cliffHalfSize = 51f;
    
    [SerializeField] private float _cliffLength = 100f;

    [Header("Obstacles")]
    [SerializeField] private GameObject[] _obstaclePrefabs;
    [SerializeField] private int _obstacleCount = 15;
    [SerializeField] private float _obstacleSpacing = 10f;
    [SerializeField] private int _maxAttempts = 10;

    [Header("Actors (reubicacion objetos en la escena)")]
    [SerializeField] private Transform _player;
    [SerializeField] private Transform _player2;
    [SerializeField] private float _coopSpawnOffset = 3f;
    [SerializeField] private bool _testCoopInEditor = false;
    [SerializeField] private Transform _goalArea;
    [SerializeField] private Transform[] _towers;
    [SerializeField] private NavMeshAgent[] _enemies;

    [Header("Safe Zones")]
    [SerializeField] private float _spawnSafeRadius = 12f;
    [SerializeField] private float _goalSafeRadius = 12f;
    [SerializeField] private float _minTowerDistance = 20f;
    [SerializeField] private float _minEnemyDistanceToSpawn = 30f;

    public Vector2 MapSize {get; private set;}
    public Vector3 SpawnPoint {get; private set;}
    public Vector3 GoalPoint {get; private set;}

    private NavMeshSurface _surface;
    private Transform _terrainRoot;
    private Transform _obstacleRoot;
    private readonly List<(Vector3 position, float radius)> _reserved = new List<(Vector3, float)>();
    private int _keyReservedCount;
    private float _sizeMultiplier = 1f;

    private void Awake()
    {
        _surface = GetComponent<NavMeshSurface>();

        if (_useRandomSeed) _seed = System.Environment.TickCount;
        Random.InitState(_seed);
        Debug.Log($"[LevelGenerator] Seed: {_seed}");

        #if UNITY_EDITOR
        if (_testCoopInEditor) GameSession.PlayerCount = 2;
        #endif
        
        if (_player2 != null) _player2.gameObject.SetActive(GameSession.PlayerCount > 1);

        ApplyDifficulty();
        Generate();
    }

    private void ApplyDifficulty()
    {
        DifficultyConfig difficulty = GameSession.GetDifficulty(_defaultDifficulty);
        if (difficulty == null) return;

        _sizeMultiplier = difficulty.MapSizeMultiplier;

        for (int i = 0; i < _towers.Length; i++)
        {
            if (_towers[i] != null) _towers[i].gameObject.SetActive(i < difficulty.TowerCount);
        }
        for (int i = 0; i < _enemies.Length; i++)
        {
            if (_enemies[i] != null) _enemies[i].gameObject.SetActive(i < difficulty.EnemyCount);
        }
    }

    private void Generate()
    {
        float width = Random.Range(_minSize.x, _maxSize.x) * _sizeMultiplier;
        float length = Random.Range(_minSize.y, _maxSize.y) * _sizeMultiplier;
        MapSize = new Vector2(width, length);
        SpawnPoint = new Vector3(0f, 0f, -length / 2f + 8f);
        GoalPoint = new Vector3(0f, 0f, length / 2f -8f);

        BuildTerrain(width, length);
        PlaceKeyPoints(width, length);

        bool valid = false;
        for (int attempt = 1; attempt <= _maxAttempts && !valid; attempt++)
        {
            PlaceObstacles(width, length);
            Physics.SyncTransforms();
            _surface.BuildNavMesh();
            valid = IsLevelValid();

            if (!valid)
            {
                Debug.LogWarning($"[LevelGenerator] Intento {attempt}: mapa sin camino válido, regenerando obstáculos...");
            }
        }

        PlaceEnemies (width, length);
    }
    
    private void BuildTerrain (float width, float length)
    {
        _terrainRoot = new GameObject("Terrain").transform;
        _terrainRoot.SetParent(transform, false);

        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ground.name = "Ground";
        ground.transform.SetParent(_terrainRoot, false);
        ground.transform.localPosition = new Vector3(0f, -0.5f, 0f);
        ground.transform.localScale = new Vector3(width + 40f, 1f, length + 40f);

        if (_groundMaterial != null)
        {
            Renderer groundRenderer = ground.GetComponent<Renderer>();
            groundRenderer.material = _groundMaterial;
            groundRenderer.material.mainTextureScale = new Vector2((width + 40f) / 10f, (length + 40f) / 10f);
        }

        float h = _wallHeight;
        CreateWall("Wall_North", new Vector3(0f, h / 2f, length / 2f + 1f), new Vector3(width + 4f, h, 2f));
        CreateWall("Wall_South", new Vector3(0f, h / 2f, -length / 2f - 1f), new Vector3(width + 4f, h, 2f));
        CreateWall("Wall_East", new Vector3(width / 2f + 1f, h / 2f, 0f), new Vector3(2f, h, length + 4f));
        CreateWall("Wall_West", new Vector3(-width / 2f - 1f, h / 2f, 0f), new Vector3(2f, h, length + 4f));

        PlaceCliffs (width, length);
    }

    private void CreateWall(string wallName, Vector3 localPosition, Vector3 size)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = wallName;
        wall.transform.SetParent(_terrainRoot, false);
        wall.transform.localPosition = localPosition;
        wall.transform.localScale = size;
        wall.GetComponent<MeshRenderer>().enabled = false;
    }

    private void PlaceCliffs (float width, float length)
    {
        if (_cliffPrefab == null) return;

        float halfW = width / 2f + 4f;
        float halfL = length / 2f + 4f;

        SpawnCliff(0f, width + 8f, halfL);
        SpawnCliff(180f, width + 8f, halfL);
        SpawnCliff(90f, length + 8f, halfW);
        SpawnCliff(270f, length + 8f, halfW);
    }

    private void SpawnCliff (float angle, float sideLength, float distanceFromCenter)
    {
        GameObject cliff = Instantiate (_cliffPrefab, Vector3.zero, Quaternion.Euler(0f, angle, 0f), _terrainRoot);
        cliff.transform.localScale = new Vector3(sideLength / _cliffLength, 1f, distanceFromCenter / _cliffHalfSize);
    }
    
    private void PlaceKeyPoints(float width, float length)
    {
        _reserved.Clear();

        bool isCoop = _player2 != null && _player2.gameObject.activeSelf;
        Vector3 offset = isCoop ? Vector3.right * _coopSpawnOffset : Vector3.zero;

        if (_player != null) _player.SetPositionAndRotation(SpawnPoint - offset, Quaternion.identity);
        if (isCoop) _player2.SetPositionAndRotation(SpawnPoint + offset, Quaternion.identity);
        Reserve (SpawnPoint, _spawnSafeRadius);

        if (_goalArea != null) _goalArea.position = GoalPoint;
        Reserve(GoalPoint, _goalSafeRadius);

        float towerRadius = _minTowerDistance / 2f;
        foreach (Transform tower in _towers)
        {
            if (tower == null || !tower.gameObject.activeSelf) continue;
            if (TryFindFreePosition(width, length, towerRadius, -0.25f, 0.35f, out Vector3 position))
            {
                tower.position = position;
                Reserve(position, towerRadius);
            }
            else
            {
                Debug.LogWarning($"[LevelGenerator] No encontró lugar para {tower.name}.");
            }
        }

        _keyReservedCount = _reserved.Count;
    }

    private void PlaceObstacles(float width, float length)
    {
        ClearObstacles();
        if (_obstaclePrefabs == null || _obstaclePrefabs.Length == 0) return;

        float radius = _obstacleSpacing / 2f;
        int count = Mathf.RoundToInt(_obstacleCount * _sizeMultiplier * _sizeMultiplier);
        for (int i = 0; i < count; i++)
        {
            if (!TryFindFreePosition(width, length, radius, -0.45f, 0.45f, out Vector3 position)) continue;

            GameObject prefab = _obstaclePrefabs[Random.Range(0, _obstaclePrefabs.Length)];
            Quaternion rotation = Quaternion.Euler(0f, Random.Range(0,4) * 90f, 0f);
            Instantiate(prefab, position, rotation, _obstacleRoot);
            Reserve(position, radius);
        }
    }

    private void ClearObstacles()
    {
        if (_obstacleRoot != null)
        {
            _obstacleRoot.gameObject.SetActive(false);
            Destroy(_obstacleRoot.gameObject);
        }

        _obstacleRoot = new GameObject("Obstacles").transform;
        _obstacleRoot.SetParent(transform, false);

        _reserved.RemoveRange(_keyReservedCount, _reserved.Count - _keyReservedCount);
    }

    private bool IsLevelValid()
    {
        Vector3 goalEntrance = GoalPoint + Vector3.back * (_goalSafeRadius * 0.75f);

        if (!HasPath(SpawnPoint, goalEntrance)) 
        {
            Debug.LogWarning("[LevelGenerator] Sin camino hasta la entrada de la meta");
            return false;
        }

        foreach (Transform tower in _towers)
        {
            if (tower != null && tower.gameObject.activeSelf && !HasPath(SpawnPoint, tower.position)) 
            {
                Debug.LogWarning($"[LevelGenerator] Sin camino hasta {tower.name}");
                return false;
            }
        }
        return true;
    }

    private bool HasPath(Vector3 from, Vector3 to)
    {
        if (!NavMesh.SamplePosition(from, out NavMeshHit fromHit, 4f, NavMesh.AllAreas)) return false;
        if (!NavMesh.SamplePosition(to, out NavMeshHit toHit, 6f, NavMesh.AllAreas)) return false;
        
        NavMeshPath path = new NavMeshPath();
        return NavMesh.CalculatePath(fromHit.position, toHit.position, NavMesh.AllAreas, path) && path.status == NavMeshPathStatus.PathComplete;
    }
    
    private void PlaceEnemies (float width, float length)
    {
        foreach (NavMeshAgent enemy in _enemies)
        {
            if (enemy == null || !enemy.gameObject.activeSelf) continue;
            enemy.enabled = true;
            for (int i = 0; i < 30; i++)
            {
                Vector3 candidate = new Vector3(Random.Range(-width / 2f + 5f, width / 2f - 5f), 0f, Random.Range(-length * 0.2f, length * 0.45f));
                
                if (Vector3.Distance(candidate, SpawnPoint) < _minEnemyDistanceToSpawn) continue;

                if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 4f, NavMesh.AllAreas))
                {
                    enemy.Warp(hit.position);
                    enemy.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
                    break;
                }
            }
        }
    }

    private bool TryFindFreePosition(float width, float length, float radius, float zMinFactor, float zMaxFactor, out Vector3 position)
    {
        const float margin = 4f;
        for (int i = 0; i < 50; i++)
        {
            float x = Random.Range(-width / 2f +margin, width / 2f - margin);
            float z = Random.Range(length * zMinFactor, length * zMaxFactor);
            Vector3 candidate = new Vector3(x, 0f, z);

            if (IsFree(candidate, radius))
            {
                position = candidate;
                return true;
            }
        }
        position = Vector3.zero;
        return false;
    }

    private bool IsFree(Vector3 candidate, float radius)
    {
        foreach ((Vector3 position, float reservedRadius) in _reserved)
        {
            if (Vector3.Distance(candidate, position) < radius + reservedRadius) return false;
        }
        return true;
    }

    private void Reserve(Vector3 position, float radius)
    {
        _reserved.Add((position, radius));
    }
}
