/*
 * GameManager.cs
 *
 * Se utiliza Singleton. Se cuenta las torres destruidas, alerta a los enemigos y abre la meta al caer todas las torres.
 *
 * Autor: Pablo G. Ruiz Orozco (Scuuz)
 * Fecha: 29/09/2026
 * Proyecto: TanquesPrototipo - Programación 2
 */


using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {get; private set;}

    [Header("Desafio/Win")]
    [SerializeField] private RadarTower[] _towers;
    [SerializeField] private EnemyAI[] _enemies;
    [SerializeField] private GameObject _goalBarrier;
    [SerializeField] private EffectPool _barrierEffectPool;

    public int TotalTowers => _towers.Length;
    public int RemainingTowers {get; private set;}
    public bool IsGoalOpen => RemainingTowers <= 0;

    public event Action<int, int> OnTowersChanged;
    public event Action OnGoalOpened;

    private Transform _player;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null) _player = playerObject.transform;

        RemainingTowers = 0;
        foreach (RadarTower tower in _towers)
        {
            if (tower == null) continue;
            tower.Health.OnDied += HandleTowerDestroyed;
            RemainingTowers++;
        }
        OnTowersChanged?.Invoke(RemainingTowers, TotalTowers);
    }

    private void OnDestroy()
    {
        foreach (RadarTower tower in _towers)
        {
            if (tower != null) tower.Health.OnDied -= HandleTowerDestroyed;
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

    private bool IsPlayerDetectedByRadar()
    {
        if (_player == null) return false;

        foreach (RadarTower tower in _towers)
        {
            if (tower != null && tower.IsDetecting(_player.position)) return true;
        }
        return false;
    }

    private void HandleTowerDestroyed()
    {
        RemainingTowers --;
        OnTowersChanged?.Invoke(RemainingTowers, TotalTowers);
        Debug.Log($"[GameManager] Torre destruida. Quedan {RemainingTowers}/{TotalTowers}.");

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
        Debug.Log("[GameManager] Todas las torres destruidas. La meta está abierta.");
    }
}
