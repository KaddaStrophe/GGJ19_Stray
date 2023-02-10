using System.Collections;
using UnityEngine;

public class HomeController : MonoBehaviour {
    [SerializeField]
    PlayerController playerController = default; 
    [HideInInspector]
    public bool isGettingCarried;

    Rigidbody2D rigidbody2d;
    BoxCollider2D boxCollider2d;
    bool reachedDestination;
    bool carryFlag;

    protected void Awake() {
        rigidbody2d = GetComponent<Rigidbody2D>();
        boxCollider2d = GetComponent<BoxCollider2D>();

    }
    protected void OnEnable() {
        if(!playerController) {
            playerController = FindObjectOfType<PlayerController>();
        }
    }

    protected void Update() {
        if (isGettingCarried && !reachedDestination) {
            //this.gameObject.transform.position = PlayerController.instance.carryPosition.position;
            transform.position = Vector3.MoveTowards(transform.position, playerController.carryPosition.position, 2.5f * Time.deltaTime);
        }
        if (transform.position == playerController.carryPosition.position) {
            reachedDestination = true;
            playerController.canMove = true;
            playerController.isCarryingHome = true;
        }
        if (reachedDestination) {
            gameObject.transform.position = playerController.carryPosition.position;
        }
    }

    public void ActivateCarryMode() {
        if (!carryFlag) {
            carryFlag = true;
            StartCoroutine(WaitForPickup());
            rigidbody2d.isKinematic = true;
            boxCollider2d.enabled = false;
        }
    }

    public void DeactivateCarryMode() {
        if (carryFlag) {
            Debug.Log("Should stop carrying");
            carryFlag = false;
            rigidbody2d.isKinematic = false;
            isGettingCarried = false;
            boxCollider2d.enabled = true;
            reachedDestination = false;
            StartCoroutine(WaitForDrop());
        }
    }

    public IEnumerator WaitForDrop() {
        yield return new WaitForSeconds(0.7f);
        playerController.canMove = true;
        playerController.isCarryingHome = false;
    }

    public IEnumerator WaitForPickup() {
        yield return new WaitForSeconds(0.25f);
        isGettingCarried = true;
    }
}
