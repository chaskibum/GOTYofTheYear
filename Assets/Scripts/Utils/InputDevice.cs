using UnityEngine;
using UnityEngine.InputSystem;

public class InputDevice : MonoBehaviour
{
    public bool UsingController { get; private set; }
    public event System.Action<bool> OnInputDeviceChanged;

    private void OnEnable()
    {
        InputSystem.onActionChange += OnActionChange;
    }

    private void OnDisable()
    {
        InputSystem.onActionChange -= OnActionChange;
    }

    private void OnActionChange(object obj, InputActionChange change)
    {
        if (change != InputActionChange.ActionPerformed)
            return;

        InputAction action = obj as InputAction;
        OnInputDeviceChanged?.Invoke(UsingController);

        if (action == null || action.activeControl == null)
            return;

        UsingController = action.activeControl.device is Gamepad;
    }
}
