/*
 * PowerUp.cs
 *
 * Objeto que el jugador va a agarrar al tocarlo.
 *
 * Autor: Pablo G. Ruiz Orozco (Scuuz)
 * Fecha: 07/10/2026
 * Proyecto: TanquesPrototipo - Programación 2
 */


using UnityEngine;

public enum PowerUpType { Repair, Shield }

[RequireComponent(typeof(Collider))]
public class PowerUp : MonoBehaviour
{
    [SerializeField] private PowerUpType _type = PowerUpType.Repair;
    [SerializeField] private float _healAmount = 50f;
    [SerializeField] private float _shieldDuration = 6f;
    [SerializeField] private float _spinSpeed = 90f;

    public event System.Action<PowerUp> Collected;

    private void Update()
    {
        transform.Rotate(0f, _spinSpeed * Time.deltaTime, 0f);
    }

    private void OnTriggerStay(Collider other)
    {
        PlayerRespawn player = other.GetComponentInParent<PlayerRespawn>();
        if (player == null || !player.IsAlive) return;

        Health health = player.GetComponent<Health>();

        if (_type == PowerUpType.Repair)
        {
            if (health.CurrentHealth >= health.MaxHealth) return;
            health.Heal(_healAmount);
        }
        else
        {
            health.AddShield(_shieldDuration);
        }

        gameObject.SetActive(false);
        Collected?.Invoke(this);
    }

    public void Show(Vector3 position)
    {
        transform.position = position;
        gameObject.SetActive(true);
    }
}
