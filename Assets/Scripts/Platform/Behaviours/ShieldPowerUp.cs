using UnityEngine;
[RequireComponent(typeof(Collider2D))]
public class ShieldPowerUp : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            if (!player.IsShielded)
            {
                player.ActivateShield();
            }
            Destroy(gameObject);
        }
    }
}