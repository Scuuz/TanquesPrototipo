/*
 * PowerUpSpawner.cs
 *
 * Se reparten power ups en posiciones validas en el mapa y se vuelven a reubicar un tiempo despues de que se agarran 
 *
 * Autor: Pablo G. Ruiz Orozco (Scuuz)
 * Fecha: 07/10/2026
 * Proyecto: TanquesPrototipo - Programación 2
 */


using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PowerUpSpawner : MonoBehaviour
{
    [SerializeField] private LevelGenerator _level;
    [SerializeField] private PowerUp[] _prefabs;
    [SerializeField] private int _count = 4;
    [SerializeField] private float _respawnTime = 15f;
    [SerializeField] private float _minDistanceToSpawn = 10f;
    [SerializeField] private float _minDistanceToGoal = 14f;

    private void Start()
    {
        if (_prefabs == null || _prefabs.Length == 0) return;

        for (int i = 0; i < _count; i++)
        {
            PowerUp powerUp = Instantiate(_prefabs[i % _prefabs.Length], transform);
            powerUp.gameObject.SetActive(false);
            powerUp.Collected += HandleCollected;
            StartCoroutine(PlaceRoutine(powerUp, 0f));
        }
    }

    private void HandleCollected(PowerUp powerUp)
    {
        StartCoroutine(PlaceRoutine(powerUp, _respawnTime));
    }

    private IEnumerator PlaceRoutine(PowerUp powerUp, float delay)
    {
        if (delay > 0f) yield return new WaitForSeconds(delay);

        Vector3 position;
        while (!TryFindPosition(out position))
        {
            yield return new WaitForSeconds(1f);
        }

        powerUp.Show(position);
    }

    private bool TryFindPosition(out Vector3 position)
    {
        position = Vector3.zero;
        Vector2 size = _level.MapSize;

        for (int i = 0; i < 20; i++)
        {
            float x = Random.Range(-size.x / 2f + 5f, size.x / 2f - 5f);
            float z = Random.Range(-size.y / 2f + 5f, size.y / 2f - 5f);
            Vector3 candidate = new Vector3(x, 0f, z);

            if (!NavMesh.SamplePosition(candidate, out NavMeshHit hit, 3f, NavMesh.AllAreas)) continue;
            if (Vector3.Distance(hit.position, _level.SpawnPoint) < _minDistanceToSpawn) continue;
            if (Vector3.Distance(hit.position, _level.GoalPoint) < _minDistanceToGoal) continue;

            position = hit.position;
            return true;
        }
        return false;
    }
}
