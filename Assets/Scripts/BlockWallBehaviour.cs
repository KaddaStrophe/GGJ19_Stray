using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockWallBehaviour : MonoBehaviour
{
    private void Awake()
    {
       GetComponent<SpriteRenderer>().enabled = false;
       GetComponent<Collider2D>().enabled = false;

    }
}
