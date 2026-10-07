/*
 * TeamMember.cs
 *
 * Indica a que equipo pertenece un objeto (proyectiles no dañe a su propio equipo)
 *
 * Autor: Pablo G. Ruiz Orozco (Scuuz)
 * Fecha: 06/10/2026
 * Proyecto: TanquesPrototipo - Programación 2
 */


using UnityEngine;

public enum Team {Player, Enemy}

public class TeamMember : MonoBehaviour
{
    [SerializeField] private Team _team = Team.Enemy;

    [SerializeField] private bool _alliedShootsPassThrough = true;

    public Team Team => _team;
    public bool AlliedShootsPassThrough => _alliedShootsPassThrough;
}
