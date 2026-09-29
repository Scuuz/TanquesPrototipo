/*
 * RadarTower.cs
 *
 * Detecta al jugdor dentro de un radio para alertar a los enemigos.
 *
 * Autor: Pablo G. Ruiz Orozco (Scuuz)
 * Fecha: 29/09/2026
 * Proyecto: TanquesPrototipo - Programación 2
 */


using UnityEngine;

[RequireComponent (typeof(Health))]

public class RadarTower : MonoBehaviour
{
    [SerializeField] private float _radarRange = 25f;

    public Health Health {get; private set;}

    private void Awake()
    {
        Health = GetComponent<Health>();
    }

    public bool IsDetecting(Vector3 targetPosition)
    {
        if (Health.IsDead || !isActiveAndEnabled) return false;

        Vector3 offset = targetPosition - transform.position;
        offset.y = 0f;
        return offset.sqrMagnitude <= _radarRange * _radarRange;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, _radarRange);
    }
}
