using UnityEngine;
using UnityEngine.SceneManagement;

public class GameJamInputController : MonoBehaviour {
    protected void Awake() {
        DontDestroyOnLoad(this);
    }
    protected void Update() {
        if (Input.GetKeyDown(KeyCode.F1)) {
            SceneManager.LoadScene("Title");

        }

        if (Input.GetKeyDown(KeyCode.F2)) {
            SceneManager.LoadScene("Level1");
        }

        if (Input.GetKeyDown(KeyCode.F3)) {
            SceneManager.LoadScene("Level2");

        }
    }
}
