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
        return PlayerPrefs.GetInt(TotalScoreKey, 0);
    }
    public void AddTotalScore(int amount)
    {
        int current = GetTotalScore();
        PlayerPrefs.SetInt(TotalScoreKey, current + amount);
        PlayerPrefs.Save();
    }
    public bool SpendTotalScore(int amount)
    {
        int current = GetTotalScore();
        if (current >= amount)
        {
            PlayerPrefs.SetInt(TotalScoreKey, current - amount);
            PlayerPrefs.Save();
            return true;
        }
        return false;
    }
    public bool IsSkinUnlocked(string id)
    {
        return PlayerPrefs.GetInt(UnlockedSkinsPrefix + id, 0) == 1;
    }
    private void UnlockSkin(string id)
    {
        PlayerPrefs.SetInt(UnlockedSkinsPrefix + id, 1);
        PlayerPrefs.Save();
    }
    public string GetSelectedSkinId()
    {
        return PlayerPrefs.GetString(SelectedSkinKey, skinConfig != null && skinConfig.skins.Length > 0 ? skinConfig.skins[0].id : "");
    }
    public void SelectSkin(string id)
    {
        if (IsSkinUnlocked(id))
        {
            PlayerPrefs.SetString(SelectedSkinKey, id);
            PlayerPrefs.Save();
            ApplySelectedSkin();
        }
    }
    public bool BuySkin(string id, int price)
    {
        if (!IsSkinUnlocked(id) && SpendTotalScore(price))
        {
            UnlockSkin(id);
            SelectSkin(id); 
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