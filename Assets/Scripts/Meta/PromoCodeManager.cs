using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class PromoCode
{
    public string code;
    public int rewardCoins;
    public string rewardSkinId;
}

public class PromoCodeManager : MonoBehaviour
{
    public static PromoCodeManager Instance { get; private set; }

    [SerializeField] private List<PromoCode> validCodes = new List<PromoCode>();
    
    private const string RedeemedCodesKey = "RedeemedCodes_";

    public event Action<string> OnCodeRedeemedSuccess;
    public event Action<string> OnCodeRedeemedFailed;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        if (validCodes.Count == 0)
        {
            validCodes.Add(new PromoCode { code = "BOUNZ2026", rewardCoins = 5000, rewardSkinId = "" });
            validCodes.Add(new PromoCode { code = "FREEVIP", rewardCoins = 0, rewardSkinId = "vip_skin" });
        }
    }

    public void RedeemCode(string inputCode)
    {
        if (string.IsNullOrEmpty(inputCode))
        {
            OnCodeRedeemedFailed?.Invoke("Geçersiz veya boş kod.");
            return;
        }

        string upperCode = inputCode.ToUpperInvariant().Trim();

        if (SaveManager.GetIntValue(RedeemedCodesKey + upperCode, 0) == 1)
        {
            OnCodeRedeemedFailed?.Invoke("Bu kod zaten kullanılmış.");
            return;
        }

        PromoCode foundCode = validCodes.Find(c => c.code.ToUpperInvariant() == upperCode);

        if (foundCode != null)
        {
            GrantReward(foundCode);
            SaveManager.SetIntValue(RedeemedCodesKey + upperCode, 1);
            OnCodeRedeemedSuccess?.Invoke("Tebrikler! Ödül başarıyla eklendi.");
        }
        else
        {
            OnCodeRedeemedFailed?.Invoke("Böyle bir kod bulunamadı veya süresi dolmuş.");
        }
    }

    private void GrantReward(PromoCode code)
    {
        if (SaveManager.Instance != null)
        {
            if (code.rewardCoins > 0)
            {
                SaveManager.Instance.CurrentSave.totalScore += code.rewardCoins;
            }

            if (!string.IsNullOrEmpty(code.rewardSkinId) && !SaveManager.Instance.CurrentSave.unlockedSkins.Contains(code.rewardSkinId))
            {
                SaveManager.Instance.CurrentSave.unlockedSkins.Add(code.rewardSkinId);
            }

            SaveManager.Instance.SaveGame();
        }
    }
}
