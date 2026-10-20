/*
 * ScoreManager.cs
 *
 * Un Singleton que suma los puntos, ademas mide el tiempo de la partida, calcula puntaje final y el record.
 *
 * Autor: Pablo G. Ruiz Orozco (Scuuz)
 * Fecha: 17/10/2026
 * Proyecto: TanquesPrototipo - Programación 2
 */



using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("Bonus al ganar")]
    [SerializeField] private int _victoryBonus = 1000;
    [SerializeField] private float _parTime = 300f;
    [SerializeField] private int _pointsPerSecondSaved = 5;

    public int Score { get; private set; }
    public float ElapsedTime { get; private set; }
    public int FinalScore { get; private set; }
    public int HighScore { get; private set; }
    public bool IsNewRecord { get; private set; }

    public event System.Action<int> OnScoreChanged;
    public event System.Action<bool> OnFinished;

    private GameManager _gameManager;
    private bool _finished;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _gameManager = GameManager.Instance;
        _gameManager.OnLevelCompleted += HandleVictory;
        _gameManager.OnGameOver += HandleDefeat;
        HighScore = PlayerPrefs.GetInt(RecordKey(), 0);
    }

    private void OnDestroy()
    {
        if (_gameManager != null)
        {
            _gameManager.OnLevelCompleted -= HandleVictory;
            _gameManager.OnGameOver -= HandleDefeat;
        }
        if (Instance == this) Instance = null;
    }

    private void Update()
    {
        if (!_finished) ElapsedTime += Time.deltaTime;
    }

    public static void Add(int points)
    {
        if (Instance == null || Instance._finished) return;

        Instance.Score += points;
        Instance.OnScoreChanged?.Invoke(Instance.Score);
    }

    private void HandleVictory()
    {
        _finished = true;

        int timeBonus = Mathf.Max(0, Mathf.RoundToInt((_parTime - ElapsedTime) * _pointsPerSecondSaved));
        FinalScore = Score + _victoryBonus + timeBonus;

        if (FinalScore > HighScore)
        {
            HighScore = FinalScore;
            IsNewRecord = true;
            PlayerPrefs.SetInt(RecordKey(), HighScore);
            PlayerPrefs.Save();
        }

        OnFinished?.Invoke(true);
    }

    private void HandleDefeat()
    {
        _finished = true;
        FinalScore = Score;
        OnFinished?.Invoke(false);
    }

    private string RecordKey()
    {
        string difficulty = _gameManager.Difficulty != null ? _gameManager.Difficulty.DisplayName : "Default";
        return $"HighScore_{difficulty}_{_gameManager.PlayerCount}P";
    }
}
