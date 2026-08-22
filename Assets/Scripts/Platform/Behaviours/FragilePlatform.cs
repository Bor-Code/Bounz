using System.Collections;
using UnityEngine;

public class FragilePlatform : MonoBehaviour
{
    [SerializeField] private float collapseDelay    = 1f;
    [SerializeField] private float breakAnimDuration = 0.3f;

    private bool _triggered = false;

    /// <summary>Pool'dan alınınca çağrılır — önceki tetikleme durumunu sıfırlar.</summary>
    public void ResetState()
    {
        _triggered = false;
        StopAllCoroutines();
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (_triggered) return;
        if (col.gameObject.GetComponent<PlayerController>() == null) return;

        _triggered = true;
        StartCoroutine(CollapseRoutine());
    }

    private IEnumerator CollapseRoutine()
    {
        yield return new WaitForSeconds(collapseDelay);

        float elapsed = 0f;
        Vector3 originalScale = transform.localScale;

        while (elapsed < breakAnimDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / breakAnimDuration;
            transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);
            yield return null;
        }

        Vector3 brokenPos = transform.position;
        GameEvents.RaisePlatformBroken(brokenPos);
        GameManager.Instance?.TriggerGameOver();

        // Destroy yerine havuza geri döndür
        GetComponent<Platform>()?.Cleanup();
        PlatformPool.Instance?.Return(gameObject);
    }
}
