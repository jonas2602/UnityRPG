using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WheelWeaponSlot : WheelSlot
{
    [SerializeField]
    private SlotInfo referencedOffSlot;
    [SerializeField]
    private Item referencedOffItem;
    [SerializeField]
    private int referencedOffSlotId;

    private Image offItemImage;
    private Text offItemAmount;
    private GameObject offSlot;

    protected override void Awake()
    {
        base.Awake();

        offSlot = transform.Find("OffSlot").gameObject;
        offItemImage = offSlot.transform.Find("ItemIcon").GetComponent<Image>();
        offItemAmount = offSlot.transform.Find("ItemAmount").GetComponent<Text>();
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public override void SetupSlot(SlotInfo referencedSlot)
    {
        referencedOffSlot = referencedSlot.connectedSlot;
        if (referencedOffSlot)
        {
            referencedOffSlotId = referencedOffSlot.equipId;
        }
        base.SetupSlot(referencedSlot);
    }

    public override void GetReferencedItem(ref int[] currentSlots)
    {
        currentSlots[0] = referencedSlotId;
        if (referencedOffSlot)
        {
            currentSlots[1] = referencedOffSlotId;
        }
        else
        {
            currentSlots[1] = -1;
        }
    }

    public override void UpdateConnection()
    {
        base.UpdateConnection();

        UpdateOffWheelSlot();
    }

    void UpdateOffWheelSlot()
    {
        // reference offSlot?
        if (referencedOffSlot)
        {
            // slot has offItem?
            ItemOnObject itemObject = referencedOffSlot.GetComponentInChildren<ItemOnObject>();
            if (itemObject)
            {
                offSlot.SetActive(true);

                // get item
                referencedOffItem = itemObject.GetStoredItem;

                // set icon
                if (referencedOffItem.itemIcon)
                {
                    offItemImage.sprite = referencedOffItem.itemIcon;
                    offItemImage.enabled = true;
                }
                // set amount
                if (referencedOffItem.itemAmount > 1)
                {
                    offItemAmount.text = referencedOffItem.itemAmount.ToString();
                    offItemAmount.enabled = true;
                }
            }
            else
            {
                offSlot.SetActive(false);
            }
        }
        else
        {
            offItemImage.enabled = false;
            offItemAmount.enabled = false;
            offSlot.SetActive(false);
        }


    }
}
