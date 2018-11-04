using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;

[NetworkSettings(channel = 0, sendInterval = 0.1f)]
public class Player_SyncPos : NetworkBehaviour
{

    [SyncVar(hook = "SyncPositionValues")] // send to clients when changed
    private Vector3 syncPlayerPos;
    [SyncVar]
    private Vector3 syncCamPos;

    public Transform camTransform;
    public float lerpRate;
    public float normalLerpRate = 16;
    public float fasterLerpRate = 27;

    [SerializeField]
    private Vector3 lastPlayerPos;
    [SerializeField]
    private Vector3 lastCamPos;
    public float threshold = 0.5f;

    // private NetworkClient nClient;
    // private int latency;
    // private Text latencyText;

    private List<Vector3> syncPosList = new List<Vector3>();
    [SerializeField]
    private bool useHistoricalLerping = false;
    private float closeEnough = 0.11f;

    void Awake()
    {
        // nClient = GameObject.Find("NetworkManager").GetComponent<NetworkManager>().client;
        // latencyText = GameObject.Find("Latency Text").GetComponent<Text>();
        camTransform = transform.Find("Camera");
    }
    
    // Use this for initialization
    void Start()
    {
        lerpRate = normalLerpRate;
    }
    
    // Update is called once per frame
    void Update()
    {
        // acts for the other player
        LerpPosition();
        // ShowLatency();
    }
    

    void FixedUpdate()
    {
        // acts for local player
        TransmitPosition();
    }


    // lerps the position of the other players (not local)
    void LerpPosition()
    {
        if (!isLocalPlayer)
        {
            if (useHistoricalLerping)
            {
                HistoricalLerping();
            }
            else
            {
                OrdinaryLerping();
            }

            //Debug.Log(Time.deltaTime.ToString());
            
            camTransform.position = Vector3.Lerp(camTransform.position, syncCamPos, Time.deltaTime * lerpRate);
        }
    }


    // Send position to server (acts)
    [Command]
    void CmdProvidePositionToServer(Vector3 playerPos, Vector3 camPos)
    {
        syncPlayerPos = playerPos;
        syncCamPos = camPos;
    }


    // starts the transmit to server (calls)
    [ClientCallback]
    void TransmitPosition()
    {
        if (isLocalPlayer)
        {
            if (Vector3.Distance(lastPlayerPos, transform.position) > threshold || Vector3.Distance(lastCamPos, camTransform.position) > threshold)
            {
                CmdProvidePositionToServer(transform.position, camTransform.position);
                lastPlayerPos = transform.position;
                lastCamPos = camTransform.position;
            }
        }
    }


    [Client]
    void SyncPositionValues(Vector3 latestPlayerPos)
    {
        syncPlayerPos = latestPlayerPos;
        syncPosList.Add(syncPlayerPos);
    }


    /*
    void ShowLatency()
    {
        if (isLocalPlayer)
        {
            latency = nClient.GetRTT();
            latencyText.text = latency.ToString();
        }
    }
    */


    void OrdinaryLerping()
    {
        transform.position = Vector3.Lerp(transform.position, syncPlayerPos, Time.deltaTime * lerpRate);
    }


    void HistoricalLerping()
    {
        if (syncPosList.Count > 0)
        {
            transform.position = Vector3.Lerp(transform.position, syncPosList[0], Time.deltaTime * lerpRate);

            if (Vector3.Distance(transform.position, syncPosList[0]) < closeEnough)
            {
                syncPosList.RemoveAt(0);
            }

            if (syncPosList.Count > 10)
            {
                lerpRate = fasterLerpRate;
            }
            else
            {
                lerpRate = normalLerpRate;
            }

            //Debug.Log(syncPosList.Count.ToString());
        }
    }
}
