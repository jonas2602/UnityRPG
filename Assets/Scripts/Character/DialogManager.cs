using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class DialogManager : NetworkBehaviour
{
    [SyncVar(hook = "OnPartnerChanged")]
    public GameObject dialogPartner;
    public StoryManager storyManager;
    

    void Awake()
    {
        storyManager = GameObject.FindWithTag("GameManager").GetComponent<StoryManager>();
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public bool IsInDialog()
    {
        return false;
    }

    // called to address sb
    public void Address(GameObject target)
    {
        // Debug.Log("start dialog with: " + target.name);
        DialogManager targetDialogManager = target.GetComponent<DialogManager>();

        // try to start dialog with target
        if (targetDialogManager.Receive(this.gameObject))
        {
            dialogPartner = target;
            storyManager.CallActorDialogEvent(this.gameObject, target);
            BroadcastMessage("StartDialog", SendMessageOptions.DontRequireReceiver);
        }
    }

    // sb addressed this creature
    public bool Receive(GameObject addresser)
    {
        // may be disturbed
        if (!dialogPartner)
        {
            // start dialog
            // Debug.Log(this.name + " has new dialog partner");
            dialogPartner = addresser;
            BroadcastMessage("StartDialog", SendMessageOptions.DontRequireReceiver);

            // tell addresser: "i join dialog"
            return true;
        }
        else
        {
            // tell addresser: "call me later"
            return false;
        }
    }

    public void QuitDialog()
    {
        dialogPartner = null;
    }

    [Command]
    void CmdTellServerPartner(string name)
    {
        dialogPartner = GameObject.Find(name);
    }
    
    void OnPartnerChanged(GameObject newPartner)
    {
        dialogPartner = newPartner;
    }
}
