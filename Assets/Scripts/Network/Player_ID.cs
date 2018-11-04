using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Player_ID : NetworkBehaviour
{

    [SyncVar]
    public string playerUniqueName;
    public NetworkInstanceId playerNetId;

    public override void OnStartLocalPlayer()
    {
        GetNetIdentity();
        SetIdentity();
    }


    // Use this for initialization
    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (this.name == "" || this.name == "Character(Clone)")
        {
            SetIdentity();
        }
    }

    void GetNetIdentity()
    {
        playerNetId = GetComponent<NetworkIdentity>().netId;
        CmdTellServerMyIdentity(MakeUniqueIdentity());
    }

    void SetIdentity()
    {
        if (!isLocalPlayer)
        {
            this.name = playerUniqueName;
        }
        else
        {
            this.name = MakeUniqueIdentity();
        }
    }

    string MakeUniqueIdentity()
    {
        string uniqueName = "Player" + playerNetId.ToString();

        return uniqueName;
    }

    [Command]
    void CmdTellServerMyIdentity(string name)
    {
        playerUniqueName = name;
    }
}
