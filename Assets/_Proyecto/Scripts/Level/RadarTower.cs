/*
 * RadarTower.cs
 *
 * Detecta al jugdor dentro de un radio para alertar a los enemigos y pide ataque aereo sobre el que esté en rango.
 *
 * Autor: Pablo G. Ruiz Orozco (Scuuz)
 * Fecha: 29/09/2026
 * Proyecto: TanquesPrototipo - Programación 2
 */


using UnityEngine;

[RequireComponent(typeof(Health))]
public class RadarTower : MonoBehaviour
{
    [SerializeField] private float _radarRange = 25f;

    [Header("Ataque aéreo")]
    [SerializeField] private float _firstStrikeDelay = 1.5f;
    [SerializeField] private float _strikeInterval = 4f;
    [SerializeField] private float _strikeSpread = 1.5f;

    public Health Health { get; private set; }

    private float _strikeTimer;

    private void Awake()
    {
        Health = GetComponent<Health>();
        _strikeTimer = _firstStrikeDelay;
    }

    private void Update()
    {
        Transform target = FindTarget();

        if (target == null)
        {
            _strikeTimer = _firstStrikeDelay;
            return;
        }

        _strikeTimer -= Time.deltaTime;
        if (_strikeTimer > 0f) return;

        _strikeTimer = _strikeInterval;
        if (AirStrikePool.Instance == null) return;

        Vector2 spread = Random.insideUnitCircle * _strikeSpread;
        Vector3 position = target.position + new Vector3(spread.x, 0f, spread.y);
        AirStrikePool.Instance.Launch(position);
    }

    public bool IsDetecting(Vector3 targetPosition)
    {
        if (Health.IsDead || !isActiveAndEnabled) return false;

        Vector3 offset = targetPosition - transform.position;
        offset.y = 0f;
        return offset.sqrMagnitude <= _radarRange * _radarRange;
    }

    private Transform FindTarget()
    {
        Transform closest = null;
        float closestDistance = float.MaxValue;

        foreach (PlayerRespawn player in PlayerRegistry.Players)
        {
            if (player == null || !player.IsAlive) continue;
            if (!IsDetecting(player.transform.position)) continue;

            float distance = (player.transform.position - transform.position).sqrMagnitude;
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = player.transform;
            }
        }
        return closest;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, _radarRange);
    }
}