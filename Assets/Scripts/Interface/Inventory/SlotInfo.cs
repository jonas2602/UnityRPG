using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SlotInfo : MonoBehaviour
{
    public bool testNeeded;

    public bool testType;
    public bool testSubtype;
    public bool testHand;
    public Item allowedItem;
    public SlotInfo connectedSlot;

    public int equipId;

    // Use this for initialization
    void Awake()
    {

    }
	
	// Update is called once per frame
	void Update ()
    {
	
	}


    public void SetupSlotInfo(Item allowedItem, bool testType, bool testSubtype, bool testHand)
    {
        this.allowedItem = allowedItem;
        this.testType = testType;
        this.testSubtype = testSubtype;
        this.testHand = testHand;
    }

    public void ActivateSlot()
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

    }

    public void DeactivateSlot()
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.5f;
    }

    /*public virtual bool TestConnectedSlot()
    {
        if(connectedSlot)
        {
            connectedSlot.ActivateSlot();
        }

        return true;
    }*/

    public virtual bool SetItemToSlot()
    {
        if (connectedSlot)
        {
            connectedSlot.ActivateSlot();
        }

        return true;
    }
    
    public virtual bool WithdrawItemFromSlot()
    {
        if (connectedSlot)
        {
            connectedSlot.DeactivateSlot();
        }

        return true;
    }
}
