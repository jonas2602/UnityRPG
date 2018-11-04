using UnityEngine;
using System.Collections;

public abstract class MunitionController : MonoBehaviour {

    protected Rigidbody rb;
    protected Collider col;
    protected Camera cam;
    protected Vector3 forceDirection = Vector3.zero;

    public void SetForceDirection(Vector3 forceDirection)
    {
        this.forceDirection = forceDirection.normalized;
    }


    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        cam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
    }

    public abstract void Shoot();
}
