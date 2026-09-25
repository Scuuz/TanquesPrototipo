/*
 * Shell.cs
 *
 * Proyectil del tanque que se destruye al impactar o pasar el tiempo de vida
 *
 * Autor: Pablo G. Ruiz Orozco (Scuuz)
 * Fecha: 25/09/2026
 * Proyecto: TanquesPrototipo - Programación 2
 */


using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Shell : MonoBehaviour
{
    [SerializeField] private float _speed = 20f;
    [SerializeField] private float _lifeTime = 3f;

    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    } 

    public void Launch (Vector3 direction)
    {
        _rb.linearVelocity = direction.normalized * _speed;
        Destroy(gameObject, _lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy (gameObject);
    }
}
