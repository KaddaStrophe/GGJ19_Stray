using UnityEngine;
using UnityEngine.Experimental.VFX;
using UnityEngine.VFX.Utils;

public class MovingVFX : MonoBehaviour
{
    public VisualEffect visualEffect;
    private Transform target;

    void Start()
    {
        target = FindObjectOfType<HomeController>().transform;
    }

    void Update()
    {
        visualEffect.SetVector3("TargetPosition", target.position);
        visualEffect.SetBool("InHomeRadius", Vector2.Distance(transform.position, target.position) < 3f);
    }
}
