using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PromoCodeUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField codeInputField;
    [SerializeField] private Button submitButton;
    [SerializeField] private TMP_Text statusMessageText;
    [SerializeField] private GameObject resultPanel;

    private void Start()
    {
        if (submitButton != null)
        {
            submitButton.onClick.AddListener(OnSubmitClicked);
        }
        if (resultPanel != null) resultPanel.SetActive(false);
    }

    private void OnEnable()
    {
        if (PromoCodeManager.Instance != null)
        {
            PromoCodeManager.Instance.OnCodeRedeemedSuccess += HandleSuccess;
            PromoCodeManager.Instance.OnCodeRedeemedFailed += HandleFail;
        }
    }

    private void OnDisable()
    {
        if (PromoCodeManager.Instance != null)
        {
            PromoCodeManager.Instance.OnCodeRedeemedSuccess -= HandleSuccess;
            PromoCodeManager.Instance.OnCodeRedeemedFailed -= HandleFail;
        }
    }

    private void OnSubmitClicked()
    {
        if (codeInputField != null && PromoCodeManager.Instance != null)
        {
            PromoCodeManager.Instance.RedeemCode(codeInputField.text);
        }
    }

    private void HandleSuccess(string message)
    {
        ShowResult(message, Color.green);
        if (codeInputField != null) codeInputField.text = "";
    }

    private void HandleFail(string message)
    {
        ShowResult(message, Color.red);
    }

    private void ShowResult(string msg, Color color)
    {
        if (resultPanel != null) resultPanel.SetActive(true);
        if (statusMessageText != null)
        {
            statusMessageText.color = color;
            statusMessageText.text = msg;
        }
    }

    public void ClosePanel()
    {
        if (resultPanel != null) resultPanel.SetActive(false);
    }
}
