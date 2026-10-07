/*
 * GoalZone.cs
 *
 * Zona de meta. Avisa al GameManager que se completó el nivel.
 *
 * Autor: Pablo G. Ruiz Orozco (Scuuz)
 * Fecha: 29/09/2026
 * Proyecto: TanquesPrototipo - Programación 2
 */


using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GoalZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (GameManager.Instance == null || !GameManager.Instance.IsGoalOpen) return;
        
        GameManager.Instance.CompleteLevel();
    }
}
