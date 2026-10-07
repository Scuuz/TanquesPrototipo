/*
 * MainMenuController.cs
 *
 * Controla los paneles del menú principal. Guarda la elección en GameSession y carga el nivel.
 *
 * Autor: Pablo G. Ruiz Orozco (Scuuz)
 * Fecha: 05/10/2026
 * Proyecto: TanquesPrototipo - Programación 2
 */


using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [Header("Dificultades")]
    [SerializeField] private DifficultyConfig _normalDifficulty;
    [SerializeField] private DifficultyConfig _hardDifficulty;

    [Header("Paneles")]
    [SerializeField] private GameObject _mainPanel;
    [SerializeField] private GameObject _modePanel;
    [SerializeField] private GameObject _difficultyPanel;

    [Header("Escenas")]
    [SerializeField] private SceneLoader _sceneLoader;

    private void Start()
    {
        ShowPanel(_mainPanel);
    }

    // Botones

    public void OnPlayPressed()
    {
        ShowPanel(_modePanel);
    }

    public void SelectSolo()
    {
        GameSession.PlayerCount = 1;
        ShowPanel(_difficultyPanel);
    }

    public void SelectCoop()
    {
        GameSession.PlayerCount = 2;
        ShowPanel(_difficultyPanel);
    }

    public void SelectNormal()
    {
        StartGame(_normalDifficulty);
    }

    public void SelectHard()
    {
        StartGame(_hardDifficulty);
    }

    public void BackToMain()
    {
        ShowPanel(_mainPanel);
    }

    public void BackToMode()
    {
        ShowPanel(_modePanel);
    }

    private void StartGame(DifficultyConfig difficulty)
    {
        GameSession.Difficulty = difficulty;
        _sceneLoader.LoadLevel();
    }

    private void ShowPanel(GameObject panelToShow)
    {
        _mainPanel.SetActive(panelToShow == _mainPanel);
        _modePanel.SetActive(panelToShow == _modePanel);
        _difficultyPanel.SetActive(panelToShow == _difficultyPanel);
    }
}
