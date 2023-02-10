using UnityEngine;
using UnityEngine.VFX;

public class HomeRadius : MonoBehaviour {
    VisualEffect vfx;
    Transform homeTransform;

    protected void Start() {
        homeTransform = FindObjectOfType<HomeController>().transform;
        vfx = GetComponent<VisualEffect>();
        vfx.SetFloat("Radius", 3.1f);

    }

    // Update is called once per frame
    protected void Update() {
        vfx.SetVector3("Center", homeTransform.position);
    }
}
