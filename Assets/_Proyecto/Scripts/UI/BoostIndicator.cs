/*
 * BoostIndicator.cs
 *
 * HUD que muesttra la recarga del boost de un jugador
 *
 * Autor: Pablo G. Ruiz Orozco (Scuuz)
 * Fecha: 07/10/2026
 * Proyecto: TanquesPrototipo - Programación 2
 */


using UnityEngine;
using UnityEngine.UI;

public class BoostIndicator : MonoBehaviour
{
    [SerializeField] private TankMovement _movement;
    [SerializeField] private Slider _slider;

    private void Update()
    {
        if (_movement == null) return;
        _slider.value = _movement.BoostReady01;
    }
}
