using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ItemOnObject : MonoBehaviour
{
    [SerializeField]
    private Item storedItem;

    private Image itemImage;
    private Text itemAmount;
    
    public Item GetStoredItem
    {
        get
        {
            return this.storedItem;
        }
    }

    // Use this for initialization
    void Awake()
    {
        itemImage = transform.Find("ItemIcon").GetComponent<Image>();
        itemAmount = transform.Find("ItemAmount").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Set new Item
    public void SetupItem(Item item)
    {
        storedItem = item;
        // set icon
        if (storedItem.itemIcon)
        {
            itemImage.sprite = storedItem.itemIcon;
        }
        // set amount
        if (storedItem.itemAmount > 1)
        {
            itemAmount.text = storedItem.itemAmount.ToString();
            itemAmount.enabled = true;
        }
    }

    // Add to itemStack
    public void AddToItemStack(int amount)
    {
        storedItem.itemAmount += amount;
        itemAmount.text = storedItem.itemAmount.ToString();
        itemAmount.enabled = true;
    }

    // Remove from itemStack
    public void RemoveFromItemStack(int amount)
    {
        storedItem.itemAmount -= amount;
        itemAmount.text = storedItem.itemAmount.ToString();
        if(storedItem.itemAmount == 1)
        {
            itemAmount.enabled = false;
        }
    }
}
