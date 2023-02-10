using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.VFX;

public class CheckPoint : MonoBehaviour
{
    private bool _lerpActive;
    private GameObject player;
    [SerializeField]
    private float _lerpTime;
    Transform home;
    private GameObject checkpointVFX;
    bool _VFXActivate;


    private void Awake()
    {
        _VFXActivate = false;
        checkpointVFX = transform.GetChild(1).gameObject;
        home = GameObject.Find("Home").transform;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            player = collision.gameObject;
            _lerpActive = true;
            _VFXActivate = true;
            PlayerController.instance.currentCheckpoint = this.gameObject.transform;
        }
    }
    private void Update()
    {
        if (_VFXActivate)
            checkpointVFX.GetComponent<VisualEffect>().enabled = true;

        if(_lerpActive)
            LerpHome(_lerpTime);
    }
    private void LerpHome(float _lerpTime)
    {

        _lerpActive = false;
        float lerpTime = _lerpTime;
        float currentLerpTime = 0f;
        currentLerpTime += Time.deltaTime;
        if (currentLerpTime > lerpTime)
        {
            currentLerpTime = lerpTime;
        }
      
        float perc = currentLerpTime / lerpTime;
        //home.position = Vector3.Lerp(home.position, player.transform.position, perc);
        home.position = Vector3.Lerp(home.position, transform.position, Time.time * 5);

    }
}
