/*
 * PlayerRegistry.cs
 *
 * Lista estatica de los jugadores de la escena.
 *
 * Autor: Pablo G. Ruiz Orozco (Scuuz)
 * Fecha: 07/10/2026
 * Proyecto: TanquesPrototipo - Programación 2
 */


using System.Collections.Generic;
using UnityEngine;

public static class PlayerRegistry
{
    private static readonly List<PlayerRespawn> _players = new List<PlayerRespawn>();
    public static IReadOnlyList<PlayerRespawn> Players => _players;

    public static void Register(PlayerRespawn player)
    {
        if (!_players.Contains(player)) _players.Add(player);
    }

    public static void Unregister(PlayerRespawn player)
    {
        _players.Remove(player);
    }

    // Devuelve el jugador vivo más cercano a una posición, o null si no hay ninguno
    public static Transform GetClosestAlive(Vector3 position)
    {
        Transform closest = null;
        float closestDistance = float.MaxValue;

        foreach (PlayerRespawn player in _players)
        {
            if (player == null || !player.IsAlive) continue;

            float distance = (player.transform.position - position).sqrMagnitude;
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = player.transform;
            }
        }
        return closest;
    }
    public static bool AnyAliveWithin(Vector3 position, float distance)
    {
        foreach(PlayerRespawn player in _players)
        {
            if (player == null || !player.IsAlive) continue;
            if (Vector3.Distance(player.transform.position, position) < distance) return true;
        }
        return false;
    }
}
