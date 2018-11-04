using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class NPCAttributes : MonoBehaviour
{
    [System.Serializable]
    public struct Sighting
    {
        public Vector3 position;
        public Vector3 direction;

        public void SetSighting(Vector3 position, Vector3 direction)
        {
            this.position = position;
            this.direction = direction;
        }
    }

    // [SyncVar(hook = "AggroSync")]
    public float aggro = 0;
    public float aggressive = 1;
    public float sightRadius = 50f;
    public float fieldOfViewAngle = 150f;           // Number of degrees, centred on forward, for the enemy see.

    // first met relation will be set
    // after that relation changes with actions, dialogues but less much
    // relation changes: costs(trader), dialoge options, help in fight
    public int[] personalRelations;


    public Sighting personalLastSighting;
    public Sighting globalLastSighting;
    public Vector3 resetPosition = new Vector3(10000f, 10000f, 10000f);
    public bool targetInSight;
    public bool targetPositionLost;
    public GroupStatus groupStatus;

    // Use this for initialization
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    /*
    [Client]
    void AggroSync(float aggro)
    {
        this.aggro = aggro;
    }*/
}
