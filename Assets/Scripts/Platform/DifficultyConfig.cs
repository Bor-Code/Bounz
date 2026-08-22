using UnityEngine;

[CreateAssetMenu(fileName = "DifficultyConfig", menuName = "Bounz/DifficultyConfig")]
public class DifficultyConfig : ScriptableObject
{
    [Header("Difficulty Scaling")]
    [Tooltip("How many seconds of gameplay equal one difficulty 'tick'.")]
    public float difficultyTickInterval = 10f;

    [Header("Gap Distance (horizontal)")]
    public float minGapStart = 2f;
    public float maxGapStart = 4f;
    public float minGapMax = 4f;
    public float maxGapMax = 7f;

    [Header("Platform Width")]
    public float widthStart = 4f;
    public float widthMin = 1.2f;

    [Header("Platform Height Variance")]
    public float minHeightOffsetStart = -0.5f;
    public float maxHeightOffsetStart = 0.5f;
    public float minHeightOffsetMax = -1.5f;
    public float maxHeightOffsetMax = 1.5f;

    [Header("Type Weights — Early Game")]
    public PlatformTypeWeight[] earlyWeights;

    [Header("Type Weights — Late Game")]
    public PlatformTypeWeight[] lateWeights;

    [Tooltip("How many difficulty ticks until late-game weights are fully applied.")]
    public int ticksToFullDifficulty = 20;

    public float EvaluateGapMin(float t) => Mathf.Lerp(minGapStart, minGapMax, t);
    public float EvaluateGapMax(float t) => Mathf.Lerp(maxGapStart, maxGapMax, t);
    public float EvaluateWidth(float t) => Mathf.Lerp(widthStart, widthMin, t);
    public float EvaluateHeightMin(float t) => Mathf.Lerp(minHeightOffsetStart, minHeightOffsetMax, t);
    public float EvaluateHeightMax(float t) => Mathf.Lerp(maxHeightOffsetStart, maxHeightOffsetMax, t);

    public PlatformType PickType(float t)
    {
        float[] weights = new float[earlyWeights.Length];
        for (int i = 0; i < weights.Length; i++)
            weights[i] = Mathf.Lerp(earlyWeights[i].weight, lateWeights[i].weight, t);

        float total = 0f;
        foreach (float w in weights) total += w;

        float roll = Random.Range(0f, total);
        float cumulative = 0f;
        for (int i = 0; i < weights.Length; i++)
        {
            cumulative += weights[i];
            if (roll <= cumulative)
                return earlyWeights[i].type;
        }

        return PlatformType.Safe;
    }
}
