/*
 * ShieldVisual.cs
 *
 * Una burbuja alrededor del tanque de proteccion
 *
 * Autor: Pablo G. Ruiz Orozco (Scuuz)
 * Fecha: 07/10/2026
 * Proyecto: TanquesPrototipo - Programación 2
 */


using UnityEngine;

[RequireComponent(typeof(Health))]
public class ShieldVisual : MonoBehaviour
{
    [SerializeField] private GameObject _bubble;

    private Health _health;

    private void Awake()
    {
        _health = GetComponent<Health>();
        _bubble.SetActive(false);
    }

    private void Update()
    {
        bool show = _health.IsProtected && !_health.IsDead;
        if (_bubble.activeSelf != show) _bubble.SetActive(show);
    }
}