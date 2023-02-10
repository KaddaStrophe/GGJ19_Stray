using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour {

    PlayerInputActions playerInput;
    PlayerController playerController;

    protected void OnEnable() {
        if (!playerController) {
            TryGetComponent(out playerController);
        }
        playerInput = new PlayerInputActions();
        playerInput.Enable();
        playerInput.Player.Jump.performed += PerformJump;
        playerInput.Player.Interact.performed += PerformInteract;
        playerInput.Player.Move.performed += PerformMove;
    }
    protected void OnDisable() {
        playerInput.Player.Jump.performed -= PerformJump;
        playerInput.Player.Interact.performed -= PerformInteract;
        playerInput.Player.Move.performed -= PerformMove;
        playerInput.Disable();
    }

    void PerformMove(InputAction.CallbackContext context) {
        float movement = context.ReadValue<Vector2>().x;
        playerController.ChangeMovementVector(movement);
    }

    void PerformInteract(InputAction.CallbackContext context) {
        playerController.PerformInteract();
    }

    void PerformJump(InputAction.CallbackContext context) {
        playerController.PerformJump();
    }
}
