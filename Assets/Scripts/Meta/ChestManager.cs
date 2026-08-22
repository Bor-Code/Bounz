using UnityEngine;
using System;
using System.Collections.Generic;

public class ChestManager : MonoBehaviour
{
    public static ChestManager Instance { get; private set; }

    [SerializeField] private int chestCost = 500;
    [SerializeField] private int jackpotReward = 2000;
    [SerializeField] private SkinConfig skinConfig;

    public event Action<string> OnSkinWon;
    public event Action<int> OnCoinsWon;
    public event Action OnChestFailed;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void OpenChest()
    {
        if (SaveManager.Instance == null || SkinManager.Instance == null)
        {
            OnChestFailed?.Invoke();
            return;
        }

        if (SaveManager.Instance.CurrentSave.totalScore < chestCost)
        {
            OnChestFailed?.Invoke();
            return;
        }

        SaveManager.Instance.CurrentSave.totalScore -= chestCost;
        SaveManager.Instance.SaveGame();

        DetermineReward();
    }

    private void DetermineReward()
    {
        List<string> unownedSkins = new List<string>();

        if (skinConfig != null && SaveManager.Instance != null)
        {
            foreach (var skin in skinConfig.skins)
            {
                if (!SaveManager.Instance.CurrentSave.unlockedSkins.Contains(skin.id))
                {
                    unownedSkins.Add(skin.id);
                }
            }
        }

        bool giveSkin = false;
        if (unownedSkins.Count > 0)
        {
            giveSkin = UnityEngine.Random.Range(0, 100) < 60;
        }

        if (giveSkin)
        {
            int randomIndex = UnityEngine.Random.Range(0, unownedSkins.Count);
            string wonSkinId = unownedSkins[randomIndex];
            
            SaveManager.Instance.CurrentSave.unlockedSkins.Add(wonSkinId);
            SaveManager.Instance.SaveGame();
            
            OnSkinWon?.Invoke(wonSkinId);
        }
        else
        {
            SaveManager.Instance.CurrentSave.totalScore += jackpotReward;
            SaveManager.Instance.SaveGame();
            
            OnCoinsWon?.Invoke(jackpotReward);
        }
    }
}
