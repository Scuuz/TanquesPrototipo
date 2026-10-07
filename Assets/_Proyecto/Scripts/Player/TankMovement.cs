/*
 * TankMovement.cs
 *
 * Movimiento del tanque con Rigidbody e Input Manager(Jugador 1 y 2).
 *
 * Autor: Pablo G. Ruiz Orozco (Scuuz)
 * Fecha: 24/09/2026
 * Modificación: 06/10/2026
 * Proyecto: TanquesPrototipo - Programación 2
 */


using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class TankMovement : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private ControlScheme _controlScheme = ControlScheme.KeyboardMouse;

    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 8f;
    [SerializeField] private float _turnSpeed = 180f;
    [SerializeField] private float _stickTurnSpeed = 540f;

    [Header("Boost")]
    [SerializeField] private float _boostMultiplier = 1.8f;
    [SerializeField] private float _boostDuration = 1.2f;
    [SerializeField] private float _boostCooldown = 5f;

    public bool IsBoosting => Time.time < _boostEndTime;

    public float BoostReady01 => Mathf.Clamp01(1f - (_nextBoostTime - Time.time) / Mathf.Max(_boostCooldown, 0.01f));

    private float CurrentSpeed => IsBoosting ? _moveSpeed * _boostMultiplier : _moveSpeed;

    private Rigidbody _rb;
    private Vector2 _input;
    private CameraFollow _cameraFollow;
    private float _boostEndTime;
    private float _nextBoostTime;

    private void Awake()
    {
        if (Camera.main != null) _cameraFollow = Camera.main.GetComponent<CameraFollow>();
        _rb = GetComponent<Rigidbody>();
    }

    private void OnDisable()
    {
        _input = Vector2.zero;
    }

    private void Update()
    {
        _input = _controlScheme == ControlScheme.Gamepad ? ReadGamepad() : ReadKeyboard();

        if (BoostPressed() && Time.time >= _nextBoostTime)
        {
            _boostEndTime = Time.time + _boostDuration;
            _nextBoostTime = Time.time + _boostCooldown;
        }
    }

    private Vector2 ReadKeyboard()
    {
        float forward = 0f;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) forward += 1f;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) forward -= 1f;

        float turn = 0f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) turn += 1f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) turn -= 1f;

        return new Vector2(turn, forward);
    }

    private Vector2 ReadGamepad()
    {
        Gamepad pad = Gamepad.current;
        if (pad == null) return Vector2.zero;

        return pad.leftStick.ReadValue();
    }

    private bool BoostPressed()
    {
        if (_controlScheme == ControlScheme.Gamepad)
        {
            Gamepad pad = Gamepad.current;
            if (pad == null) return false;

            return pad.leftTrigger.wasPressedThisFrame || pad.leftShoulder.wasPressedThisFrame;
        }

        return Input.GetKeyDown(KeyCode.LeftShift);
    }

    private void FixedUpdate()
    {
        if (_controlScheme == ControlScheme.Gamepad)
        {
            MoveTowardsStick();
        }
        else
        {
            Move();
            Turn();
        }
    }

    private void Move()
    {
        float distance = _input.y * CurrentSpeed * Time.fixedDeltaTime;
        Vector3 target = _rb.position + transform.forward * distance;

        if (CanMoveTo(target)) _rb.MovePosition(target);
    }

    private void Turn()
    {
        float degrees = _input.x * _turnSpeed * Time.fixedDeltaTime;
        Quaternion rotation = Quaternion.Euler(0f, degrees, 0f);
        _rb.MoveRotation(_rb.rotation * rotation);
    }

    private void MoveTowardsStick()
    {
        Vector3 direction = new Vector3(_input.x, 0f, _input.y);
        if (direction.sqrMagnitude < 0.01f) return;

        Vector3 offset = Vector3.ClampMagnitude(direction, 1f) * CurrentSpeed * Time.fixedDeltaTime;
        Vector3 target = _rb.position + offset;
        if (CanMoveTo(target)) _rb.MovePosition(target);

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        Quaternion newRotation = Quaternion.RotateTowards(_rb.rotation, targetRotation, _stickTurnSpeed * Time.fixedDeltaTime);
        _rb.MoveRotation(newRotation);
    }

    private bool CanMoveTo(Vector3 target)
    {
        return _cameraFollow == null || _cameraFollow.AllowsMove(_rb.position, target);
    }
}