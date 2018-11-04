using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WheelSlot : MonoBehaviour {

    [SerializeField]
    protected SlotInfo referencedSlot;
    [SerializeField]
    protected Item referencedItem;
    [SerializeField]
    protected int referencedSlotId;
    
    private Image itemImage;
    private Text itemAmount;

    protected virtual void Awake()
    {
        itemImage = transform.Find("ItemIcon").GetComponent<Image>();
        itemAmount = transform.Find("ItemAmount").GetComponent<Text>();
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public virtual void SetupSlot(SlotInfo referencedSlot)
    {
        this.referencedSlot = referencedSlot;
        this.referencedSlotId = referencedSlot.equipId;
    }

    public virtual void GetReferencedItem(ref int[] currentSlots)
    {
        currentSlots[2] = referencedSlotId;
    }
    

    public virtual void UpdateConnection()
    {
        // update slot
        UpdateWheelSlot();
    }

    void UpdateWheelSlot()
    {
        // reference slot?
        if (referencedSlot)
        {
            // slot has item?
            ItemOnObject itemObject = referencedSlot.GetComponentInChildren<ItemOnObject>();
            if (itemObject)
            {
                // get item
                referencedItem = itemObject.GetStoredItem;

                // set icon
                if (referencedItem.itemIcon)
                {
                    itemImage.sprite = referencedItem.itemIcon;
                    itemImage.enabled = true;
                }
                // set amount
                if (referencedItem.itemAmount > 1)
                {
                    itemAmount.text = referencedItem.itemAmount.ToString();
                    itemAmount.enabled = true;
                }
            }
            else
            {
                itemImage.enabled = false;
                itemAmount.enabled = false;
            }
        }
        else
        {
            itemImage.enabled = false;
            itemAmount.enabled = false;
        }
    }
}
