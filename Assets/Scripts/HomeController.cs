using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeController : MonoBehaviour
{
    [HideInInspector]
    public bool isGettingCarried;

    private Rigidbody2D _rigidbody2d;
    private BoxCollider2D _boxCollider2d;
    private bool _reachedDestination;
    private bool _carryFlag;

    private void Awake()
    {
        _rigidbody2d = GetComponent<Rigidbody2D>();
        _boxCollider2d = GetComponent<BoxCollider2D>();
       
    }

    private void Update()
    {
        if (isGettingCarried && !_reachedDestination)
        {
            //this.gameObject.transform.position = PlayerController.instance.carryPosition.position;
            this.transform.position = Vector3.MoveTowards(this.transform.position, PlayerController.instance.carryPosition.position, 2.5f * Time.deltaTime);
        }
        if (this.transform.position == PlayerController.instance.carryPosition.position)
        {
            _reachedDestination = true;
            PlayerController.instance.canMove = true;
            PlayerController.instance.isCarryingHome = true;
        }
        if (_reachedDestination)
        {
            this.gameObject.transform.position = PlayerController.instance.carryPosition.position;
        }
    }

    public void activateCarryMode()
    {
        if (!_carryFlag)
        {
            _carryFlag = true;
            StartCoroutine(WaitForPickup());
            _rigidbody2d.isKinematic = true;
            _boxCollider2d.enabled = false;
        }
    }

    public void deactivateCarryMode()
    {
        if (_carryFlag)
        {
            _carryFlag = false;
            _rigidbody2d.isKinematic = false;
            isGettingCarried = false;
            _boxCollider2d.enabled = true;
            _reachedDestination = false;
            StartCoroutine(WaitForDrop());
        }
    }

    public IEnumerator WaitForDrop()
    {
        yield return new WaitForSeconds(0.7f);
        PlayerController.instance.canMove = true;
        PlayerController.instance.isCarryingHome = false;
    }

    public IEnumerator WaitForPickup()
    {
        yield return new WaitForSeconds(0.25f);
        isGettingCarried = true;
    }
}
