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
    [SerializeField] private GameObject _respawnMessage;

    [Header("Victoria")]
    [SerializeField] private GameObject _victoryPanel;

    private void Start()
    {
        _playerHealth.OnHealthChanged += UpdateHealth;
        _playerRespawn.OnPlayerDied += ShowRespawnMessage;
        _playerRespawn.OnPlayerRespawned += HideRespawnMessage;

        GameManager gameManager = GameManager.Instance;
        gameManager.OnTowersChanged += UpdateTowers;
        gameManager.OnGoalOpened += ShowGoalOpen;
        gameManager.OnLevelCompleted += ShowVictory;

        UpdateHealth(_playerHealth.CurrentHealth, _playerHealth.MaxHealth);
        UpdateTowers(gameManager.RemainingTowers, gameManager.TotalTowers);
        _objectiveText.text = "Destruí las 3 torres de radar";
        _respawnMessage.SetActive(false);
        _victoryPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        if (_playerHealth != null) _playerHealth.OnHealthChanged -= UpdateHealth;
        if (_playerRespawn != null)
        {
            _playerRespawn.OnPlayerDied -= ShowRespawnMessage;
            _playerRespawn.OnPlayerRespawned -= HideRespawnMessage;
        }
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnTowersChanged -= UpdateTowers;
            GameManager.Instance.OnGoalOpened -= ShowGoalOpen;
            GameManager.Instance.OnLevelCompleted -= ShowVictory;
        }
    }
    private void UpdateHealth(float current, float max)
    {
        _healthBar.value = current / max;
    }
    private void UpdateTowers(int remaining, int total)
    {
        _towersText.text = $"Torres destruidas: {total - remaining}/{total}";
    }
    private void ShowGoalOpen()
    {
        _objectiveText.text = "Escudo desactivado. Llega al helipuerto";
    }
    private void ShowRespawnMessage()
    {
        _respawnMessage.SetActive(true);
    }
    private void HideRespawnMessage()
    {
        _respawnMessage.SetActive(false);
    }
    private void ShowVictory()
    {
        _victoryPanel.SetActive(true);
    }
}
