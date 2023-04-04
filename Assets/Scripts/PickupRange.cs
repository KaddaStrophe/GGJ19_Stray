using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupRange : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Home")
        {
            PlayerController.instance.pickupInRange = true;
        }
        if (other.gameObject.tag == "PushBlock")
        {
            PlayerController.instance.currentPushBlock = other.gameObject;
            PlayerController.instance.isStandingNearBlock = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Home")
        {
            PlayerController.instance.pickupInRange = false;
        }
        if (other.gameObject.tag == "PushBlock")
        {
            PlayerController.instance.currentPushBlock = null;
            PlayerController.instance.isStandingNearBlock = false;
        }
    }
}
