/*
 * TankMovement.cs
 *
 * Movimiento del tanque utilizando Rigidbody e Input System.
 *
 * Autor: Pablo G. Ruiz Orozco (Scuuz)
 * Fecha: 24/09/2026
 * Proyecto: TanquesPrototipo - Programación 2
 */


using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class TankMovement : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference _moveAction;

    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 8f;
    [SerializeField] private float _turnSpeed = 180f;

    private Rigidbody _rb;
    private Vector2 _input;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }
    private void OnEnable()
    {
        _moveAction.action.Enable();
    }

    private void OnDisable()
    {
        _moveAction.action.Disable();
    }

    private void Update()
    {
        _input = _moveAction.action.ReadValue<Vector2>();
        //Debug.Log(_input);
    }

    private void FixedUpdate()
    {
        Move();
        Turn();
    }

    private void Move()
    {
        float distance = _input.y * _moveSpeed * Time.fixedDeltaTime;
        Vector3 offset = transform.forward * distance;
        _rb.MovePosition(_rb.position + offset);
    }

    private void Turn()
    {
        float degrees = _input.x * _turnSpeed * Time.fixedDeltaTime;
        Quaternion rotation = Quaternion.Euler(0f, degrees, 0f);
        _rb.MoveRotation(_rb.rotation * rotation);
    }
    
}
