using UnityEngine;
[RequireComponent(typeof(Collider2D))]
public class Spike : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            if (player.IsShielded)
            {
                player.ConsumeShield();
                Destroy(gameObject); 
            }
            else if (GameManager.Instance != null)
            {
                GameManager.Instance.TriggerGameOver();
            }
        }
    }
}