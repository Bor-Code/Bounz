using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EnergyUI : MonoBehaviour
{
    [SerializeField] private TMP_Text energyText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private Button buyWithCoinsButton;
    [SerializeField] private Button watchAdButton;
    [SerializeField] private GameObject outOfEnergyPanel;

    private void Start()
    {
        if (buyWithCoinsButton != null) buyWithCoinsButton.onClick.AddListener(OnBuyWithCoinsClicked);
        if (watchAdButton != null) watchAdButton.onClick.AddListener(OnWatchAdClicked);
        UpdateUI();
    }

    private void OnEnable()
    {
        if (EnergyManager.Instance != null)
        {
            EnergyManager.Instance.OnEnergyUpdated += UpdateUI;
        }
    }

    private void OnDisable()
    {
        if (EnergyManager.Instance != null)
        {
            EnergyManager.Instance.OnEnergyUpdated -= UpdateUI;
        }
    }

    private void UpdateUI()
    {
        if (EnergyManager.Instance == null) return;

        if (energyText != null)
        {
            energyText.text = $"{EnergyManager.Instance.CurrentEnergy}/5";
        }

        if (EnergyManager.Instance.CurrentEnergy >= 5)
        {
            if (timerText != null) timerText.text = "DOLU";
            if (outOfEnergyPanel != null) outOfEnergyPanel.SetActive(false);
        }
        else
        {
            if (timerText != null) 
            {
                var time = EnergyManager.Instance.TimeUntilNextEnergy;
                timerText.text = $"{time.Minutes:D2}:{time.Seconds:D2}";
            }
        }
    }

    public void ShowOutOfEnergyPanel()
    {
        if (outOfEnergyPanel != null) outOfEnergyPanel.SetActive(true);
    }

    private void OnBuyWithCoinsClicked()
    {
        if (EnergyManager.Instance != null)
        {
            EnergyManager.Instance.RefillEnergyWithCoins();
        }
    }

    private void OnWatchAdClicked()
    {
        if (EnergyManager.Instance != null)
        {
            EnergyManager.Instance.RefillEnergyWithAd();
        }
    }
}
