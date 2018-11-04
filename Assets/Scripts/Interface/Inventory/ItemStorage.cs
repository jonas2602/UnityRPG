using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemStorage : MonoBehaviour
{
    [SerializeField]
    private List<Item> inventoryItems = new List<Item>();
    [SerializeField]
    private Item money;
    private ItemList itemDatabase;

    public List<Item> GetInventoryItemList
    {
        get
        {
            return this.inventoryItems;
        }
    }

    public virtual List<Item> GetLoot()
    {
        List<Item> loot = new List<Item>();

        // add money
        if (money.itemAmount > 0)
        {
            loot.Add(money);
        }

        // add inventory
        for(int i = 0;i < inventoryItems.Count;i++)
        {
            loot.Add(inventoryItems[i]);
        }
        
        return loot;
    }

    public Item GetMoney
    {
        get
        {
            return money;
        }
    }

    protected virtual void Awake()
    {
        itemDatabase = GameObject.FindWithTag("Database").GetComponent<ItemList>();
        money = itemDatabase.GetCurrencyByName("Goldmünze");
        money.itemAmount = 0;
    }

	// Use this for initialization
	protected virtual void Start ()
    {
        LoadItemsFromSaveGame();
	}
	
	// Update is called once per frame
	void Update ()
    {
        
    }

    public void AddItemToInventory(Item item)
    {
        // add money
        if (item.itemSubtype == ItemSubtype.Geld)
        {
            AddMoneyToInventory(item.itemAmount);
            return;
        }
        else
        {
            /*
            // item is stackable
            if (item.itemStackable)
            {
                for (int i = 0; i < inventoryItems.Count; i++)
                {
                    // search for existing stack
                    if(item.itemID == inventoryItems[i].itemID)
                    {
                        inventoryItems[i].itemAmount += item.itemAmount;
                        return;
                    }
                }
            }
            */

            // add item to new stack
            inventoryItems.Add(item);
        }
    }

    public void AddItemToInventory(string itemName)
    {
        Item item = itemDatabase.GetItemByName(itemName);
        inventoryItems.Add(item);
    }

    public void WithdrawItemFromInventory(Item item)
    {
        // remove money
        if (item.itemSubtype == ItemSubtype.Geld)
        {
            WithdrawMoneyFromInventory(money.itemAmount);
            return;
        }
        else
        {
            inventoryItems.Remove(item);
        }
    }
    
    public void ClearInventory()
    {
        inventoryItems.Clear();
    }

    public void AddMoneyToInventory(int amount)
    {
        money.itemAmount += amount;
    }

    public void WithdrawMoneyFromInventory(int amount)
    {
        money.itemAmount -= amount;
        
        if(money.itemAmount < 0)
        {
            Debug.LogError("not enough money stored");
            money.itemAmount = 0;
        }
    }

    void LoadItemsFromSaveGame()
    {
        // Add items
        for (int i = 0; i < 14; i++)
        {
            Item item = itemDatabase.GetItemById(i);
            AddItemToInventory(item);
        }

        // add munition
        Item munition = itemDatabase.GetItemById(2);
        munition.itemAmount = 100;
        AddItemToInventory(munition);
    }
}
