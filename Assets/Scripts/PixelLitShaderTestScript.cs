using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelLitShaderTestScript : MonoBehaviour
{
    Material mat;
    public Transform centerTransform;
    public Material material;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        material.SetVector("_Center", centerTransform.position);
    }
}
