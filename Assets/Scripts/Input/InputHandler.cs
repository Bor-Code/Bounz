using UnityEngine;
using UnityEngine.Events;

public class InputHandler : MonoBehaviour
{
    public UnityAction onPressStarted;
    public UnityAction onPressEnded;

    private bool _isPressed = false;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            HandlePressStart();
        else if (Input.GetMouseButtonUp(0))
            HandlePressEnd();

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began && !_isPressed)
                HandlePressStart();
            else if ((touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) && _isPressed)
                HandlePressEnd();
        }
    }

    private void HandlePressStart()
    {
        _isPressed = true;
        onPressStarted?.Invoke();
    }

    private void HandlePressEnd()
    {
        _isPressed = false;
        onPressEnded?.Invoke();
    }

    public bool IsPressed => _isPressed;
}
