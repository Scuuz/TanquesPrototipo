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
using UnityEngine.Pool;

[RequireComponent(typeof(Rigidbody))]
public class Shell : MonoBehaviour
{
    [SerializeField] private float _speed = 20f;
    [SerializeField] private float _lifeTime = 3f;

    private Rigidbody _rb;
    private IObjectPool<Shell> _pool;
    private float _lifeTimer;
    private bool _isReleased;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    } 

    public void SetPool (IObjectPool<Shell> pool)
    {
        _pool = pool;
    }

    public void Launch (Vector3 position, Quaternion rotation, Vector3 direction)
    {
        transform.SetPositionAndRotation(position, rotation);
        _rb.position = position;
        _rb.rotation = rotation;

        _isReleased = false;
        _lifeTimer = _lifeTime;
        _rb.linearVelocity = direction.normalized * _speed;
    }

    private void Update()
    {
        _lifeTimer -= Time.deltaTime;
        if (_lifeTimer <= 0f)
        {
            ReturnToPool();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //float flightTime = _lifeTime - _lifeTimer;
        //Debug.Log($"{name} chocó con {other.name} después de {flightTime:F2} s", other);
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        if (_isReleased) return;
        _isReleased = true;
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        if (_pool != null) 
        {
            _pool.Release(this);
        }
        else 
        {
            Destroy(gameObject);
        }
    }
}
