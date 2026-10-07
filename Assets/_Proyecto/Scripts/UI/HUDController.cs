/*
 * HUDController.cs
 *
 * Muestra la vida del player, las torres destruidas, el objetivo actual, mensaje de respawn y la pantalla de victoria. Escuchando los eventos
 *
 * Autor: Pablo G. Ruiz Orozco (Scuuz)
 * Fecha: 30/09/2026
 * Proyecto: TanquesPrototipo - Programación 2
 */


using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Health _playerHealth;
    [SerializeField] private PlayerRespawn _playerRespawn;

    [Header("HUD")]
    [SerializeField] private Slider _healthBar;
    [SerializeField] private TMP_Text _towersText;
    [SerializeField] private TMP_Text _objectiveText;
    [SerializeField] private TMP_Text _respawnsText;
    [SerializeField] private GameObject _respawnMessage;

    [Header("Paneles")]
    [SerializeField] private GameObject _victoryPanel;
    [SerializeField] private GameObject _gameOverPanel;

    private GameManager _gameManager;

    private void Start()
    {
        _gameManager = GameManager.Instance;

        _playerHealth.OnHealthChanged += UpdateHealth;
        _playerRespawn.OnPlayerDied += ShowRespawnMessage;
        _playerRespawn.OnPlayerRespawned += HideRespawnMessage;

        _gameManager.OnTowersChanged += UpdateTowers;
        _gameManager.OnGoalOpened += ShowGoalOpen;
        _gameManager.OnLevelCompleted += ShowVictory;
        _gameManager.OnRespawnsChanged += UpdateRespawns;
        _gameManager.OnGameOver += ShowGameOver;

        // Estado inicial
        UpdateHealth(_playerHealth.CurrentHealth, _playerHealth.MaxHealth);
        UpdateTowers(_gameManager.RemainingTowers, _gameManager.TotalTowers);
        UpdateRespawns();
        _respawnMessage.SetActive(false);
        _victoryPanel.SetActive(false);
        _gameOverPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        if (_playerHealth != null) _playerHealth.OnHealthChanged -= UpdateHealth;
        if (_playerRespawn != null)
        {
            _playerRespawn.OnPlayerDied -= ShowRespawnMessage;
            _playerRespawn.OnPlayerRespawned -= HideRespawnMessage;
        }
        if (_gameManager != null)
        {
            _gameManager.OnTowersChanged -= UpdateTowers;
            _gameManager.OnGoalOpened -= ShowGoalOpen;
            _gameManager.OnLevelCompleted -= ShowVictory;
            _gameManager.OnRespawnsChanged -= UpdateRespawns;
            _gameManager.OnGameOver -= ShowGameOver;
        }
    }

    private void UpdateHealth(float current, float max)
    {
        _healthBar.value = current / max;
    }

    private void UpdateTowers(int remaining, int total)
    {
        _towersText.text = $"Torres destruidas: {total - remaining}/{total}";
        if (remaining > 0) _objectiveText.text = $"Destruí las {total} torres de radar";
    }

    private void UpdateRespawns()
    {
        _respawnsText.text = $"Respawns: {_gameManager.GetRespawnsLeft(_playerRespawn.PlayerIndex)}";
    }

    private void ShowGoalOpen()
    {
        _objectiveText.text = "¡Escudo desactivado! Llegá al helipuerto";
    }

    private void ShowRespawnMessage() { _respawnMessage.SetActive(true); }
    private void HideRespawnMessage() { _respawnMessage.SetActive(false); }
    private void ShowVictory() { _victoryPanel.SetActive(true); }

    private void ShowGameOver()
    {
        _respawnMessage.SetActive(false);
        _gameOverPanel.SetActive(true);
    }
}
