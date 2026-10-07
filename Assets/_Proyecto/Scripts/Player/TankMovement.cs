/*
 * TankMovement.cs
 *
 * Movimiento del tanque con Rigidbody e Input Manager.
 *
 * Autor: Pablo G. Ruiz Orozco (Scuuz)
 * Fecha: 24/09/2026
 * Modificación: 06/10/2026
 * Proyecto: TanquesPrototipo - Programación 2
 */


using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TankMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 8f;
    [SerializeField] private float _turnSpeed = 180f;

    private Rigidbody _rb;
    private Vector2 _input;   // x = giro, y = avance

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void OnDisable()
    {
        // Si el script se apaga (muerte), no queda input "pegado"
        _input = Vector2.zero;
    }

    private void Update()
    {
        float forward = 0f;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) forward += 1f;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) forward -= 1f;

        float turn = 0f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) turn += 1f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) turn -= 1f;

        _input = new Vector2(turn, forward);
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

    private void Turn ()
    {
        float degrees = _input.x * _turnSpeed * Time.fixedDeltaTime;
        Quaternion rotation = Quaternion.Euler(0f, degrees, 0f);
        _rb.MoveRotation(_rb.rotation * rotation);
    }
}
