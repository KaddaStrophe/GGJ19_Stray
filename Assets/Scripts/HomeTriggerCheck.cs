using UnityEngine;

public class HomeTriggerCheck : MonoBehaviour {

    protected void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("BlockingWall")) {
            collision.gameObject.transform.parent.GetComponent<SpriteRenderer>().enabled = true;
            collision.gameObject.transform.parent.GetComponent<Collider2D>().enabled = true;
        }

        if (collision.gameObject.CompareTag("Button")) {
            collision.gameObject.transform.parent.GetComponent<BoxCollider2D>().enabled = false;
        }

        if (collision.gameObject.CompareTag("Platform")) {
            collision.gameObject.transform.parent.GetComponent<BoxCollider2D>().enabled = true;
        }

        if (collision.gameObject.CompareTag("Player")) {
            PlayerController.instance.leftTheCircle = false;
            PlayerController.instance.SetOutOfHomeState(0);
            PlayerController.instance.SetNormalModeTrigger();
        }
    }

    protected void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("BlockingWall")) {
            collision.gameObject.transform.parent.GetComponent<SpriteRenderer>().enabled = false;
            collision.gameObject.transform.parent.GetComponent<Collider2D>().enabled = false;
        }

        if (collision.gameObject.CompareTag("Button")) {

            collision.gameObject.transform.parent.GetComponent<BoxCollider2D>().enabled = true;
            Debug.Log(collision.gameObject.transform.GetComponentInParent<BoxCollider2D>().gameObject.name);
        }
        if (collision.gameObject.CompareTag("Platform")) {
            collision.gameObject.transform.parent.GetComponent<BoxCollider2D>().enabled = false;
        }

        if (collision.gameObject.CompareTag("Player")) {
            PlayerController.instance.leftTheCircle = true;
            PlayerController.instance.outOfCircleStartFrame = Time.frameCount;
        }
    }
}

