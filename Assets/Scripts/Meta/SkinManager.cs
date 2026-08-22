using UnityEngine;
public class SkinManager : MonoBehaviour
{
    public static SkinManager Instance { get; private set; }
    [SerializeField] private SkinConfig skinConfig;
    [Tooltip("Renk değişikliğinin uygulanacağı ana karakterin görsel SpriteRenderer'ı (Visual altındaki)")]
    [SerializeField] private SpriteRenderer playerVisual;
    private const string SelectedSkinKey = "SelectedSkin";
    private const string UnlockedSkinsPrefix = "UnlockedSkin_";
    private const string TotalScoreKey = "TotalScore";
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }
    private void Start()
    {
        if (skinConfig != null && skinConfig.skins.Length > 0)
        {
            UnlockSkin(skinConfig.skins[0].id);
        }
        ApplySelectedSkin();
    }
    public int GetTotalScore()
    {
        return SaveManager.Instance != null ? SaveManager.Instance.CurrentSave.totalScore : 0;
    }
    public void AddTotalScore(int amount)
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.CurrentSave.totalScore += amount;
            SaveManager.Instance.SaveGame();
        }
    }
    public bool SpendTotalScore(int amount)
    {
        int current = GetTotalScore();
        if (current >= amount && SaveManager.Instance != null)
        {
            SaveManager.Instance.CurrentSave.totalScore -= amount;
            SaveManager.Instance.SaveGame();
            return true;
        }
        return false;
    }
    public bool IsSkinUnlocked(string id)
    {
        if (SaveManager.Instance != null)
        {
            return SaveManager.Instance.CurrentSave.unlockedSkins.Contains(id);
        }
        return false;
    }
    private void UnlockSkin(string id)
    {
        if (SaveManager.Instance != null && !SaveManager.Instance.CurrentSave.unlockedSkins.Contains(id))
        {
            SaveManager.Instance.CurrentSave.unlockedSkins.Add(id);
            SaveManager.Instance.SaveGame();
        }
    }
    public string GetSelectedSkinId()
    {
        if (SaveManager.Instance != null && !string.IsNullOrEmpty(SaveManager.Instance.CurrentSave.selectedSkinId))
        {
            return SaveManager.Instance.CurrentSave.selectedSkinId;
        }
        return skinConfig != null && skinConfig.skins.Length > 0 ? skinConfig.skins[0].id : "";
    }
    public void SelectSkin(string id)
    {
        if (IsSkinUnlocked(id) && SaveManager.Instance != null)
        {
            SaveManager.Instance.CurrentSave.selectedSkinId = id;
            SaveManager.Instance.SaveGame();
            ApplySelectedSkin();
        }
    }
    public bool BuySkin(string id, int price)
    {
        if (!IsSkinUnlocked(id) && SpendTotalScore(price))
        {
            UnlockSkin(id);
            SelectSkin(id); 
            if (AnalyticsManager.Instance != null)
            {
                AnalyticsManager.Instance.LogSkinPurchased(id, price);
            }
            return true;
        }
        return false;
    }
    public void ApplySelectedSkin()
    {
        if (skinConfig == null || playerVisual == null) return;
        string activeId = GetSelectedSkinId();
        SkinData data = skinConfig.GetSkin(activeId);
        if (data != null)
        {
            playerVisual.color = data.color;
        }
    }
}