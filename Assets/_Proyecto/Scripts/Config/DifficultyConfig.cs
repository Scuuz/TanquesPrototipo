/*
 * DifficultyConfig.cs
 *
 * Scriptable Object con los valores de una dificultad
 *
 * Autor: Pablo G. Ruiz Orozco (Scuuz)
 * Fecha: 06/10/2026
 * Proyecto: TanquesPrototipo - Programación 2
 */


using UnityEngine;

[CreateAssetMenu(fileName = "Difficulty", menuName = "Tanques/Difficulty Config")]
public class DifficultyConfig : ScriptableObject
{
    [SerializeField] private string _displayName = "Normal";
    [SerializeField] private int _towerCount = 3;
    [SerializeField] private int _enemyCount = 3;
    [SerializeField] private int _soloRespawns = 3;
    [SerializeField] private int _coopRespawns = 3;
    [SerializeField] private bool _coopRespawnsShared = false;
    [SerializeField] private float _mapSizeMultiplier = 1f;

    public string DisplayName => _displayName;
    public int TowerCount => _towerCount;
    public int EnemyCount => _enemyCount;
    public int SoloRespawns => _soloRespawns;
    public int CoopRespawns => _coopRespawns;
    public bool CoopRespawnsShared => _coopRespawnsShared;
    public float MapSizeMultiplier => _mapSizeMultiplier;
}
