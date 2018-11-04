using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Player_NetworkSetup : NetworkBehaviour {

    public GameObject cam;
    public Transform cameraRig;


    public Camera GetCamera
    {
        get
        {
            return cam.GetComponent<Camera>();
        }
    }
    
    void Update()
    {

    }
    /*
    public override void PreStartClient()
    {
        // get components
        cameraRig = GameObject.Find("CameraRig").transform;
        cam = cameraRig.Find("Camera").gameObject;
    }
    
    public override void OnStartLocalPlayer()
    {
        // activate character
        GetComponent<CharakterControler>().enabled = true;
        // activate camera
        cam.GetComponent<Camera>().enabled = true;
        cam.name = "Camera (" + this.name + ")";
        cam.GetComponent<CameraController>().enabled = true;
        cam.GetComponent<AudioListener>().enabled = true;

        // set camera parent
        cam.transform.parent = cameraRig;

        // get Interface
        GameObject ui = GameObject.FindWithTag("Interface");
        ui.BroadcastMessage("SetupPlayer", this.gameObject);
        GetComponent<EquipmentManager>().SetupUiReferences(ui.GetComponentsInChildren<PlayerWeaponWheel>(true)[0]);
        GetComponent<PlayerHealth>().SetHealthbarScript = ui.GetComponentsInChildren<HealthbarScript>(true)[0];
        // Debug.Log("setup finished");
    }*/
}
