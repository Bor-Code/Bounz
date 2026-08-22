using UnityEngine;
public class FallDetector : MonoBehaviour
{
    [Tooltip("Kameranın alt kenarından ne kadar aşağıya düşünce ölsün (Unity birim)")]
    [SerializeField] private float fallThreshold = 3f;
    private Camera _cam;
    private void Awake()
    {
        _cam = Camera.main;
    }
    private void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.State != GameManager.GameState.Playing)
            return;
        if (_cam == null) return;
        float bottomEdge = _cam.ViewportToWorldPoint(new Vector3(0f, 0f, 0f)).y;
        if (transform.position.y < bottomEdge - fallThreshold)
        {
            GameManager.Instance.TriggerGameOver();
        }
    }
}