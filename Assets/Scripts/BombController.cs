using UnityEngine;
using System.Collections;
using System;

public class BombController : MunitionController
{
    private ParticleSystem partSystem;
    
    protected override void Awake()
    {
        base.Awake();
        partSystem = GetComponentInChildren<ParticleSystem>();
    }

    public override void Shoot()
    {
        rb.isKinematic = false;
        col.isTrigger = false;

        this.transform.parent = null;
        Vector3 force = cam.transform.forward;
        rb.AddForce(force, ForceMode.Impulse);
    }

    void OnCollisionEnter(Collision collision)
    {
        // has particle system?
        if (partSystem)
        {
            Explode();
        }
    }

    void Explode()
    {
        partSystem.Play();
        rb.isKinematic = true;
        GetComponent<MeshRenderer>().enabled = false;
        Destroy(gameObject, partSystem.startLifetime);
    }
}
