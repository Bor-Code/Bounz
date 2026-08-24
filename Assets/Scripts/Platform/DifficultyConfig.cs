

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

        EnsureWeights();

        float[] weights = new float[earlyWeights.Length];

        for (int i = 0; i < weights.Length; i++)

            weights[i] = Mathf.Lerp(earlyWeights[i].weight, lateWeights[i].weight, t);

        float total = 0f;

        foreach (float w in weights) total += Mathf.Max(0f, w);

        if (total <= 0f) return PlatformType.Safe;

        float roll = Random.Range(0f, total);

        float cumulative = 0f;

        for (int i = 0; i < weights.Length; i++)

        {

            cumulative += Mathf.Max(0f, weights[i]);

            if (roll <= cumulative)

                return earlyWeights[i].type;

        }

        return PlatformType.Safe;

    }



    public static DifficultyConfig CreateDefault()

    {

        DifficultyConfig config = CreateInstance<DifficultyConfig>();

        config.name = "RuntimeDifficultyConfig";

        config.EnsureWeights();

        return config;

    }



    public void EnsureWeights()

    {

        if (earlyWeights != null && lateWeights != null && earlyWeights.Length > 0 && earlyWeights.Length == lateWeights.Length) return;



        earlyWeights = new[]

        {

            new PlatformTypeWeight { type = PlatformType.Safe, weight = 0.7f },

            new PlatformTypeWeight { type = PlatformType.Spring, weight = 0.1f },

            new PlatformTypeWeight { type = PlatformType.Fragile, weight = 0.08f },

            new PlatformTypeWeight { type = PlatformType.SizeShift, weight = 0.06f },

            new PlatformTypeWeight { type = PlatformType.Moving, weight = 0.06f }

        };

        lateWeights = new[]

        {

            new PlatformTypeWeight { type = PlatformType.Safe, weight = 0.35f },

            new PlatformTypeWeight { type = PlatformType.Spring, weight = 0.15f },

            new PlatformTypeWeight { type = PlatformType.Fragile, weight = 0.2f },

            new PlatformTypeWeight { type = PlatformType.SizeShift, weight = 0.15f },

            new PlatformTypeWeight { type = PlatformType.Moving, weight = 0.15f }

        };

    }

}

