/*
 * GameManager.cs
 *
 * Se utiliza Singleton. Aplica la dificultad, cuenta las torres y respawns, alerta a los enemigos, se abre la meta y defina victoria o derrota
 *
 * Autor: Pablo G. Ruiz Orozco (Scuuz)
 * Fecha: 29/09/2026
 * Proyecto: TanquesPrototipo - Programación 2
 */


using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {get; private set;}

    [Header("Dificultad")]
    [SerializeField] private DifficultyConfig _defaultDifficulty;

    [Header("Desafio/Win")]
    [SerializeField] private RadarTower[] _towers;
    [SerializeField] private EnemyAI[] _enemies;
    [SerializeField] private GameObject _goalBarrier;
    [SerializeField] private EffectPool _barrierEffectPool;

    public DifficultyConfig Difficulty{get; private set;}
    public int PlayerCount {get; private set;}
    public int TotalTowers {get; private set;}
    public int RemainingTowers {get; private set;}
    public bool IsGoalOpen => RemainingTowers <= 0;
    public bool IsLevelCompleted {get; private set;}
    public bool IsGameOver {get; private set;}

    public event Action<int, int> OnTowersChanged;
    public event Action OnGoalOpened;
    public event Action OnLevelCompleted;
    public event Action OnRespawnsChanged;
    public event Action OnGameOver;

    private readonly List<RadarTower> _activeTowers = new List<RadarTower>();

    private int[] _respawnsLeft;
    private int _sharedRespawnsLeft;
    private bool _respawnsAreShared;
    private int _eliminatedPlayers;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        Difficulty = GameSession.GetDifficulty(_defaultDifficulty);
        PlayerCount = Mathf.Clamp(GameSession.PlayerCount, 1, 2);
        SetupRespawns();
    }

    private void SetupRespawns()
    {
        bool isCoop = PlayerCount > 1;
        int amount = 3;

        if (Difficulty != null)
        {
            amount = isCoop ? Difficulty.CoopRespawns : Difficulty.SoloRespawns;
            _respawnsAreShared = isCoop && Difficulty.CoopRespawnsShared;
        }
        else
        {
            Debug.LogError("[GameManager] Falta asignar Default Difficulty.");
        }

        _sharedRespawnsLeft = amount;
        _respawnsLeft = new int[PlayerCount];
        for (int i = 0; i < PlayerCount; i++) _respawnsLeft[i] = amount;
    }

    private void Start()
    {
        RemainingTowers = 0;
        foreach (RadarTower tower in _towers)
        {
            if (tower == null || !tower.gameObject.activeInHierarchy) continue;
            _activeTowers.Add(tower);
            tower.Health.OnDied += HandleTowerDestroyed;
        }

        TotalTowers = _activeTowers.Count;
        RemainingTowers = TotalTowers;
        OnTowersChanged?.Invoke(RemainingTowers, TotalTowers);
    }

    private void OnDestroy()
    {
        foreach (RadarTower tower in _towers)
        {
            if (tower != null && tower.Health != null) tower.Health.OnDied -= HandleTowerDestroyed;
        }
        if (Instance == this) Instance = null;
    }

    private void Update()
    {
        bool detected = IsPlayerDetectedByRadar();
        foreach (EnemyAI enemy in _enemies)
        {
            if (enemy != null) enemy.IsAlerted = detected;
        }
    }

    // TORRES
    private bool IsPlayerDetectedByRadar()
    {
        foreach (PlayerRespawn player in PlayerRegistry.Players)
        {
            if (player == null || !player.IsAlive) continue;

            foreach (RadarTower tower in _activeTowers)
            {
                if (tower != null && tower.IsDetecting(player.transform.position)) return true;
            }
        }
        return false;
    }

    public RadarTower GetRandomActiveTower()
    {
        List<RadarTower> alive = new List<RadarTower>();
        foreach (RadarTower tower in _activeTowers)
        {
            if (tower != null && !tower.Health.IsDead) alive.Add(tower);
        }

        if (alive.Count == 0) return null;
        return alive[UnityEngine.Random.Range(0, alive.Count)];
    }

    private void HandleTowerDestroyed()
    {
        RemainingTowers --;
        OnTowersChanged?.Invoke(RemainingTowers, TotalTowers);
        

        if (RemainingTowers <= 0) OpenGoal();
    }

    private void OpenGoal()
    {
        if (_goalBarrier != null)
        {
            if (_barrierEffectPool != null) _barrierEffectPool.Play(_goalBarrier.transform.position);
            _goalBarrier.SetActive(false);
        }
        OnGoalOpened?.Invoke();
    }

    // vidas

    public int GetRespawnsLeft(int playerIndex)
    {
        if (_respawnsAreShared) return _sharedRespawnsLeft;
        return _respawnsLeft[Mathf.Clamp(playerIndex, 0, _respawnsLeft.Length - 1)];
    }

    public bool TryUseRespawn(int playerIndex)
    {
        if (IsGameOver || IsLevelCompleted) return false;
        if (GetRespawnsLeft(playerIndex) <= 0) return false;

        if (_respawnsAreShared) _sharedRespawnsLeft--;
        else _respawnsLeft[Mathf.Clamp(playerIndex, 0, _respawnsLeft.Length - 1)]--;

        OnRespawnsChanged?.Invoke();
        return true;
    }

    public void NotifyPlayerEliminated()
    {
        _eliminatedPlayers++;
        if (_eliminatedPlayers >= PlayerCount) GameOver();
    }

    // Win/Lose

    public void CompleteLevel()
    {
        if (IsLevelCompleted || IsGameOver) return;

        IsLevelCompleted = true;
        OnLevelCompleted?.Invoke();
        Time.timeScale = 0f;
    }

    private void GameOver()
    {
        if (IsGameOver || IsLevelCompleted) return;

        IsGameOver = true;
        OnGameOver?.Invoke();
        Time.timeScale = 0f;
    }
}
