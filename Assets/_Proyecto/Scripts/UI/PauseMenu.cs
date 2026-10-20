/*
 * PauseMenu.cs
 *
 * Menu de pausa con "ESC", P o Start (Joystick)
 *
 * Autor: Pablo G. Ruiz Orozco (Scuuz)
 * Fecha: 17/10/2026
 * Proyecto: TanquesPrototipo - Programación 2
 */



using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject _pausePanel;

    public static bool IsPaused => _isPaused || Time.frameCount == _resumeFrame;

    private static bool _isPaused;
    private static int _resumeFrame = -1;

    private void Start()
    {
        _isPaused = false;
        _pausePanel.SetActive(false);
    }

    private void OnDestroy()
    {
        _isPaused = false;
    }

    private void Update()
    {
        if (!PausePressed()) return;

        if (_isPaused) Resume();
        else Pause();
    }

    private bool PausePressed()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P)) return true;

        Gamepad pad = Gamepad.current;
        return pad != null && pad.startButton.wasPressedThisFrame;
    }

    private void Pause()
    {
        GameManager gameManager = GameManager.Instance;
        if (gameManager != null && (gameManager.IsLevelCompleted || gameManager.IsGameOver)) return;

        _isPaused = true;
        Time.timeScale = 0f;
        _pausePanel.SetActive(true);
    }

    public void Resume()
    {
        _isPaused = false;
        _resumeFrame = Time.frameCount;
        Time.timeScale = 1f;
        _pausePanel.SetActive(false);
    }
}