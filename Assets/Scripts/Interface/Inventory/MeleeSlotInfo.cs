using UnityEngine;
using System.Collections;

public class MeleeSlotInfo : SlotInfo
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override bool SetItemToSlot()
    {
        MeleeWeapon weapon = GetComponentInChildren<ItemOnObject>().GetStoredItem as MeleeWeapon;
        SlotInfo offHandSlotInfo = connectedSlot.GetComponent<SlotInfo>();

        // Debug.Log(weapon.handInfo.preferedHand + ", " + weapon.handInfo.twoHanded);

        // player is not able to port secondary weapon
        if (weapon.handInfo.twoHanded)
        {
            // is secondary weapon equiped
            if (connectedSlot.transform.childCount > 0)
            {
                // connected slotItem invalid
                return false;
            }
        }
        else
        {
            // activate Slot
            offHandSlotInfo.ActivateSlot();
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
