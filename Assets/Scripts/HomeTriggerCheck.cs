using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeTriggerCheck : MonoBehaviour
{
    public Sprite sp_BlockingWallSprite;
    public Sprite sp_PassWallSprite;

    private void OnTriggerEnter2D(Collider2D collision)
    {


        if (collision.gameObject.tag == "BlockingWall")
        {
          
                collision.gameObject.transform.parent.GetComponent<SpriteRenderer>().enabled = true;
          
            
            collision.gameObject.transform.parent.GetComponent<Collider2D>().enabled = true;

            //collision.gameObject.transform.parent.GetComponent<SpriteRenderer>().sprite = sp_BlockingWallSprite;
        }

        if (collision.gameObject.tag == "Button")
        {

            collision.gameObject.transform.parent.GetComponent<BoxCollider2D>().enabled = false;
        }

        if (collision.gameObject.tag == "Platform")
        {
            
            collision.gameObject.transform.parent.GetComponent<BoxCollider2D>().enabled = true;

        }

        if (collision.gameObject.tag == "Player")
        {
            PlayerController.instance.leftTheCircle = false;
            PlayerController.instance.SetOutOfHomeState(0);
            PlayerController.instance.SetNormalModeTrigger();
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {


        if (collision.gameObject.tag == "BlockingWall")
        {
            
            //collision.gameObject.transform.parent.GetComponent<SpriteRenderer>().sprite = sp_PassWallSprite;

            collision.gameObject.transform.parent.GetComponent<SpriteRenderer>().enabled = false;
            collision.gameObject.transform.parent.GetComponent<Collider2D>().enabled = false;
          
        }

        if (collision.gameObject.tag == "Button")
        {

            collision.gameObject.transform.parent.GetComponent<BoxCollider2D>().enabled = true;
            Debug.Log(collision.gameObject.transform.GetComponentInParent<BoxCollider2D>().gameObject.name);
        }
        if (collision.gameObject.tag == "Platform")
        {

            collision.gameObject.transform.parent.GetComponent<BoxCollider2D>().enabled = false;

        }

        if (collision.gameObject.tag == "Player")
        {
            PlayerController.instance.leftTheCircle = true;
            PlayerController.instance.outOfCircleStartFrame = Time.frameCount;
        }
    }

}

