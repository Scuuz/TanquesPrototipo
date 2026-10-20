/*
 * Health.cs
 *
 * Vida de un objeto. Cuando recibe daño avisa por eventos cuando la vida cambia o muere.
 *
 * Autor: Pablo G. Ruiz Orozco (Scuuz)
 * Fecha: 25/09/2026
 * Proyecto: TanquesPrototipo - Programación 2
 */


using System;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    [SerializeField] private float _maxHealth = 100f;

    public float CurrentHealth { get; private set; }
    public float MaxHealth => _maxHealth;
    public bool IsDead { get; private set; }
    public bool IsInvulnerable { get; set; }
    public bool IsShielded => Time.time < _shieldEndTime;
    public bool IsProtected => IsInvulnerable || IsShielded;

    public event Action<float, float> OnHealthChanged;
    public event Action OnDied;
    public event Action <float> OnDamaged;

    private float _shieldEndTime;

    private void Awake()
    {
        ResetHealth();
    }

    public void ResetHealth()
    {
        CurrentHealth = _maxHealth;
        IsDead = false;
        OnHealthChanged?.Invoke(CurrentHealth, _maxHealth);
    }

    public void TakeDamage(float amount)
    {
        if (IsDead || IsProtected) return;

        CurrentHealth = Mathf.Max(CurrentHealth - amount, 0f);
        OnHealthChanged?.Invoke(CurrentHealth, _maxHealth);
        OnDamaged?.Invoke(amount);

        if (CurrentHealth <= 0f)
        {
            IsDead = true;
            OnDied?.Invoke();
        }
    }

    public void Heal(float amount)
    {
        if (IsDead) return;

        CurrentHealth = Mathf.Min(CurrentHealth + amount, _maxHealth);
        OnHealthChanged?.Invoke(CurrentHealth, _maxHealth);
    }

    public void AddShield(float seconds)
    {
        _shieldEndTime = Mathf.Max(_shieldEndTime, Time.time + seconds);
    }
}
