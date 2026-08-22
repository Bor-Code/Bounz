using UnityEngine;

public class SizeShiftPlatform : MonoBehaviour
{
    [SerializeField] private float minMultiplier = 0.7f;
    [SerializeField] private float maxMultiplier = 1.3f;

    private bool _triggered = false;

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (_triggered) return;

        PlayerController player = col.gameObject.GetComponent<PlayerController>();
        if (player == null) return;

        _triggered = true;
        float multiplier = Random.Range(minMultiplier, maxMultiplier);
        player.ApplyScaleChange(multiplier);
    }
}
