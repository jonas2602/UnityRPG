using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnvironmentScanner : MonoBehaviour
{
    private Transform avatar;
    private Relations relations;

    private int creatureLayer;
    private int interactableLayer;
    
    public List<Transform> enemys = new List<Transform>();

    public List<Transform> allies = new List<Transform>();
    public List<AllyEnteredInfo> allyInterested = new List<AllyEnteredInfo>();

    public List<Transform> interactables = new List<Transform>();

    public void GetAlliesInfo(AllyEnteredInfo script)
    {
        allyInterested.Add(script);
    }

    public void LeaveAlliesInfo(AllyEnteredInfo scipt)
    {
        allyInterested.Remove(scipt);
    }

    void Awake()
    {
        avatar = transform.root;
        relations = GameObject.FindWithTag("Relations").GetComponent<Relations>();

        creatureLayer = LayerMask.NameToLayer("Creature");
        interactableLayer = LayerMask.NameToLayer("Interactable");
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {

	}

    void OnTriggerEnter(Collider other)
    {
        // Debug.Log(other.name + " entered " + avatar.name + " trigger");

        Transform curTransform = other.transform.root;

        // ignore myself
        if (curTransform == avatar)
        {
            return;
        }
        
        // check for interactable
        if (curTransform.gameObject.layer == interactableLayer)
        {
            // Debug.Log("is interactable");
            if (!interactables.Contains(curTransform))
            {
                interactables.Add(curTransform);
            }
        }

        // check for creatures
        if (curTransform.gameObject.layer == creatureLayer)
        {
            // is alive?
            if (curTransform.GetComponent<Health>().condition == Health.Condition.Healthy)
            {
                // friend ...
                Relation relation = relations.GetFractionRelation(avatar.gameObject, curTransform.gameObject);
                if (!(relation == Relation.Enemy))
                {
                    if (!allies.Contains(curTransform))
                    {
                        allies.Add(curTransform);

                        // inform interested scripts
                        for(int i = 0; i < allyInterested.Count;i++)
                        {
                            allyInterested[i].AllyEntered(curTransform, relation);
                        }
                    }
                }
                // ... or foe
                else
                {
                    if (!enemys.Contains(curTransform))
                    {
                        enemys.Add(curTransform);
                    }
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Debug.Log(other.name + " left " + avatar.name + " trigger");

        GameObject curObject = other.gameObject;


        if (curObject.layer == interactableLayer)
        {
            interactables.RemoveAll(interactable => interactable == curObject.transform);
        }
        else
        {
            if(relations.GetFractionRelation(avatar.gameObject, curObject) == Relation.Enemy)
            {
                enemys.RemoveAll(enemy => enemy == curObject.transform);
            }
            else
            {
                allies.RemoveAll(ally => ally == curObject.transform);
            }
        }
    }
}
