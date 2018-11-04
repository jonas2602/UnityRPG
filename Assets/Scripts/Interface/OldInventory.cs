using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OldInventory : MonoBehaviour {

    public int slotsX, slotsY;
    public GUISkin skin;
    public List<ItemStack> slots = new List<ItemStack>();
    private ItemList database;
    public Profil profil;
    public ItemManager itemManager;
    public float posX = 140f;
    public float posY = 140f;
    public float slotSize = 60f;
    public float slotDistance = 10;

    

	// Use this for initialization
	void Start () 
    {
        for (int i = 0; i < (slotsX * slotsY); i++)
        {
            slots.Add(new ItemStack());

        }
        database = GameObject.FindWithTag("ItemDatabase").GetComponent<ItemList>();
        itemManager = GetComponent<ItemManager>();
        profil = GetComponent<Profil>();

        for (int i = 0; i <= 9; i++)
        {
            AddItem(i);
        }

        AddItem(2, 10);
	}


    public void DrawInventory(ref Item tooltipItem)
    {
        // Draw Window
        GUI.Box(new Rect(posX, posY, slotsX * 60 + 10, slotsY * 60 + 10), "", skin.GetStyle("Inventory"));

        // Draw Inventoryslots
        int slotCount = 0;
        for (int y = 0; y < slotsY; y++)
        {
            for (int x = 0; x < slotsX; x++)
            {
                Rect slotRect = new Rect(posX + 10 + x * 60, posY + 10 + y * 60, 50, 50);
                GUI.Box(slotRect, "", skin.GetStyle("Inventory-Slot"));

                // Slot is used
                if (slots[slotCount].slotAmount != 0)
                {
                    // GUI.DrawTexture(slotRect, slots[slotCount].slotItem.itemIcon);
                    
                    // Item is stackable    
                    if (slots[slotCount].slotAmount > 1)
                    {
                        Rect amountRect = new Rect(posX + 45 + x * 60, posY + 45 + y * 60, 15, 15);
                        GUI.Box(amountRect, "" + slots[slotCount].slotAmount, skin.GetStyle("Amount"));
                    }
                    
                    // mouse over slot
                    if (slotRect.Contains(Event.current.mousePosition))
                    {
                        // Draw Tooltip
                        tooltipItem = slots[slotCount].slotItem;

                        // mouseDrag
                        if (Event.current.button == 0 && Event.current.type == EventType.MouseDrag && itemManager.GetDraggedItem.slotAmount == 0)
                        {
                            itemManager.SetDraggedItem = slots[slotCount];
                            slots[slotCount] = new ItemStack();
                            itemManager.SetDraggingStart = new Vector2(0, slotCount);
                        }
                        // Rightklick
                        if (Event.current.button == 1 && Event.current.type == EventType.MouseUp && itemManager.GetDraggedItem.slotAmount == 0)
                        {
                            if (itemManager.FindSlot(0, slotCount, slots[slotCount]))
                            {
                                // clear slot
                                slots[slotCount] = new ItemStack();
                            }
                        }

                        // Mouse Up
                        if (Event.current.type == EventType.MouseUp && itemManager.GetDraggedItem.slotAmount != 0)
                        {
                            itemManager.NewItemMatch(0, slotCount, slots[slotCount], false, true);
                        }
                    }

                }
                // slot not used
                else
                {
                    // mouse over slot
                    if (slotRect.Contains(Event.current.mousePosition))
                    {
                        // mouse up
                        if (Event.current.type == EventType.MouseUp && itemManager.GetDraggedItem.slotAmount != 0)
                        {
                            itemManager.NewItemMatch(0, slotCount, slots[slotCount], false, false);
                        }
                    }
                }
                slotCount++;
            }
        }
    }

    /*
    string CreateTooltip(ItemStack itemstack)
    {
        tooltip = "<color=#ffffff>" + itemstack.slotItem.itemName + "</color>\n\n"
                + "(Developer)ID: " + itemstack.slotItem.itemID + "\n"
                + "Desc: \n" + itemstack.slotItem.itemDesc; 
                                                            
        return tooltip;
    }
    */

    public int AddItem(int id)
    {
        for(int i = 0; i< slots.Count;i++)                              //ist noch ein höherer inventarplatz vorhanden
        {                                                               //wenn ja i++
            if (slots[i].slotItem == null)                              //ist der aktuelle platz leer?
            {
                for (int j = 0; j < database.items.Count;j++)           //geht die itemdatabase durch
                {
                    if(database.items[j].itemID == id)                  //ist das item mit der id j = der eingegebenen id
                    {
                        slots[i] = new ItemStack(database.items[j],1);  //setzt in den freien inventarslot den gegenstand mit der id "id" 
                        return 1;
                    }

                }
                return -1;    
            }
        }
        return 0;
    }


    public int AddItem(int ID, int amount)
    {
        // Item is stackable?
        if (database.items[ID].itemType == ItemType.Consumable)
        {
            bool added = false;
            int firstFreeSpot = -1;

            // go through the inventory
            for (int i = 0; i < slots.Count; i++)
            {
                // is there a stack?
                if (slots[i].slotItem != null)
                {
                    if (slots[i].slotItem.itemID == ID)
                    {
                        slots[i].slotAmount += amount;
                        added = true;
                        return -1;
                    }
                }
                // seach first free place
                if (slots[i].slotAmount == 0 && firstFreeSpot == -1)
                {
                    firstFreeSpot = i;
                }
            }
            //if no stack exists
            if (added == false)
            {
                if (firstFreeSpot != -1)
                {
                    slots[firstFreeSpot] = new ItemStack(database.items[ID], amount);
                    return -1;
                }
                else
                {
                    return 0;
                }
            }
        }
        else
        {
            Debug.Log("Error: Wrong Add-Funktion");
        }
        return 0;
    }


    public int AddItem(string name)
    {
        for (int i = 0; i < slots.Count; i++)                           //ist noch ein höherer inventarplatz vorhanden
        {                                                               //wenn ja i++
            if (slots[i].slotItem == null)                              //ist der aktuelle platz leer?
            {
                for (int j = 0; j < database.items.Count; j++)          //geht die itemdatabase durch
                {
                    if (database.items[j].itemName == name)             //ist das item mit der id j = der eingegebenen id
                    {
                        slots[i] = new ItemStack(database.items[j], 1);  //setzt in den freien inventarslot den gegenstand mit der id "id" 
                        return -1;
                    }

                }
                return 1;
            }
        }
        return 0;
    }


    public int AddItem(string name, int amount)
    {
        for (int i = 0; i < slots.Count; i++)                                   //ist noch ein höherer inventarplatz vorhanden
        {                                                                       //wenn ja i++
            if (slots[i].slotItem == null)                                      //ist der aktuelle platz leer?
            {
                for (int j = 0; j < database.items.Count; j++)                  //geht die itemdatabase durch
                {
                    if (database.items[j].itemName == name)                     //ist das item mit der id j = der eingegebenen id
                    {
                        slots[i] = new ItemStack(database.items[j], amount);    //setzt in den freien inventarslot den gegenstand mit der id "id" 
                        return -1;
                    }

                }
                return 1;
            }
        }
        return 0;
    }


    public void SetItem(int id, ItemStack item)
    {
        slots[id] = item;
    }


    public bool InventoryContains(int id)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if(slots[i].slotItem != null)
            {
                if (slots[i].slotItem.itemID == id)
                {
                    return true;
                }
            }
        }
        return false;
    }


    public bool InventoryContains(int id, int amount)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].slotItem.itemID == id && slots[i].slotAmount == amount)
            {
                return true;
            }
        }
        return false;
    }


    public void RemoveItem(int id)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].slotItem.itemID == id)
            {
                slots[i] = new ItemStack();
                break;
            }
        }
    }


    public void RemoveItem(int id, int amount)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].slotAmount > 0)
            {

                if (slots[i].slotItem.itemID == id)
                {
                    int quantity = slots[i].slotAmount - amount;
                    if (quantity < 1)
                    {
                        slots[i] = new ItemStack();
                        break;
                    }

                    if (quantity >= 1)
                    {
                        slots[i].slotAmount -= amount;
                        break;
                    }
                    Debug.Log("Error: Remove failed");
                }
            }
        }
    }


    public void RemoveItem(string name, int amount)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].slotAmount > 0)
            {

                if (slots[i].slotItem.itemName == name)
                {
                    int quantity = slots[i].slotAmount - amount;
                    if (quantity < 1)
                    {
                        slots[i] = new ItemStack();
                        break;
                    }

                    if (quantity >= 1)
                    {
                        slots[i].slotAmount -= amount;
                        break;
                    }
                    Debug.Log("Error: Remove failed");
                }
            }
        }
    }


    public int GetSpace()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].slotAmount == 0)
            {
                return i;
            }
        }

        return -1;
    }


    void SaveInventory()
    {
        for(int i = 0; i < slots.Count; i++)
        {
            PlayerPrefs.SetInt("Inventory " + i, slots[i].slotItem.itemID);
        }
    }

    /*
    void LoadInventory()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i] = new ItemStack(PlayerPrefs.GetInt("Inventory " + i, -1) >= 0 ? database.items[PlayerPrefs.GetInt("Inventory " + i)] : new Item(),1);
        }
    }*/
}
