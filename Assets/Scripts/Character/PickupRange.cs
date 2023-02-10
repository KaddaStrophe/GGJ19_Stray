using UnityEngine;

public class PickupRange : MonoBehaviour {
    [SerializeField]
    PlayerController playerController;

    protected void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Home")) {
            playerController.pickupInRange = true;
        }
        if (other.gameObject.CompareTag("PushBlock")) {
            playerController.currentPushBlock = other.gameObject;
            playerController.isStandingNearBlock = true;
        }
    }

    protected void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.CompareTag("Home")) {
            playerController.pickupInRange = false;
        }
        if (other.gameObject.CompareTag("PushBlock")) {
            playerController.currentPushBlock = null;
            playerController.isStandingNearBlock = false;
        }
    }
}
