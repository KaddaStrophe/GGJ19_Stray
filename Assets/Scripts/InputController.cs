using UnityEngine;
using UnityEngine.SceneManagement;

public class InputController : MonoBehaviour {

    PlayerController _playerController;

    bool _jump;
    bool _shoot;
    bool _interact;

    // Use this for initialization
    protected void Start() {
        _playerController = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    protected void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            SceneManager.LoadScene("Title");
        }

        if (!_jump) {
            _jump = Input.GetButtonDown("Jump");
        }
        if (!_shoot) {
            _shoot = Input.GetButtonDown("Fire1");
        }
        if (!_interact) {
            _interact = Input.GetButtonDown("Interact");
        }
    }

    protected void FixedUpdate() {
        _playerController.HandleButtonInputs(_jump, _shoot, _interact);

        _jump = false;
        _shoot = false;
        _interact = false;
    }
}
