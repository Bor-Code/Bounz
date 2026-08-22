using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    [SerializeField] private float defaultDuration = 0.2f;
    [SerializeField] private float defaultMagnitude = 0.3f;

    private Vector3 _originalPos;
    private Coroutine _shakeCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void OnEnable()
    {
        GameEvents.OnPlayerLanded += HandleLand;
        GameEvents.OnPlayerDied += HandleDeath;
        GameEvents.OnPlatformBroken += HandleBreak;
    }

    private void OnDisable()
    {
        GameEvents.OnPlayerLanded -= HandleLand;
        GameEvents.OnPlayerDied -= HandleDeath;
        GameEvents.OnPlatformBroken -= HandleBreak;
    }

    private void HandleLand(float impactSpeed)
    {
        if (impactSpeed > 8f)
        {
            Shake(0.15f, 0.2f);
        }
    }

    private void HandleDeath(Vector3 _)
    {
        Shake(0.4f, 0.5f);
    }

    private void HandleBreak(Vector3 _)
    {
        Shake(0.2f, 0.3f);
    }

    public void Shake(float duration, float magnitude)
    {
        if (_shakeCoroutine != null) StopCoroutine(_shakeCoroutine);
        _shakeCoroutine = StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        _originalPos = transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(_originalPos.x + x, _originalPos.y + y, _originalPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = _originalPos;
    }
}
