using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeRadius : MonoBehaviour
{
    private UnityEngine.Experimental.VFX.VisualEffect vfx;
    private Transform homeTransform;

    void Start()
    {
        homeTransform = FindObjectOfType<HomeController>().transform;
        vfx = GetComponent<UnityEngine.Experimental.VFX.VisualEffect>();
        vfx.SetFloat("Radius", 3.1f);

    }

    // Update is called once per frame
    void Update()
    {
        vfx.SetVector3("Center", homeTransform.position);
    }
}
