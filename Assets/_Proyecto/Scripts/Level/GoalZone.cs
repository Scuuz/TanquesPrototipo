/*
 * GoalZone.cs
 *
 * Descripción breve de qué hace este script.
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
