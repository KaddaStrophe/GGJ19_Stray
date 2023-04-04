using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonBehaviour : MonoBehaviour
{
    [SerializeField]
    private bool _pressed;

    public Transform connectedWall;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            _pressed = !_pressed;

            //Insert Button behaviour with connectedWall here! 
            //(Spritechange,...)


            if (_pressed)
            {
                GetComponent<SpriteRenderer>().color = Color.red;
                connectedWall.gameObject.SetActive(false);
            }
            else
            {
                GetComponent<SpriteRenderer>().color = Color.white;
                connectedWall.gameObject.SetActive(true);
            }
        }
    }

  
}
