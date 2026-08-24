using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class LocalizedString
{
    public string key;
    public string english;
    public string turkish;
}

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance { get; private set; }

    public enum Language { English, Turkish }
    
    [SerializeField] private Language currentLanguage = Language.English;
    [SerializeField] private List<LocalizedString> translations = new List<LocalizedString>();

    private const string LanguageSaveKey = "GameLanguage";
    private Dictionary<string, LocalizedString> _dictionary = new Dictionary<string, LocalizedString>();

    public event Action OnLanguageChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        BuildDictionary();
        LoadLanguage();
    }

    private void BuildDictionary()
    {
        _dictionary.Clear();
        foreach (var loc in translations)
        {
            if (!_dictionary.ContainsKey(loc.key))
            {
                _dictionary.Add(loc.key, loc);
            }
        }
    }

    private void LoadLanguage()
    {
        int langIndex = SaveManager.GetIntValue(LanguageSaveKey, -1);
        if (langIndex == -1)
        {
            if (Application.systemLanguage == SystemLanguage.Turkish)
                currentLanguage = Language.Turkish;
            else
                currentLanguage = Language.English;
        }
        else
        {
            currentLanguage = (Language)langIndex;
        }
    }

    public void SetLanguage(Language language)
    {
        currentLanguage = language;
        SaveManager.SetIntValue(LanguageSaveKey, (int)currentLanguage);
        
        OnLanguageChanged?.Invoke();
    }

    public string GetText(string key)
    {
        if (_dictionary.TryGetValue(key, out LocalizedString loc))
        {
            return currentLanguage == Language.Turkish ? loc.turkish : loc.english;
        }
        return key;
    }
}
