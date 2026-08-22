using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkinItemUI : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text priceOrStatusText;
    [SerializeField] private Image colorPreview;
    [SerializeField] private Button actionButton;

    private SkinData _skin;
    private bool _isUnlocked;
    private System.Action<SkinData, bool> _onClickCallback;

    public void Setup(SkinData skin, bool isUnlocked, bool isSelected, System.Action<SkinData, bool> onClick)
    {
        _skin = skin;
        _isUnlocked = isUnlocked;
        _onClickCallback = onClick;

        if (nameText != null) nameText.text = skin.displayName;
        if (colorPreview != null) colorPreview.color = skin.color;

        if (priceOrStatusText != null)
        {
            if (isSelected)
                priceOrStatusText.text = "EQUIPPED";
            else if (isUnlocked)
                priceOrStatusText.text = "EQUIP";
            else
                priceOrStatusText.text = skin.price.ToString();
        }

        // Seçiliyse buton tıklanamaz olsun (zaten seçili)
        if (actionButton != null)
        {
            actionButton.interactable = !isSelected;
            actionButton.onClick.RemoveAllListeners();
            actionButton.onClick.AddListener(OnButtonClicked);
        }
    }

    private void OnButtonClicked()
    {
        _onClickCallback?.Invoke(_skin, _isUnlocked);
    }
}
