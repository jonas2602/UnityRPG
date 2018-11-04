using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{
    public OldInventory inventory;
    public Profil profil;
    public EquipmentManager equipment;
    public Interface Interface;
    public HoldObject holdObject;

    public ItemStack draggedItem;
    public Vector2 draggingStart; // (startList, startPoint)
    public bool testStart;


    public ItemStack GetDraggedItem
    {
        get
        {
            return draggedItem;
        }
    }

    public Vector2 GetDraggingStart
    {
        get
        {
            return draggingStart;
        }
    }

    public ItemStack SetDraggedItem
    {
        set
        {
            draggedItem = value;
        }
    }

    public Vector2 SetDraggingStart
    {
        set
        {
            draggingStart = value;
        }
    }

    // Use this for initialization
    void Start ()
    {
        Transform player = this.transform.root;

        inventory = GetComponent<OldInventory>();
        profil = GetComponent<Profil>();
        equipment = player.GetComponent<EquipmentManager>();
        Interface = GetComponent<Interface>();
        holdObject = player.GetComponent<HoldObject>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}


    /**
     *  Lists:
     *  Inventory = 0
     *  Armor = 1
     *  MeleeWeapon = 2
     *  RangeWeapon = 3
     *  Shop = 4
    **/
    public void NewItemMatch(int targetList, int targetPoint, ItemStack currentItem, bool testTarget, bool targetUsed)
    {
        bool targetProved = true;
        bool startProved = true;

        // targetset need test
        if (testTarget)
        {
            // test dragged item
            if (!NewTypeMatch(currentItem, draggedItem))
            {
                targetProved = false;
            }
        }
        // startset need test
        if (testStart)
        {
            // slot is used
            if (targetUsed)
            {
                // test current item in targetslot
                if (!NewTypeMatch(currentItem, draggedItem))
                {
                    startProved = false;
                }
            }
        }

        int startList = (int)draggingStart.x;
        int startPoint = (int)draggingStart.y;

        // item is allowed to set
        if (startProved && targetProved)
        {
            // Slot is still used
            if (targetUsed)
            {
                // switch items
                SetItem(startList, startPoint, currentItem);
            }

            // set item to target
            SetItem(targetList, targetPoint, draggedItem);
            draggedItem = new ItemStack();
            Debug.Log("Set success");
        }
        else
        {
            // reset item to start
            SetItem(startList, startPoint, draggedItem);
            draggedItem = new ItemStack();
            Debug.Log("Reset success");
        }
    }


    bool NewTypeMatch(ItemStack slotStack, ItemStack newItem)
    {
        //item is from same instance ...
        if (slotStack.slotItem.GetType() == newItem.slotItem.GetType())
        {
            switch(slotStack.slotItem.GetType().ToString())
            {
                case "Armor":
                    {
                        Armor slot = slotStack.slotItem as Armor;
                        Armor newArmor = newItem.slotItem as Armor;
                        
                        // ... same subtype
                        if (slot.armorType == newArmor.armorType)
                        {
                            return true;
                        }
                        break;
                    }
                case "Weapon":
                    {
                        Weapon slot = slotStack.slotItem as Weapon;
                        Weapon newWeapon = newItem.slotItem as Weapon;

                        // ... same weaponHand
                        if(slot.handInfo == newWeapon.handInfo)
                        {
                            return true;
                        }
                        break;
                    }
                case "Munition":
                    {
                        Munition slot = slotStack.slotItem as Munition;
                        Munition newMunition = newItem.slotItem as Munition;

                        // munition fit to weapon
                        if (slot/*.munitionType*/ == newMunition/*.munitionType*/)
                        {
                            return true;
                        }

                        break;
                    }
                default:
                    {
                        return true;
                    }
            }
        }
        return false;
    }


    public bool FindSlot(int startList, int startPoint, ItemStack newItem)
    {
        bool clearSlot = true;

        // move to inventory
        if (startList != 0)
        {
            Debug.Log("move " + newItem.slotItem.itemName + " to inventory");

            int space = inventory.GetSpace();
            // inventory has space
            if (space != -1)
            {
                // set item
                SetItem(0, space, newItem);
            }
            else
            {
                // reset item
                clearSlot = false;
            }
        }
        // Find open window
        else
        {
            Debug.Log("move " + newItem.slotItem.itemName + " from inventory ...");
            // get correct list
            int targetList = -1;
            int targetPoint = -1;

            GetNewSlot(newItem, ref targetList, ref targetPoint);

            // item has slot to set
            if (targetList != -1 && targetPoint != -1)
            {
                Debug.Log("... to " + targetList + ", " + targetPoint);
                ItemStack currentItem = GetList(targetList)[targetPoint];

                // Slot is still used
                if (currentItem.slotAmount != 0)
                {
                    // switch items
                    SetItem(startList, startPoint, currentItem);
                    clearSlot = false;
                }

                // set item to target
                SetItem(targetList, targetPoint, newItem);
                Debug.Log("Set success");
            }
            // no slot to set this item
            else
            {
                return false;
            }
        }

        return clearSlot;
    }


    void GetNewSlot(ItemStack newItem, ref int targetList, ref int targetPoint)
    {
        System.Type itemType = newItem.slotItem.GetType();

        Interface.GUIType window = Interface.currentGUI;

        // switch lists
        switch (window)
        {
            case Interface.GUIType.Inventory:
                {
                    //switch types
                    if(itemType.ToString() ==  "Armor") // ArmorList
                    {
                        targetList = 1;
                        targetPoint = (int)(newItem.slotItem as Armor).armorType;
                    }
                    else // WeaponList
                    {
                        targetList = 2;
                        List<ItemStack> list = GetList(targetList);

                        for(int i = 0; i < list.Count;i++)
                        {
                            if(itemType == list[i].slotItem.GetType())
                            {
                                targetPoint = i;
                                break;
                            }
                        }
                        /*if (itemType == "RangedWeapon")
                        {
                            targetPoint = 2;
                        }
                        else if (itemType == "Munition")
                        {
                            targetPoint = 3;
                        }
                        else if(itemType == "Weapon")
                        {
                            targetPoint = (int)(newItem.slotItem as Weapon).itemHand;
                        }
                        else
                        {
                            Debug.Log("no slot found to set " + newItem.slotItem.itemName);
                        }*/
                    }

                    break;
                }
            default:
                {
                    Debug.LogError(window.ToString() + "is not supported");
                    break;
                }
        }
    }


    void SetItem(int targetList, int targetPoint, ItemStack item)
    {
        switch (targetList)
        {
            case 0:
                {
                    inventory.SetItem(targetPoint, item);
                    break;
                }

            case 1:
            case 2:

                {
                    profil.SetItem(targetList, targetPoint, item);
                    break;
                }
        }

    }


    List<ItemStack> GetList(int listId)
    {
        List<ItemStack> list = null;

        switch(listId)
        {
            case 0:
                {
                    list = inventory.slots;
                    break;
                }
            case 1:
                {
                    list = profil.armorSet;
                    break;
                }
            case 2:
                {
                    list = profil.weaponSet;
                    break;
                }
        }

        return list;
    }

    
    public static void SendToOtherInventory(Transform startSlot)
    {
        // get new inventory
        Inventory oldInventory = startSlot.GetComponentInParent<Inventory>();
        Inventory newInventory = startSlot.GetComponentInParent<InventoryConnector>().GetOtherInventory(oldInventory);

        // inventory is open?
        if (newInventory.isActiveAndEnabled)
        {
            // get new slot
            Transform newSlot = null;
            int moveType = 0;

            // new slot found
            if (newInventory.FindSlotForItem(startSlot, ref newSlot, ref moveType))
            {
                // move item
                MoveItems(startSlot, newSlot, moveType);
            }
        }
        // inventory closed
        else
        {
            Debug.Log(oldInventory.name + ", " + newInventory.name);

            Transform itemObject = startSlot.GetChild(0);
            oldInventory.WithdrawItemFromSlot(startSlot);
            newInventory.AddToStorage(itemObject.GetComponent<ItemOnObject>().GetStoredItem);

            // destroy uiElement
            Destroy(itemObject.gameObject);
        }

    }


    public static int ItemMatch(Transform startSlot, Transform targetSlot)
    {
        SlotInfo targetSlotInfo = targetSlot.GetComponent<SlotInfo>();
        SlotInfo startSlotInfo = startSlot.GetComponent<SlotInfo>();
        
        // slotinfo not accessable
        if (targetSlotInfo == null)
        {
            // Debug.Log("unexepted situation for typematch no targetslot available");
            return -1;
        }

        // item over startslot
        if(startSlot == targetSlot)
        {
            return 0;
        }

        // targetset need test
        if (targetSlotInfo.testType)
        {
            // test dragged item
            if (!TypeMatch(targetSlotInfo, startSlot.GetComponentInChildren<ItemOnObject>().GetStoredItem))
            {
                // reset item
                Debug.LogWarning("dragged item is invalid");
                return 0;
            }
        }

        // startset need test
        if (startSlotInfo.testType)
        {
            // slot is used
            if (targetSlot.transform.childCount > 0)
            {
                // test current item in targetslot
                if (!TypeMatch(startSlotInfo, targetSlot.GetComponentInChildren<ItemOnObject>().GetStoredItem))
                {
                    // reset item
                    Debug.LogWarning("item in targetSlot is invalid");
                    return 0;
                }
            }
        }

        Inventory targetInventory = targetSlot.GetComponentInParent<Inventory>();
        Inventory startInventory = startSlot.GetComponentInParent<Inventory>();

        if (startInventory != targetInventory)
        {
            // test targetInventory
            if (!targetInventory.PreSetItem(startSlot.GetComponentInChildren<ItemOnObject>().GetStoredItem))
            {
                return 0;
            }
            
            // test startInventory
            if (!startInventory.PreWithdrawItem(startSlot.GetComponentInChildren<ItemOnObject>().GetStoredItem, targetInventory))
            {
                return 0;
            }
        }

        // targetslot is used
        if (targetSlot.transform.childCount > 0)
        {
            // switch items
            return 2;
        }
        // targetslot is not used
        else
        {
            // set item
            return 1;
        }
    }


    static bool TypeMatch(SlotInfo slotInfo, Item item)
    {
        // item is from different type
        // Debug.Log("Slottype: " + slotInfo.allowedItem.GetType() + ", ItemType: " + item.GetType());
        if (item.GetType() != slotInfo.allowedItem.GetType())
        {
            return false;
        }

        // subtype need to be proved
        if (slotInfo.testSubtype)
        {
            // item is from different subtype
            // Debug.Log("SlotSubtype: " + slotInfo.allowedItem.itemSubtype + ", ItemSubtype: " + item.itemSubtype);
            if (item.itemSubtype != slotInfo.allowedItem.itemSubtype)
            {
                // proved
                return false;
            }
        }
        // itemHand need to be proved
        if (slotInfo.testHand)
        {
            HandInfo testItemHand = (item as Usable).handInfo;
            HandInfo allowedItemHand = (slotInfo.allowedItem as Usable).handInfo;

            // item uses different hand
            if(testItemHand.preferedHand != allowedItemHand.preferedHand && !testItemHand.bothHands)
            {
                return false;
            }
        }

        // proved
        return true;
    }


    public static void MoveItems(Transform startSlot, Transform targetSlot, int moveType)
    {
        /**
         *  0 -> reset item 
         *  1 -> set Item
         *  2 -> switch item
         *  3 -> add to stack
        **/
        // Debug.Log(startSlot.GetComponentInParent<Inventory>().gameObject.name + ", " + targetSlot.GetComponentInParent<Inventory>().gameObject.name);

        switch (moveType)
        {
            // reset item
            case 0:
                {
                    // Debug.Log("reset item");

                    // reset alpha value
                    startSlot.GetChild(0).GetComponent<CanvasGroup>().alpha = 1;
                    break;
                }
            // set item
            case 1:
                {
                    // Debug.Log("set item");

                    Transform item = startSlot.GetChild(0);
                    
                    // reset alpha value
                    item.GetComponent<CanvasGroup>().alpha = 1;

                    // set item to targetSlot
                    startSlot.GetComponentInParent<Inventory>().WithdrawItemFromSlot(startSlot);
                    targetSlot.GetComponentInParent<Inventory>().SetItem(targetSlot, item);
                    break;
                }
            // switch items
            case 2:
                {
                    // Debug.Log("switch items");

                    Transform startItem = startSlot.GetChild(0);
                    Transform targetItem = targetSlot.GetChild(0);

                    // reset alpha value
                    startItem.GetComponent<CanvasGroup>().alpha = 1;
                    
                    Inventory targetInventory = targetSlot.GetComponentInParent<Inventory>();
                    Inventory startInventory = startSlot.GetComponentInParent<Inventory>();

                    startInventory.WithdrawItemFromSlot(startSlot);
                    targetInventory.WithdrawItemFromSlot(targetSlot);

                    // set startItem to targetSlot
                    targetInventory.SetItem(targetSlot, startItem);

                    // set targetItem to startSlot
                    startInventory.SetItem(startSlot, targetItem);
                    break;
                }
            // add to stack
            case 3:
                {
                    // Debug.Log("add to itemStack");

                    Transform startItem = startSlot.GetChild(0);
                    Transform targetItem = targetSlot.GetChild(0);

                    // reset alpha value
                    startItem.GetComponent<CanvasGroup>().alpha = 1;

                    // update targetStack
                    
                    // delete startItem

                    break;
                }
            default:
                {
                    Debug.LogError("unexpected draggingStatus: " + moveType);
                    break;
                }

        }
    }
}
