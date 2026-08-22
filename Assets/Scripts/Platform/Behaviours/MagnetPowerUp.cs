using UnityEngine;
[RequireComponent(typeof(Collider2D))]
public class MagnetPowerUp : MonoBehaviour
{
    [SerializeField] private float duration = 5f;
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            player.ActivateMagnet(duration);
            Destroy(gameObject);
        }
    }
}