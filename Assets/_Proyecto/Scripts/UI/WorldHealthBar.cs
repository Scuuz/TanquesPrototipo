/*
 * WorldHealthBar.cs
 *
 * Barra de vida flotante sobre objetos
 *
 * Autor: Pablo G. Ruiz Orozco (Scuuz)
 * Fecha: 17/10/2026
 * Proyecto: TanquesPrototipo - Programación 2
 */



using UnityEngine;
using UnityEngine.UI;

public class WorldHealthBar : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private bool _hideWhenFull = true;

    private Health _health;
    private Camera _camera;

    private void Awake()
    {
        _health = GetComponentInParent<Health>();
        _camera = Camera.main;
    }

    private void OnEnable()
    {
        if (_health == null) return;
        _health.OnHealthChanged += UpdateBar;
        UpdateBar(_health.CurrentHealth, _health.MaxHealth);
    }

    private void OnDisable()
    {
        if (_health != null) _health.OnHealthChanged -= UpdateBar;
    }

    private void LateUpdate()
    {
        if (_camera != null) transform.rotation = _camera.transform.rotation;
    }

    private void UpdateBar(float current, float max)
    {
        float value = max > 0f ? current / max : 0f;
        _slider.value = value;

        bool show = !(_hideWhenFull && value >= 1f);
        if (_slider.gameObject.activeSelf != show) _slider.gameObject.SetActive(show);
    }
}
