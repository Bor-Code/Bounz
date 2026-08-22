using UnityEngine;
using System;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    private const string TutorialSaveKey = "IsTutorialCompleted";

    public bool IsTutorialCompleted { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadTutorialStatus();
    }

    private void LoadTutorialStatus()
    {
        if (SaveManager.Instance != null && SaveManager.Instance.CurrentSave != null)
        {
            IsTutorialCompleted = PlayerPrefs.GetInt(TutorialSaveKey, 0) == 1;
        }
        else
        {
            IsTutorialCompleted = PlayerPrefs.GetInt(TutorialSaveKey, 0) == 1;
        }
    }

    public void CompleteTutorial()
    {
        IsTutorialCompleted = true;
        PlayerPrefs.SetInt(TutorialSaveKey, 1);
        PlayerPrefs.Save();
    }
}
