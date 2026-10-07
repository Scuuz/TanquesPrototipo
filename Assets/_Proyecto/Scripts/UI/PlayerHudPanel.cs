/*
 * PlayerHudPanel.cs
 *
 * Panel del HUD para el player adicional
 *
 * Autor: Pablo G. Ruiz Orozco (Scuuz)
 * Fecha: 07/10/2026
 * Proyecto: TanquesPrototipo - Programación 2
 */


using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHudPanel : MonoBehaviour
{
    [SerializeField] private PlayerRespawn _playerRespawn;
    [SerializeField] private Slider _healthBar;
    [SerializeField] private TMP_Text _respawnsText;
    [SerializeField] private string _label = "J2";

    private GameManager _gameManager;
    private Health _playerHealth;
    private bool _isEliminated;

    private void Start()
    {
        // En solitario el jugador 2 está apagado: se oculta todo el panel
        if (_playerRespawn == null || !_playerRespawn.gameObject.activeInHierarchy)
        {
            gameObject.SetActive(false);
            return;
        }

        _gameManager = GameManager.Instance;
        _playerHealth = _playerRespawn.GetComponent<Health>();

        _playerHealth.OnHealthChanged += UpdateHealth;
        _playerRespawn.OnPlayerEliminated += ShowEliminated;
        _gameManager.OnRespawnsChanged += UpdateRespawns;

        UpdateHealth(_playerHealth.CurrentHealth, _playerHealth.MaxHealth);
        UpdateRespawns();
    }

    private void OnDestroy()
    {
        if (_playerHealth != null) _playerHealth.OnHealthChanged -= UpdateHealth;
        if (_playerRespawn != null) _playerRespawn.OnPlayerEliminated -= ShowEliminated;
        if (_gameManager != null) _gameManager.OnRespawnsChanged -= UpdateRespawns;
    }

    private void UpdateHealth(float current, float max)
    {
        _healthBar.value = current / max;
    }

    private void UpdateRespawns()
    {
        if (_isEliminated) return;
        _respawnsText.text = $"{_label} Respawns: {_gameManager.GetRespawnsLeft(_playerRespawn.PlayerIndex)}";
    }

    private void ShowEliminated()
    {
        _isEliminated = true;
        _respawnsText.text = $"{_label} eliminado";
    }
}