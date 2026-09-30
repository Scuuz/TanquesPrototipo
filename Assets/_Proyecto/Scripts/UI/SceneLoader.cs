/*
 * SceneLoader.cs
 *
 * Cambia las escenas con los botones de la UI y se resetea(Time.timeScale).
 *
 * Autor: Pablo G. Ruiz Orozco (Scuuz)
 * Fecha: 30/09/2026
 * Proyecto: TanquesPrototipo - Programación 2
 */


using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private string _levelSceneName = "Nivel01";
    [SerializeField] private string _menuSceneName = "MainMenu";

    public void LoadLevel()
    {
        Load(_levelSceneName);
    }

    public void LoadMenu()
    {
        Load(_menuSceneName);
    }

    public void RestartCurrent()
    {
        Load(SceneManager.GetActiveScene().name);
    }

    private void Load(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }
}
