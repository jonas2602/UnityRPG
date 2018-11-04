using UnityEngine;
using System.Collections;

public class RangedSlotInfo : SlotInfo
{

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    
    public override bool SetItemToSlot()
    {
        RangedWeapon weapon = GetComponentInChildren<ItemOnObject>().GetStoredItem as RangedWeapon;

        SlotInfo muntitionSlotInfo = connectedSlot.GetComponent<SlotInfo>();
        muntitionSlotInfo.allowedItem = new Munition(weapon.munitionType);

        // activate Slot
        muntitionSlotInfo.ActivateSlot();

        // is munition equiped
        if (connectedSlot.transform.childCount > 0)
        {
            // get current munition
            Munition munition = connectedSlot.GetComponentInChildren<ItemOnObject>().GetStoredItem as Munition;

            // munition type is not allowed
            if (munition.itemSubtype != weapon.munitionType)
            {
                // connected slotItem invalid
                return false;
            }
        }

        return true;
    }

    public override bool WithdrawItemFromSlot()
    {
        SlotInfo offHandSlotInfo = connectedSlot.GetComponent<SlotInfo>();

        // deactivate Slot
        offHandSlotInfo.DeactivateSlot();

        // is secondary weapon equiped
        if (connectedSlot.transform.childCount > 0)
        {
            // remove to inventory
            return false;
        }

        return true;
    }
}
