using UnityEngine;
[RequireComponent(typeof(Collider2D))]
public class ScoreMultiplierPowerUp : MonoBehaviour
{
    [SerializeField] private float duration = 5f;
    [SerializeField] private float multiplierAmount = 2f;
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            player.ActivateMultiplier(duration, multiplierAmount);
            Destroy(gameObject);
        }
    }
}