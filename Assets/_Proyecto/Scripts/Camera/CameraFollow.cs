/*
 * CameraFollow.cs
 *
 * Cámara vista de pájaro inclinada. Sigue el punto medio entre los jugadores vivos y se aleja al separarse
 *
 * Autor: Pablo G. Ruiz Orozco (Scuuz)
 * Fecha: 24/09/2026
 * Modificación: 06/10/2026
 * Proyecto: TanquesPrototipo - Programación 2
 */


using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _target;

    [Header("Angulo y distancia de camara")]
    [Range(30f, 90f)]
    [SerializeField] private float _pitch = 55f;
    [SerializeField] private float _distance = 24f;

    [Header("Zoom cooperativo")]
    [SerializeField] private float _maxDistance = 60f;
    [SerializeField] private float _emergencyDistance = 300f;
    [Range(0.5f, 0.95f)]
    [SerializeField] private float _screenMargin = 0.8f;

    [Header("Suavizado")]
    [SerializeField] private float _smoothTime = 0.2f;

    [Header("Temblor")]
    [SerializeField] private float _maxShake = 1f;

    public static CameraFollow Main { get; private set; }
    public bool IsStretched { get; private set; }

    private Camera _camera;
    private Vector3 _velocity = Vector3.zero;
    private float _zoomVelocity;
    private Vector3 _focus;
    private float _currentDistance;
    private Vector3 _basePosition;

    private float _shakeIntensity;
    private float _shakeDuration;
    private float _shakeTimer;

    private Quaternion ViewRotation => Quaternion.Euler(_pitch, 0f, 0f);

    private void Awake()
    {
        Main = this;
        _camera = GetComponent<Camera>();
        _currentDistance = _distance;
    }

    private void OnDestroy()
    {
        if (Main == this) Main = null;
    }

    private void Start()
    {
        if (_target != null) _focus = _target.position;

        _currentDistance = UpdateFocus();
        transform.rotation = ViewRotation;
        _basePosition = _focus + ViewRotation * new Vector3(0f, 0f, -_currentDistance);
        transform.position = _basePosition;
    }

    private void LateUpdate()
    {
        float targetDistance = UpdateFocus();
        _currentDistance = Mathf.SmoothDamp(_currentDistance, targetDistance, ref _zoomVelocity, _smoothTime);

        Vector3 desiredPosition = _focus + ViewRotation * new Vector3(0f, 0f, -_currentDistance);
        _basePosition = Vector3.SmoothDamp(_basePosition, desiredPosition, ref _velocity, _smoothTime);

        transform.position = _basePosition + GetShakeOffset();
        transform.rotation = ViewRotation;
    }

    public static void Shake(float intensity, float duration)
    {
        if (Main == null) return;

        Main._shakeIntensity = Mathf.Min(Mathf.Max(Main._shakeIntensity, intensity), Main._maxShake);
        Main._shakeDuration = Mathf.Max(duration, 0.01f);
        Main._shakeTimer = Main._shakeDuration;
    }

    private Vector3 GetShakeOffset()
    {
        if (_shakeTimer <= 0f || Time.timeScale == 0f) return Vector3.zero;

        _shakeTimer -= Time.deltaTime;

        float strength = _shakeIntensity * Mathf.Clamp01(_shakeTimer / _shakeDuration);
        if (_shakeTimer <= 0f) _shakeIntensity = 0f;

        return Random.insideUnitSphere * strength;
    }

    private float UpdateFocus()
    {
        bool found = false;
        Bounds bounds = new Bounds();

        foreach (PlayerRespawn player in PlayerRegistry.Players)
        {
            if (player == null || !player.IsAlive) continue;

            if (!found)
            {
                bounds = new Bounds(player.transform.position, Vector3.zero);
                found = true;
            }
            else
            {
                bounds.Encapsulate(player.transform.position);
            }
        }

        if (!found)
        {
            IsStretched = false;
            return _currentDistance;
        }

        _focus = bounds.center;

        float tanVertical = Mathf.Tan(_camera.fieldOfView * 0.5f * Mathf.Deg2Rad) * _screenMargin;
        float tanHorizontal = tanVertical * _camera.aspect;
        Quaternion worldToView = Quaternion.Inverse(ViewRotation);

        float needed = _distance;
        foreach (PlayerRespawn player in PlayerRegistry.Players)
        {
            if (player == null || !player.IsAlive) continue;

            Vector3 local = worldToView * (player.transform.position - _focus);
            float forWidth = Mathf.Abs(local.x) / tanHorizontal - local.z;
            float forHeight = Mathf.Abs(local.y) / tanVertical - local.z;
            needed = Mathf.Max(needed, forWidth, forHeight);
        }

        IsStretched = needed > _maxDistance;
        return Mathf.Min(needed, _emergencyDistance);
    }

    public bool AllowsMove(Vector3 from, Vector3 to)
    {
        if (!IsStretched) return true;

        Vector3 currentOffset = from - _focus;
        Vector3 nextOffset = to - _focus;
        currentOffset.y = 0f;
        nextOffset.y = 0f;
        return nextOffset.sqrMagnitude <= currentOffset.sqrMagnitude;
    }

    private void OnValidate()
    {
        if (Application.isPlaying || _target == null) return;
        transform.rotation = ViewRotation;
        transform.position = _target.position + ViewRotation * new Vector3(0f, 0f, -_distance);
    }
}