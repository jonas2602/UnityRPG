using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Used : MonoBehaviour {

    private Animator anim;
    private ParticleSystem particleSystem;
    private bool isUsed = false;

    List<GameObject> userQueue = new List<GameObject>();

    void Awake()
    {
        anim = GetComponent<Animator>();
        particleSystem = GetComponentInChildren<ParticleSystem>();
    }

    public bool StartInteracting(/*GameObject npc*/)
    {
        if (isUsed == false)
        {
            // start interacting
            isUsed = true;
            if (anim)
            {
                anim.SetBool("used", true);
            }
            if(particleSystem)
            {
                particleSystem.Play();
            }
            return true;
        }
        else
        {
            // wait
            // userQueue.Add(npc);
            return false;
        }
    }

    public void StopInteracting()
    {/*
        if(userQueue.Count > 0)
        {
            // next npc start interacting
        }*/
        
        isUsed = false;
        if (anim)
        {
            anim.SetBool("used", false);
        }
        if (particleSystem)
        {
            particleSystem.Stop();
        }

    }
}
