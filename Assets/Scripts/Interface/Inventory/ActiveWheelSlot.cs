using UnityEngine;
using System.Collections;

public class ActiveWheelSlot : MonoBehaviour {

    public int slotId = -1;

    public void SetParent(Transform slot, int slotId)
    {
        this.transform.position = slot.position;
        this.slotId = slotId;
    }


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
