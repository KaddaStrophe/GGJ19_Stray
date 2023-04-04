using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuInput : MonoBehaviour {
    protected void Update() {
        if (Input.anyKey) {
            SceneManager.LoadScene("Level1");
        }
    }
}
