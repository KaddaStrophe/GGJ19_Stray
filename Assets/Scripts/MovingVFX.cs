using UnityEngine;
using UnityEngine.VFX;

public class MovingVFX : MonoBehaviour {
    public VisualEffect visualEffect;
    Transform target;

    protected void Start() {
        target = FindObjectOfType<HomeController>().transform;
    }

    protected void Update() {
        visualEffect.SetVector3("TargetPosition", target.position);
        visualEffect.SetBool("InHomeRadius", Vector2.Distance(transform.position, target.position) < 3f);
    }
}
