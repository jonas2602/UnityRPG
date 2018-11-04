using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CrossHairController : MonoBehaviour
{
    private AnimInfo playerAnimInfo;
    private Image crossHair;

    public void SetupPlayer(GameObject player)
    {
        playerAnimInfo = player.GetComponent<AnimInfo>();
    }

    void Awake()
    {
        crossHair = GetComponent<Image>();
    }

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(!playerAnimInfo)
        {
            return;
        }

	    if(playerAnimInfo.IsAiming())
        {
            crossHair.enabled = true;
        }
        else
        {
            crossHair.enabled = false;
        }
	}
}
