using UnityEngine;

public enum PlatformType
{
    Safe,
    Spring,
    Fragile,
    SizeShift
}

[System.Serializable]
public struct PlatformTypeWeight
{
    public PlatformType type;
    [Range(0f, 1f)] public float weight;
}
