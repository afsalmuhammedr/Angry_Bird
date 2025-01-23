using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static PlayerInput playerInput;

    private InputAction mousePointer;
    private InputAction mouse;

    public static Vector2 mousePosition;

    public static bool wasLeftButtonPressed;
    public static bool isLeftButtonPressed;
    public static bool wasLeftButtonReleased;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        mousePointer = playerInput.actions["MousePosition"];
        mouse = playerInput.actions["Mouse"];
    }
    private void Update()
    {
        mousePosition = mousePointer.ReadValue<Vector2>();

        wasLeftButtonPressed = mouse.WasPressedThisFrame();
        isLeftButtonPressed = mouse.IsPressed();
        wasLeftButtonReleased = mouse.WasReleasedThisFrame();
    }
}
