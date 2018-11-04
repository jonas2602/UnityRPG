using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class Player_SyncRot : NetworkBehaviour
{

    [SyncVar(hook = "OnPlayerRotSynced")]
    private float syncPlayerRotation;
    [SyncVar(hook = "OnCamRotSynced")]
    private Quaternion syncCamRotation;

    [SerializeField]
    private Transform playerTransform;
    [SerializeField]
    private Transform camTransform;
    private float lerpRate = 20;

    [SerializeField]
    private float lastPlayerRot;
    [SerializeField]
    private Quaternion lastCamRot;
    public float threshold = 1;

    private List<float> syncPlayerRotList = new List<float>();
    private List<Quaternion> syncCamRotList = new List<Quaternion>();
    private float closeEnough = 0.4f;
    [SerializeField]
    private bool useHistoricalInterpolation;


    // Use this for initialization
    void Awake()
    {
        camTransform = transform.Find("Camera");
        playerTransform = this.transform;
    }

    // Update is called once per frame
    void Update()
    {
        LerpRotation();
    }


    void FixedUpdate()
    {
        TransmitRotations();
    }


    void LerpRotation()
    {
        if (!isLocalPlayer)
        {
            if (useHistoricalInterpolation)
            {
                HistoricalInterpolation();
            }
            else
            {
                OrdinaryLerping();
            }
        }
    }


    void HistoricalInterpolation()
    {
        if (syncPlayerRotList.Count > 0)
        {
            LerpPlayerRotation(syncPlayerRotList[0]);

            if (Mathf.Abs(playerTransform.localEulerAngles.y - syncPlayerRotList[0]) < closeEnough)
            {
                syncPlayerRotList.RemoveAt(0);
            }

            //Debug.Log(syncPlayerRotList.Count.ToString() + " syncPlayerRotList Count");
        }

        if (syncCamRotList.Count > 0)
        {
            LerpCamRot(syncCamRotList[0]);

            if (Quaternion.Angle(camTransform.rotation, syncCamRotList[0]) < closeEnough)
            {
                syncCamRotList.RemoveAt(0);
            }

            //Debug.Log(syncCamRotList.Count.ToString() + " syncCamRotList Count");
        }
    }


    void OrdinaryLerping()
    {
        LerpPlayerRotation(syncPlayerRotation);
        LerpCamRot(syncCamRotation);
    }


    void LerpPlayerRotation(float rotAngle)
    {
        Vector3 playerNewRot = new Vector3(0, rotAngle, 0);
        playerTransform.rotation = Quaternion.Lerp(playerTransform.rotation, Quaternion.Euler(playerNewRot), lerpRate * Time.deltaTime);
    }


    void LerpCamRot(Quaternion newCamRot)
    {
        camTransform.localRotation = Quaternion.Lerp(camTransform.localRotation, newCamRot, lerpRate * Time.deltaTime);
    }

    [Command]
    void CmdProvideRotationsToServer(float playerRot, Quaternion camRot)
    {
        syncPlayerRotation = playerRot;
        syncCamRotation = camRot;
    }

    [Client]
    void TransmitRotations()
    {
        if (isLocalPlayer)
        {
            if (CheckIfBeyondThreshold(playerTransform.localEulerAngles.y, lastPlayerRot) || CheckIfBeyondThreshold(camTransform.rotation, lastCamRot))
            {
                lastPlayerRot = playerTransform.localEulerAngles.y;
                lastCamRot = camTransform.rotation;
                CmdProvideRotationsToServer(lastPlayerRot, lastCamRot);
            }
        }
    }


    bool CheckIfBeyondThreshold(float rot1, float rot2)
    {
        if (Mathf.Abs(rot1 - rot2) > threshold)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    bool CheckIfBeyondThreshold(Quaternion rot1, Quaternion rot2)
    {
        if (Quaternion.Angle(rot1, rot2) > threshold)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
    [Client]
    void OnPlayerRotSynced(float latestPlayerRot)
    {
        syncPlayerRotation = latestPlayerRot;
        syncPlayerRotList.Add(syncPlayerRotation);
    }

    [Client]
    void OnCamRotSynced(Quaternion latestCamRot)
    {
        syncCamRotation = latestCamRot;
        syncCamRotList.Add(syncCamRotation);
    }
}
