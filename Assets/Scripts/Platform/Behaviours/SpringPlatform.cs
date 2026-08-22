using UnityEngine;

public class SpringPlatform : MonoBehaviour
{
    [SerializeField] private float launchForce = 16f;

    private void OnCollisionEnter2D(Collision2D col)
    {
        PlayerController player = col.gameObject.GetComponent<PlayerController>();
        if (player == null) return;

        GameEvents.RaiseSpringBounced();
        player.ForceJump(launchForce);
    }
}
