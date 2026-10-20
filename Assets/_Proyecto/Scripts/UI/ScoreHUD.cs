/*
 * ScoreHUD.cs
 *
 * Muestra el puntaje y el tiempo de la partida. Y el resumen en pantalla de victoria/derrota
 *
 * Autor: Pablo G. Ruiz Orozco (Scuuz)
 * Fecha: 17/10/2026
 * Proyecto: TanquesPrototipo - Programación 2
 */



using TMPro;
using UnityEngine;

public class ScoreHUD : MonoBehaviour
{
    [Header("Durante la partida")]
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private TMP_Text _timeText;

    [Header("Resultados")]
    [SerializeField] private TMP_Text _victoryStatsText;
    [SerializeField] private TMP_Text _gameOverStatsText;

    private ScoreManager _scoreManager;

    private void Start()
    {
        _scoreManager = ScoreManager.Instance;
        _scoreManager.OnScoreChanged += UpdateScore;
        _scoreManager.OnFinished += ShowResults;
        UpdateScore(0);
    }

    private void OnDestroy()
    {
        if (_scoreManager != null)
        {
            _scoreManager.OnScoreChanged -= UpdateScore;
            _scoreManager.OnFinished -= ShowResults;
        }
    }

    private void Update()
    {
        _timeText.text = FormatTime(_scoreManager.ElapsedTime);
    }

    private void UpdateScore(int score)
    {
        _scoreText.text = $"Puntos: {score}";
    }

    private void ShowResults(bool victory)
    {
        string time = FormatTime(_scoreManager.ElapsedTime);

        if (victory)
        {
            string text = $"Tiempo: {time}\nPuntaje: {_scoreManager.FinalScore}\nRécord: {_scoreManager.HighScore}";
            if (_scoreManager.IsNewRecord) text += "\n¡NUEVO RÉCORD!";
            _victoryStatsText.text = text;
        }
        else
        {
            _gameOverStatsText.text = $"Tiempo: {time}\nPuntaje: {_scoreManager.FinalScore}";
        }
    }

    public static string FormatTime(float seconds)
    {
        int total = Mathf.FloorToInt(seconds);
        return $"{total / 60}:{total % 60:00}";
    }
}
