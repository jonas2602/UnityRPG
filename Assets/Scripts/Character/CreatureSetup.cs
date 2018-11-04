using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class CreatureSetup : NetworkBehaviour
{
    private GameObject cam;
    private Transform cameraRig;
    private PlayerAttributes attributes;

    private GameObject playerController;
    private GameObject aiController;
    private GameObject avatarInfo;
    private GameObject targetInfo;
    
    void Awake()
    {
        // get components
        cameraRig = GameObject.Find("CameraRig").transform;
        cam = cameraRig.Find("Camera").gameObject;
        attributes = GetComponent<PlayerAttributes>();

        playerController = transform.Find("PlayerController").gameObject;
        aiController = transform.Find("AiController").gameObject;
        // avatarInfo = transform.Find("AvatarInfo").gameObject;
        // targetInfo = transform.Find("TargetInfo").gameObject;
    }

    // Use this for initialization
    void Start ()
    {
        if(isLocalPlayer)
        {
            StartPlayerControl();
        }
        else
        {
            StartAiControl();
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public override void PreStartClient()
    {
        
    }
    
    public override void OnStartLocalPlayer()
    {
        // Debug.Log("setup finished");
    }

    void StartAiControl()
    {
        // stop player control
        playerController.SetActive(false);
        GetComponent<NetworkIdentity>().localPlayerAuthority = false;

        // update movement controll
        GetComponent<UnityEngine.AI.NavMeshObstacle>().enabled = false;
        GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = true;

        // start ai control
        aiController.SetActive(true);

        attributes.controlMode = ControlMode.Ai;
    }

    void StartPlayerControl()
    {
        // stop ai control
        aiController.SetActive(false);

        // update components
        cam.GetComponent<CameraControllerNew>().SetupCamera(this.transform.root);
        GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
        GetComponent<UnityEngine.AI.NavMeshObstacle>().enabled = true;
        GetComponent<NetworkIdentity>().localPlayerAuthority = true;

        // get Interface
        GameObject ui = GameObject.FindWithTag("Interface");
        ui.BroadcastMessage("SetupPlayer", this.gameObject);
        GetComponent<Health>().AddHealthbarScript(ui.GetComponentsInChildren<HealthbarScript>(true)[0]);

        // start player control
        playerController.SetActive(true);

        // activate camera
        cam.GetComponent<Camera>().enabled = true;
        cam.name = "Camera (" + this.name + ")";
        cam.GetComponent<CameraControllerNew>().enabled = true;
        cam.GetComponent<AudioListener>().enabled = true;

        attributes.controlMode = ControlMode.Player;
    }
}
