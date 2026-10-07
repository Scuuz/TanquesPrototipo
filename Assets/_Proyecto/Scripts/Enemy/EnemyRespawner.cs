/*
 * EnemyRespawner.cs
 *
 * Respawn de los enemigos destruidos
 *
 * Autor: Pablo G. Ruiz Orozco (Scuuz)
 * Fecha: 06/10/2026
 * Proyecto: TanquesPrototipo - Programación 2
 */


using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyRespawner : MonoBehaviour
{
    [SerializeField] private EnemyAI[] _enemies;
    [SerializeField] private float _respawnDelay = 8f;
    [SerializeField] private float _spawnRadius = 10f;            // distancia máxima a la torre
    [SerializeField] private float _minDistanceToPlayer = 15f;    // para que no aparezcan encima del jugador

    private Transform _player;

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null) _player = playerObject.transform;
    }

    private void OnEnable()
    {
        foreach (EnemyAI enemy in _enemies)
        {
            if (enemy != null) enemy.Died += HandleEnemyDied;
        }
    }

    private void OnDisable()
    {
        foreach (EnemyAI enemy in _enemies)
        {
            if (enemy != null) enemy.Died -= HandleEnemyDied;
        }
    }

    private void HandleEnemyDied(EnemyAI enemy)
    {
        StartCoroutine(RespawnRoutine(enemy));
    }

    private IEnumerator RespawnRoutine(EnemyAI enemy)
    {
        yield return new WaitForSeconds(_respawnDelay);

        while (true)
        {
            GameManager gameManager = GameManager.Instance;

            // Sin torres (o partida terminada) ya no reaparece nadie
            if (gameManager == null || gameManager.IsGoalOpen || gameManager.IsLevelCompleted || gameManager.IsGameOver) yield break;

            if (TryFindSpawnPosition(gameManager, out Vector3 position))
            {
                enemy.Respawn(position);
                yield break;
            }

            // No encontró lugar (player demasiado cerca): reintenta en un segundo
            yield return new WaitForSeconds(1f);
        }
    }

    private bool TryFindSpawnPosition(GameManager gameManager, out Vector3 position)
    {
        position = Vector3.zero;
        
        RadarTower tower = gameManager.GetRandomActiveTower();
        if (tower == null) return false;

        for (int i = 0; i < 10; i++)
        {
            Vector2 circle = Random.insideUnitCircle.normalized * Random.Range(4f, _spawnRadius);
            Vector3 candidate = tower.transform.position + new Vector3(circle.x, 0f, circle.y);

            if (!NavMesh.SamplePosition(candidate, out NavMeshHit hit, 4f, NavMesh.AllAreas)) continue;
            if (_player != null && Vector3.Distance(hit.position, _player.position) < _minDistanceToPlayer) continue;

            position = hit.position;
            return true;
        }
        return false;
    }
}
