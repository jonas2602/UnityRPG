using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NPC_SyncMotion : NetworkBehaviour {

    [SyncVar]
    private Vector3 syncPos;
    [SyncVar]
    private float syncYRot;

    private Quaternion lastRot;
    private Vector3 lastPos;
    public float lerpRate = 10;
    public float posThreshold = 0.5f;
    public float rotThreshold = 5;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        TransmitMotion();
        LerpMotion();
	}

    void TransmitMotion()
    {
        if (!isServer)
        {
            return;
        }

        if (Vector3.Distance(transform.position, lastPos) > posThreshold || Quaternion.Angle(transform.rotation, lastRot) > rotThreshold)
        {
            lastPos = transform.position;
            lastRot = transform.rotation;

            syncPos = transform.position;
            syncYRot = transform.localEulerAngles.y;
        }
    }

    void LerpMotion()
    {
        if (isServer)
        {
            return;
        }

        transform.position = Vector3.Lerp(transform.position, syncPos, Time.deltaTime * lerpRate);

        Vector3 newRot = new Vector3(0, syncYRot, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(newRot), Time.deltaTime * lerpRate);
    }
}
