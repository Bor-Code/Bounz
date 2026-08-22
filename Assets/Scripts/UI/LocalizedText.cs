using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class LocalizedText : MonoBehaviour
{
    [SerializeField] private string localizationKey;
    
    private TMP_Text _textComponent;

    private void Awake()
    {
        _textComponent = GetComponent<TMP_Text>();
    }

    private void Start()
    {
        UpdateText();
    }

    private void OnEnable()
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged += UpdateText;
        }
    }

    private void OnDisable()
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged -= UpdateText;
        }
    }

    public void UpdateText()
    {
        if (LocalizationManager.Instance != null && !string.IsNullOrEmpty(localizationKey))
        {
            _textComponent.text = LocalizationManager.Instance.GetText(localizationKey);
        }
    }
}
