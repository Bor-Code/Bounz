using UnityEngine;
using System;

public class PiggyBankManager : MonoBehaviour
{
    public static PiggyBankManager Instance { get; private set; }

    private const string PiggyBankCoinsKey = "PiggyBankCoins";
    private const string PiggyBankLevelKey = "PiggyBankLevel";

    [SerializeField] private int baseCapacity = 2000;
    [SerializeField] private int capacityIncreasePerLevel = 1000;
    [SerializeField] private float scoreToCoinRatio = 0.5f;

    public int CurrentCoins { get; private set; }
    public int CurrentLevel { get; private set; }
    public int CurrentCapacity => baseCapacity + (CurrentLevel * capacityIncreasePerLevel);

    public event Action OnPiggyBankUpdated;
    public event Action OnPiggyBankSmashed;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadPiggyBank();
    }

    private void OnEnable()
    {
        ScoreEvents.OnGameOver += HandleGameOver;
    }

    private void OnDisable()
    {
        ScoreEvents.OnGameOver -= HandleGameOver;
    }

    private void LoadPiggyBank()
    {
        CurrentCoins = SaveManager.GetIntValue(PiggyBankCoinsKey, 0);
        CurrentLevel = SaveManager.GetIntValue(PiggyBankLevelKey, 0);
    }

    private void SavePiggyBank()
    {
        SaveManager.SetIntValue(PiggyBankCoinsKey, CurrentCoins);
        SaveManager.SetIntValue(PiggyBankLevelKey, CurrentLevel);
        OnPiggyBankUpdated?.Invoke();
    }

    private void HandleGameOver(int finalScore, bool isNewHighScore)
    {
        if (CurrentCoins >= CurrentCapacity) return;

        int coinsToBank = Mathf.FloorToInt(finalScore * scoreToCoinRatio);
        if (coinsToBank > 0)
        {
            CurrentCoins = Mathf.Min(CurrentCoins + coinsToBank, CurrentCapacity);
            SavePiggyBank();
        }
    }

    public bool IsFull()
    {
        return CurrentCoins >= CurrentCapacity;
    }

    public void PurchaseAndSmashPiggyBank()
    {
        if (CurrentCoins <= 0) return;

        if (IAPManager.Instance != null)
        {
            IAPManager.Instance.BuyProduct(IAPManager.PRODUCT_PIGGY_BANK_SMASH, SmashPiggyBank);
        }
        else
        {
            SmashPiggyBank();
        }
    }

    public void SmashPiggyBank()
    {
        if (CurrentCoins > 0 && SaveManager.Instance != null)
        {
            SaveManager.Instance.CurrentSave.totalScore += CurrentCoins;
            SaveManager.Instance.SaveGame();

            CurrentCoins = 0;
            CurrentLevel++;
            SavePiggyBank();
            OnPiggyBankSmashed?.Invoke();
        }
    }
}
