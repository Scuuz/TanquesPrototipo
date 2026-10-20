/*
 * DamageFlash.cs
 *
 * Parpadea la pantalla con rojo y tiembla cuando recibe daño
 *
 * Autor: Pablo G. Ruiz Orozco (Scuuz)
 * Fecha: 17/10/2026
 * Proyecto: TanquesPrototipo - Programación 2
 */



using UnityEngine;
using UnityEngine.UI;

public class DamageFlash : MonoBehaviour
{
    [SerializeField] private Image _flashImage;
    [SerializeField] private Health[] _targets;

    [Header("Flash")]
    [SerializeField] private float _maxAlpha = 0.35f;
    [SerializeField] private float _fadeSpeed = 1.5f;

    [Header("Temblor")]
    [SerializeField] private float _shakeIntensity = 0.25f;
    [SerializeField] private float _shakeDuration = 0.2f;

    private float _alpha;

    private void Start()
    {
        SetAlpha(0f);
        foreach (Health target in _targets)
        {
            if (target != null) target.OnDamaged += HandleDamaged;
        }
    }

    private void OnDestroy()
    {
        foreach (Health target in _targets)
        {
            if (target != null) target.OnDamaged -= HandleDamaged;
        }
    }

    private void HandleDamaged(float amount)
    {
        _alpha = _maxAlpha;
        CameraFollow.Shake(_shakeIntensity, _shakeDuration);
    }

    private void Update()
    {
        if (_alpha <= 0f) return;

        _alpha = Mathf.MoveTowards(_alpha, 0f, _fadeSpeed * Time.deltaTime);
        SetAlpha(_alpha);
    }

    private void SetAlpha(float alpha)
    {
        Color color = _flashImage.color;
        color.a = alpha;
        _flashImage.color = color;
    }
}