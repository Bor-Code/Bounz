using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Coin : MonoBehaviour
{
    [Tooltip("Bu altın kaç puan/para değerinde?")]
    [SerializeField] private int coinValue = 1;
    [Tooltip("Mıknatıs ne kadar uzaktan çekecek?")]
    [SerializeField] private float magnetRadius = 6f;
    [SerializeField] private float magnetMoveSpeed = 15f;

    private bool _isCollected = false;

    private void Update()
    {
        if (_isCollected || GameManager.Instance == null) return;

        PlayerController player = GameManager.Instance.Player;
        if (player != null && player.IsMagnetActive)
        {
            float dist = Vector2.Distance(transform.position, player.transform.position);
            if (dist <= magnetRadius)
            {
                transform.position = Vector2.MoveTowards(transform.position, player.transform.position, magnetMoveSpeed * Time.deltaTime);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isCollected) return;
        if (other.GetComponent<PlayerController>() != null)
        {
            _isCollected = true;
            int finalValue = GetFinalCoinValue();
            if (SkinManager.Instance != null)
            {
                SkinManager.Instance.AddTotalScore(finalValue);
            }
            else if (SaveManager.Instance != null)
            {
                SaveManager.Instance.CurrentSave.totalScore += finalValue;
                SaveManager.Instance.SaveGame();
            }
            GameEvents.RaiseCoinCollected(transform.position);
            Destroy(gameObject);
        }
    }

    private int GetFinalCoinValue()
    {
        float multiplier = UpgradeManager.Instance != null
            ? UpgradeManager.Instance.GetUpgradeValue(UpgradeManager.CoinMultiplierUpgradeId, 1f)
            : 1f;

        return Mathf.Max(1, Mathf.RoundToInt(coinValue * multiplier));
    }
}
