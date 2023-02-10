using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public float smoothSpeed = 5f;
    public float cameraYOffset = 1.5f;
    private GameObject _player;

    private Vector3 _specificVector;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void LateUpdate()
    {
        _specificVector = new Vector3(_player.transform.position.x, _player.transform.position.y + cameraYOffset, this.transform.position.z);
        this.transform.position = Vector3.Lerp(this.transform.position, _specificVector, smoothSpeed * Time.deltaTime);
    }
}