using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TitleInputHandler : MonoBehaviour {
    PlayerInputActions playerInput;

    protected void OnEnable() {
        playerInput = new PlayerInputActions();
        playerInput.Enable();
        playerInput.UI.Submit.performed += StartGameScene;
    }

    void StartGameScene(InputAction.CallbackContext context) {
        SceneManager.LoadScene("Level1");
    }
}
