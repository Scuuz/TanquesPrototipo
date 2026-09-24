/*
 * CameraFollow.cs
 *
 * Cámara cenital que sigue al tanque.
 *
 * Autor: Pablo G. Ruiz Orozco (Scuuz)
 * Fecha: 24/09/2026
 * Proyecto: TanquesPrototipo - Programación 2
 */


using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private Vector3 _offset = new Vector3(0f, 18f, -5f);
    [SerializeField] private float _smoothTime = 0.2f;

    private Vector3 _velocity = Vector3.zero;

    private void LateUpdate()
    {
        Vector3 desiredPosition = _target.position + _offset;
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref _velocity, _smoothTime);

    }

}
