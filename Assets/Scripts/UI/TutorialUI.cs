using UnityEngine;
using TMPro;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private TMP_Text tutorialText;

    private void Start()
    {
        if (TutorialManager.Instance != null && !TutorialManager.Instance.IsTutorialCompleted)
        {
            if (tutorialPanel != null) tutorialPanel.SetActive(true);
            if (tutorialText != null) tutorialText.text = "Hold to Charge\nRelease to Jump!";
        }
        else
        {
            if (tutorialPanel != null) tutorialPanel.SetActive(false);
        }
    }

    private void OnEnable()
    {
        GameEvents.OnPlayerJumped += HandleFirstJump;
    }

    private void OnDisable()
    {
        GameEvents.OnPlayerJumped -= HandleFirstJump;
    }

    private void HandleFirstJump(float chargeRatio)
    {
        if (TutorialManager.Instance != null && !TutorialManager.Instance.IsTutorialCompleted)
        {
            TutorialManager.Instance.CompleteTutorial();
            if (tutorialPanel != null) tutorialPanel.SetActive(false);
        }
    }
}
