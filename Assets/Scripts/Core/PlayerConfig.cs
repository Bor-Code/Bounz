using UnityEngine;

[CreateAssetMenu(fileName = "PlayerConfig", menuName = "Bounz/PlayerConfig")]
public class PlayerConfig : ScriptableObject
{
    [Header("Zıplama Kuvveti")]
    public float minJumpForce = 5f;
    public float maxJumpForce = 14f;
    public float chargeTime = 0.5f;

    [Header("Fizik")]
    public float gravityMultiplier = 2.5f;
    public float moveSpeed = 5f;

    [Header("Boyut")]
    public float defaultScale = 1f;
    public float minScale = 0.5f;
    public float maxScale = 2f;
}
