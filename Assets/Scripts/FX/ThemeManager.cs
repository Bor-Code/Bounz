using UnityEngine;
using System;

[Serializable]
public class ThemeMilestone
{
    public int scoreThreshold;
    public Color backgroundColor;
}

public class ThemeManager : MonoBehaviour
{
    public static ThemeManager Instance { get; private set; }

    [SerializeField] private Camera mainCamera;
    [SerializeField] private ThemeMilestone[] milestones;
    [SerializeField] private float transitionSpeed = 1f;

    private int _currentMilestoneIndex = 0;
    private Color _targetColor;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (mainCamera == null)
            mainCamera = Camera.main;
            
        if (milestones != null && milestones.Length > 0)
        {
            _targetColor = milestones[0].backgroundColor;
            if (mainCamera != null)
                mainCamera.backgroundColor = _targetColor;
        }
    }

    private void OnEnable()
    {
        ScoreEvents.OnScoreChanged += HandleScoreChanged;
    }

    private void OnDisable()
    {
        ScoreEvents.OnScoreChanged -= HandleScoreChanged;
    }

    private void Update()
    {
        if (mainCamera != null && mainCamera.backgroundColor != _targetColor)
        {
            mainCamera.backgroundColor = Color.Lerp(mainCamera.backgroundColor, _targetColor, Time.deltaTime * transitionSpeed);
        }
    }

    private void HandleScoreChanged(int newScore)
    {
        if (milestones == null || milestones.Length == 0) return;

        int nextIndex = _currentMilestoneIndex + 1;
        if (nextIndex < milestones.Length && newScore >= milestones[nextIndex].scoreThreshold)
        {
            _currentMilestoneIndex = nextIndex;
            _targetColor = milestones[_currentMilestoneIndex].backgroundColor;
        }
    }
}
