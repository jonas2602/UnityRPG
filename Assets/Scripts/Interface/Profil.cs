using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Profil : MonoBehaviour
{


    public GUISkin skin;
    public ItemList database;
    public OldInventory inventory;
    public ItemManager itemManager;
    public HoldObject holdObject;
    public EquipmentManager equipment;
    public int currentWindow = 0;
    public int windowAmount = 3;

    public int armorSlots = 5;
    // public int weaponSlots = 8;
    public List<ItemStack> armorSet = new List<ItemStack>();
    public List<ItemStack> weaponSet = new List<ItemStack>();
    public List<ItemStack> toolSet = new List<ItemStack>();     // tools (2)
    public List<ItemStack> bombs = new List<ItemStack>();       // bombs (2)
    public List<ItemStack> consumable = new List<ItemStack>();  // Consumable / food / tränke (2)

    // Consumable / food / tränke (2)

    //position
    //ProfilWindow
    public float posX;
    public float posY;
    public float windowHeight = 590f;
    public float windowWidth = 450f;

    //switchBar
    public float posXS = 132f;
    public float posYS = 17f;
    public float switchHeight = 30f;
    public float switchWidth = 60f;
    public float distanceXS = 2;

    //ArmorSlots
    public float posXA1 = 200f;
    public float posYA1 = 70f;
    public float posXAL = 70f;
    public float posYAO = 200f;
    public float posXAR = 330f;
    public float posYAU = 370f;
    public float slotWidthA = 50f;

    //WeaponSlots
    //WeaponWindow
    public float posXW1 = 300f;
    public float posYW1 = 63f;
    public float posXW2 = 300f;
    public float posYW2 = 210f;
    public float WeaponWidth = 145f;

    //MainWeaponSet
    //FirstHand
    public float posXWF = 16f;
    public float posYWF = 50f;
    public float slotWidthWF = 60f;

    //SecondHand
    public float posXWS1 = 78f;
    public float posYWS1 = 28f;
    public float posXWS2 = 98f;
    public float posYWS2 = 53f;
    public float posXWS3 = 83f;
    public float posYWS3 = 78f;
    public float slotWidthWS = 40f;

    //First+Second unhovered
    public float distanceW = -32;
    public float slotWidthW = 50f;


    // Use this for initialization
    void Start()
    {
        database = GameObject.FindGameObjectWithTag("ItemDatabase").GetComponent<ItemList>();
        inventory = GetComponent<OldInventory>();
        itemManager = GetComponent<ItemManager>();
        holdObject = transform.root.GetComponent<HoldObject>();
        equipment = transform.root.GetComponent<EquipmentManager>();
        posX = (Screen.width - windowWidth) * 0.8f;
        posY = (Screen.height - windowHeight) * 0.05f;

        // Armor Set (5)
        for (int i = 0; i < armorSlots; i++)
        {
            ItemSubtype armorType = (ItemSubtype)i;
            armorSet.Add(new ItemStack(new Armor(armorType), 0));
        }

        // Melee Set (2)
        // weaponSet.Add(new ItemStack(new Weapon(ItemHand.Right), 0));
        // weaponSet.Add(new ItemStack(new Weapon(ItemHand.Left), 0));

        // Range Set (2)
        weaponSet.Add(new ItemStack(new RangedWeapon(), 0));
        weaponSet.Add(new ItemStack(new Munition(), 0));

        // Tool Set (2)
        toolSet.Add(new ItemStack(new Tool(), 0)); 
        toolSet.Add(new ItemStack(new Tool(), 0));

        // bombs (2)
        bombs.Add(new ItemStack(new Munition(ItemSubtype.Bomb), 0));
        bombs.Add(new ItemStack(new Munition(ItemSubtype.Bomb), 0));

        // Consumable / food / tränke (2)
        consumable.Add(new ItemStack(new Consumable(), 0));
        consumable.Add(new ItemStack(new Consumable(), 0));

        // Refresh();
    }


    // Update is called once per frame
    void Update()
    {
        /*
        if(Input.GetKeyDown("u"))
        {
            Debug.Log(weaponSet[3].slotItem.GetType()); 
        }
        */
    }


    public void DrawProfil(ref Item tooltipItem)
    {
        // Draw Window
        GUI.Box(new Rect(posX, posY, windowWidth, windowHeight), "", skin.GetStyle("Inventory"));

        //Draw switchBar
        for (int i = 0; i < windowAmount; i++)
        {
            Rect switchRect = new Rect(posX + posXS + i * (switchWidth + distanceXS), posY + posYS, switchWidth, switchHeight);
            GUI.Box((switchRect), "", skin.GetStyle("Inventory"));
            GUI.DrawTexture(switchRect, GetSlotSkin(i, 3));

            // mouse over slot
            if (switchRect.Contains(Event.current.mousePosition))
            {
                // Leftklick
                if (Event.current.button == 0 && Event.current.type == EventType.MouseUp && itemManager.GetDraggedItem.slotAmount == 0)
                {
                    currentWindow = i;
                }
            }

        }
        switch (currentWindow)
        {
            case 0:
                {
                    DrawArmor(ref tooltipItem);
                    break;
                }
            case 1:
                {
                    DrawWeapon(ref tooltipItem);
                    break;
                }
            case 2:
                {
                    DrawMagic();
                    break;
                }
        }
    }


    void DrawArmor(ref Item tooltipItem)
    {
        // Draw Armorslots
        for (int i = 0; i < armorSlots; i++)
        {
            Rect armorRect = GetArmorRect(i, 0);
            GUI.Box(armorRect, "", skin.GetStyle("Inventory-Slot"));

            // Slot is used
            if (armorSet[i].slotAmount != 0)
            {
                // GUI.DrawTexture(armorRect, armorSet[i].slotItem.itemIcon);
                
                // mouse over slot
                if (armorRect.Contains(Event.current.mousePosition))
                {
                    // Draw Tooltip
                    tooltipItem = armorSet[i].slotItem;

                    // mouseDrag
                    if (Event.current.button == 0 && Event.current.type == EventType.MouseDrag && itemManager.GetDraggedItem.slotAmount == 0)
                    {
                        itemManager.SetDraggedItem = armorSet[i];
                        armorSet[i] = new ItemStack(new Armor((ItemSubtype)i), 0);
                        itemManager.SetDraggingStart = new Vector2(1, i);
                        // equipment.UnequipFromPlayer(i);
                    }

                    // Rightklick
                    if (Event.current.button == 1 && Event.current.type == EventType.MouseUp && itemManager.GetDraggedItem.slotAmount == 0)
                    {
                        if (itemManager.FindSlot(1, i, armorSet[i]))
                        {
                            // unequip item
                            // equipment.UnequipFromPlayer(i);

                            // clear slot
                            armorSet[i] = new ItemStack(new Armor((ItemSubtype)i), 0);
                        }
                    }

                    // Mouse Up
                    if (Event.current.type == EventType.MouseUp && itemManager.GetDraggedItem.slotAmount != 0)
                    {
                        itemManager.NewItemMatch(1, i, armorSet[i], true, true);
                    }
                }

            }
            // slot not used
            else
            {
                GUI.DrawTexture(armorRect, GetSlotSkin(i, 0));
                // mouse over slot
                if (armorRect.Contains(Event.current.mousePosition))
                {
                    // mouse up
                    if (Event.current.type == EventType.MouseUp && itemManager.GetDraggedItem.slotAmount != 0)
                    {
                        itemManager.NewItemMatch(1, i, armorSet[i], true, false);
                    }
                }
            }
        }
    }


    void DrawWeapon(ref Item tooltipItem)
    {
        // Draw Weaponslots
        for (int i = 0; i < weaponSet.Count; i++)
        {
            Rect weaponRect = new Rect(posX + posXA1, posY + posYA1 + i * 60, slotWidthA, slotWidthA);
            GUI.Box(weaponRect, "", skin.GetStyle("Inventory-Slot"));

            // Slot is used
            if (weaponSet[i].slotAmount != 0)
            {
                // GUI.DrawTexture(weaponRect, weaponSet[i].slotItem.itemIcon);

                // Item is stackable    
                if (weaponSet[i].slotAmount > 1)
                {
                    Rect amountRect = new Rect(weaponRect.xMax - 15, weaponRect.yMax - 15, 15, 15);
                    GUI.Box(amountRect, "" + weaponSet[i].slotAmount, skin.GetStyle("Amount"));
                }

                // mouse over slot
                if (weaponRect.Contains(Event.current.mousePosition))
                {
                    // Draw Tooltip
                    tooltipItem = weaponSet[i].slotItem;

                    // mouseDrag
                    if (Event.current.button == 0 && Event.current.type == EventType.MouseDrag && itemManager.GetDraggedItem.slotAmount == 0)
                    {
                        itemManager.SetDraggedItem = weaponSet[i];
                        holdObject.RemoveWeapon(i);
                        weaponSet[i] = GetStandartStack(i);
                        itemManager.SetDraggingStart = new Vector2(2, i);
                    }

                    // Rightklick
                    if (Event.current.button == 1 && Event.current.type == EventType.MouseUp && itemManager.GetDraggedItem.slotAmount == 0)
                    {
                        if (itemManager.FindSlot(2, i, weaponSet[i]))
                        {
                            // unequip item
                            holdObject.RemoveWeapon(i);

                            // clear slot
                            weaponSet[i] = GetStandartStack(i);
                        }
                    }

                    // Mouse Up
                    if (Event.current.type == EventType.MouseUp && itemManager.GetDraggedItem.slotAmount != 0)
                    {
                        itemManager.NewItemMatch(2, i, weaponSet[i], true, true);
                    }
                }
            }
            else
            {
                GUI.DrawTexture(weaponRect, GetSlotSkin(i, 1));
                // mouse over slot
                if (weaponRect.Contains(Event.current.mousePosition))
                {
                    // mouse up
                    if (Event.current.type == EventType.MouseUp && itemManager.GetDraggedItem.slotAmount != 0)
                    {
                        itemManager.NewItemMatch(2, i, weaponSet[i], true, false);
                    }
                }

            }
        }
    }


    void DrawMagic()
    {

    }

    /*
    public int AddItem(int id)
    {
        int itemID = id;
        if (database.items[id].itemType == Item.ItemType.Armor)
        {
            for (int i = 0; i < armorSlots; i++)
            {
                if (itemManager.TypeMatch(database.items[id].itemType, i, database.items[id]))
                {
                    if (armor[i].slotAmount == 0)
                    {
                        itemManager.SetItem(1, i, new ItemStack(database.items[id], 1));
                        itemID = -1;
                    }
                    else
                    {
                        itemID = armor[i].slotItem.itemID;
                        itemManager.SetItem(1, i, new ItemStack(database.items[id], 1));
                    }
                    
                    break;
                }
            }
        }

        if (database.items[id].itemType == Item.ItemType.Weapon)
        {
            for (int i = 0; i < weaponSlots; i++)
            {
                if (itemManager.TypeMatch(database.items[id].itemType, i, database.items[id]))
                {
                    if (weaponSet[i].slotAmount == 0)
                    {
                        itemManager.SetItem(2, i, new ItemStack(database.items[id], 1));
                        itemID = -1;
                    }
                    else
                    {
                        itemID = weaponSet[i].slotItem.itemID;
                        itemManager.SetItem(2, i, new ItemStack(database.items[id], 1));
                    }

                    break;
                }
            }
        }
        if(itemID == -1)
        Debug.Log("successful equiped");
        else
        {
            Debug.Log("failed to equip");
        }
        return itemID;
    }
    */

    public void SetItem(int list, int slot, ItemStack item)
    {
        string itemType = item.slotItem.GetType().ToString();

        switch (list)
        {
            case 1: // ArmorSet
                {
                    armorSet[slot] = item;
                    // equipment.EquipToPlayer(item.slotItem.itemMesh, slot);
                    break;
                }
            case 2: // WeaponSet
                {
                    weaponSet[slot] = item;
                    if (itemType != "Munition")
                    {
                        holdObject.AddWeapon(item.slotItem as Weapon, slot);
                    }
                    // is equiped munition still usable
                    if (itemType == "RangedWeapon" && weaponSet[slot + 1].slotAmount != 0)
                    {
                        UpdateMunition(slot + 1);
                    }
                    break;
                }
            case 3: // 
                {
                    break;
                }
        }
    }


    void UpdateMunition(int munitionSlot)
    {
        // RangedWeapon rangedWeapon = weaponSet[munitionSlot - 1].slotItem as RangedWeapon;
        // Munition munition = weaponSet[munitionSlot].slotItem as Munition;
        // don't fit
        if (/*munition.munitionType != rangedWeapon.munitionType*/true)
        {
            // remove munition
            if (itemManager.FindSlot(2, munitionSlot, weaponSet[munitionSlot]))
            {
                // weaponSet[munitionSlot] = new ItemStack(new Munition(rangedWeapon.munitionType), 0);
            }
        }
    }

    Rect GetArmorRect(int ID, int list)
    {
        Rect slotRect = new Rect();
        switch (list)
        {
            case 0:
                {
                    switch (ID)
                    {
                        case 0:
                            {
                                slotRect = new Rect(posX + posXA1, posY + posYA1, slotWidthA, slotWidthA);
                                break;
                            }
                        case 1:
                            {
                                slotRect = new Rect(posX + posXAL, posY + posYAO, slotWidthA, slotWidthA);
                                break;
                            }
                        case 2:
                            {
                                slotRect = new Rect(posX + posXAR, posY + posYAO, slotWidthA, slotWidthA);
                                break;
                            }
                        case 3:
                            {
                                slotRect = new Rect(posX + posXAL, posY + posYAU, slotWidthA, slotWidthA);
                                break;
                            }
                        case 4:
                            {
                                slotRect = new Rect(posX + posXAR, posY + posYAU, slotWidthA, slotWidthA);
                                break;
                            }
                    }
                    break;
                }
            case 1:
                {
                    switch (ID)
                    {
                        case 0:
                            {
                                slotRect = new Rect(posX + posXW1 + posXWF, posY + posYW1 + posYWF, slotWidthWF, slotWidthWF);
                                break;
                            }
                        case 1:
                            {
                                slotRect = new Rect(posX + posXW1 + posXWS3, posY + posYW1 + posYWS3, slotWidthWS, slotWidthWS);
                                break;
                            }
                        case 2:
                            {
                                slotRect = new Rect(posX + posXW1 + posXWS2, posY + posYW1 + posYWS2, slotWidthWS, slotWidthWS);
                                break;
                            }
                        case 3:
                            {
                                slotRect = new Rect(posX + posXW1 + posXWS1, posY + posYW1 + posYWS1, slotWidthWS, slotWidthWS);
                                break;
                            }
                    }
                    break;
                }
        }
        return slotRect;
    }


    /***
     * ID:
     * 0 = Armor
     * 1 = Weapon
     * 2 = Magic
     * 3 = Switch
     **/
    public Texture GetSlotSkin(int ID, int list)
    {
        Texture slotSkin = null;
        switch (list)
        {
            case 0:
                {
                    switch (ID)
                    {
                        case 0:
                            {
                                slotSkin = Resources.Load<Texture>("SlotIcons/" + "Helm");
                                break;
                            }
                        case 1:
                            {
                                slotSkin = Resources.Load<Texture>("SlotIcons/" + "Harnisch");
                                break;
                            }
                        case 2:
                            {
                                slotSkin = Resources.Load<Texture>("SlotIcons/" + "Handschuhe");
                                break;
                            }
                        case 3:
                            {
                                slotSkin = Resources.Load<Texture>("SlotIcons/" + "Hose");
                                break;
                            }
                        case 4:
                            {
                                slotSkin = Resources.Load<Texture>("SlotIcons/" + "Schuhe");
                                break;
                            }
                    }
                    break;
                }
            case 1:
                {
                    switch (ID)
                    {
                        case 0:
                            {
                                slotSkin = Resources.Load<Texture>("SlotIcons/" + "Bogen");
                                break;
                            }
                        case 1:
                            {
                                slotSkin = Resources.Load<Texture>("SlotIcons/" + "Pfeil");
                                break;
                            }
                        case 2:
                            {
                                slotSkin = Resources.Load<Texture>("SlotIcons/" + "Dolch");
                                break;
                            }
                        case 3:
                            {
                                slotSkin = Resources.Load<Texture>("SlotIcons/" + "Schwert");
                                break;
                            }
                        case 4:
                            {
                                slotSkin = Resources.Load<Texture>("SlotIcons/" + "Faust");
                                break;
                            }
                        case 5:
                            {
                                slotSkin = Resources.Load<Texture>("SlotIcons/" + "Magie");
                                break;
                            }
                        case 6:
                            {
                                slotSkin = Resources.Load<Texture>("SlotIcons/" + "Großschwert");
                                break;
                            }
                        case 7:
                            {
                                slotSkin = Resources.Load<Texture>("SlotIcons/" + "Wurfmesser");
                                break;
                            }
                    }
                    break;
                }
            case 2:
                {
                    break;
                }
            case 3:
                {
                    switch (ID)
                    {
                        case 0:
                            {
                                slotSkin = Resources.Load<Texture>("SlotIcons/" + "Armor");
                                break;
                            }
                        case 1:
                            {
                                slotSkin = Resources.Load<Texture>("SlotIcons/" + "Weapon");
                                break;
                            }
                        case 2:
                            {
                                slotSkin = Resources.Load<Texture>("SlotIcons/" + "Magic");
                                break;
                            }
                    }
                    break;
                }
        }
        return slotSkin;
    }


    void Refresh()
    {
        for (int i = 0; i < armorSet.Count; i++)
        {
            if (armorSet[i].slotAmount != 0)
            {
                // equipment.EquipToPlayer(armorSet[i].slotItem.itemMesh, i);
                Debug.Log(i + ". fixed");
            }
            else
            {
                if (i == 4)
                {
                    // equipment.EquipToPlayer(database.items[12].itemMesh, 4);
                    Debug.Log(i + ". fixed");
                }
                else
                {
                    Debug.Log(i + ". correct");
                }
            }
        }
    }


    ItemStack GetStandartStack(int id)
    {
        ItemStack newStack = null;
        switch (id)
        {

            case 0:
            case 1:
                {
                    // newStack = new ItemStack(new Weapon((ItemHand)id), 0);
                    break;
                }
            case 2:
                {
                    newStack = new ItemStack(new RangedWeapon(), 0);
                    break;
                }
            case 3:
                {
                    newStack = new ItemStack(new Munition(), 0);
                    break;
                }
        }

        return newStack;
    }
}