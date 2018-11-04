using UnityEngine;
using System.Collections;

public class WeaponCollisionTrigger : MonoBehaviour
{
    private GameObject avatar;
    private Animator animChar;
    private CombatEvents combatEvents;

	// Use this for initialization
	void Start () 
    {

	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    void UpdateAnim()
    {
        avatar = transform.root.gameObject;
        animChar = avatar.GetComponent<Animator>();
        combatEvents = avatar.GetComponent<CombatEvents>();
    }

    void OnTriggerEnter(Collider other)
    {
        // avatar is attacking
        if (animChar && animChar.GetInteger("attack") != -1)
        {
            // in no trigger
            if (!other.isTrigger)
            {
                GameObject hitObject = other.transform.root.gameObject;

                // dont damage yourself
                if (hitObject != avatar)
                {
                    Debug.Log(avatar.name + " hit " + hitObject.name);
                    combatEvents.Attack(hitObject);
                }
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("CollisionEnter");
    }
}
