using System.Collections;
using UnityEngine;

public class FragilePlatform : MonoBehaviour
{
    [SerializeField] private float collapseDelay = 1f;
    [SerializeField] private float breakAnimDuration = 0.3f;

    private bool _triggered = false;

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (_triggered) return;
        if (col.gameObject.GetComponent<PlayerController>() == null) return;

        _triggered = true;
        StartCoroutine(CollapseRoutine(col.gameObject.GetComponent<PlayerController>()));
    }

    private IEnumerator CollapseRoutine(PlayerController player)
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

        Destroy(gameObject);
        GameManager.Instance?.TriggerGameOver();
    }
}
