using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{

    private PlayerController _playerController;

    private bool _jump;
    private bool _shoot;
    private bool _interact;

    // Use this for initialization
    void Start()
    {
        _playerController = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_jump)
        {
            _jump = Input.GetButtonDown("Jump");
        }
        if (!_shoot)
        {
            _shoot = Input.GetButtonDown("Fire1");
        }
        if(!_interact)
        {
            _interact = Input.GetButtonDown("Interact");
        }
    }

    void FixedUpdate()
    {
        _playerController.HandleButtonInputs(_jump, _shoot, _interact);

        _jump = false;
        _shoot = false;
        _interact = false;
    }
}
