using UnityEngine;
using System.Collections;
using System;

public class PlayerProfil : Inventory
{
    public EquipmentManager equipmentManager;

    public override void SetupPlayer(GameObject player)
    {
        equipmentManager = player.GetComponent<EquipmentManager>();
        SetupInventory(player);
    }

    protected override void Awake()
    {
        containerParent = this.transform;
        base.Awake();
    }

    // Use this for initialization
    protected override void Start ()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update ()
    {

	}

    protected override void AddStartSlots()
    {
        Transform parent;

        parent = containerParent.Find("ArmorSet");
        // Add Armor Set (5)
        for (int i = 0; i < 5; i++)
        {
            Transform slot = parent.GetChild(i);
            Item allowedItem = new Armor((ItemSubtype)(i));
            UpdateSlot(i, slot, allowedItem, true, true, false);
        }

        int equipId = 0;
        parent = containerParent.Find("WeaponSet");

        // Melee Set (2)
        SlotInfo mainSlot = UpdateSlot(equipId, parent.GetChild(equipId), parent.GetChild(++equipId).GetComponent<SlotInfo>(), new MeleeWeapon(new HandInfo(ItemHand.Right)), true, false, true);
        SlotInfo offSlot = UpdateSlot(equipId, parent.GetChild(equipId++), new MeleeWeapon(new HandInfo(ItemHand.Left)), true, false, true);
        // SlotInfo mainSlot = AddSlot("MainWeaponSlot", parent, equipId++, new MeleeWeapon(ItemHand.Right), true, false, true);
        // SlotInfo offSlot = AddSlot("OffWeaponSlot", parent, equipId++, new MeleeWeapon(ItemHand.Left), true, false, true);
        offSlot.DeactivateSlot();
        // mainSlot.connectedSlot = offSlot;

        // Range Set (2)
        mainSlot = UpdateSlot(equipId, parent.GetChild(equipId), parent.GetChild(++equipId).GetComponent<SlotInfo>(), new RangedWeapon(), true, false, false);
        offSlot = UpdateSlot(equipId, parent.GetChild(equipId++), new Munition(), true, true, false);
        // mainSlot = AddSlot("RangedWeaponSlot", parent, equipId++, new RangedWeapon(), true, false, false);
        // offSlot = AddSlot("MunitionSlot", parent, equipId++, new Munition(), true, true, false);
        offSlot.DeactivateSlot();
        // mainSlot.connectedSlot = offSlot;

        // Small Set (1)
        UpdateSlot(equipId, parent.GetChild(equipId++), new MeleeWeapon(ItemSubtype.Dolch), true, true, false);
        // AddSlot("SmallWeaponSlot", parent, equipId++, new MeleeWeapon(ItemSubtype.Dolch), true, true, false);
        
        parent = containerParent.Find("ConsumableSet");
        int equipIdOffset = equipId;

        // Tool Set (2)
        for (int i = 0; i < 2; i++)
        {
            UpdateSlot(equipId, parent.GetChild(equipId++ - equipIdOffset), new Tool(), true, false, false);
            // AddSlot("ToolSlot" + i, parent, equipId++, new Tool(), true, false, false);
        }
        // bombs (2)
        for (int i = 0; i < 2; i++)
        {
            UpdateSlot(equipId, parent.GetChild(equipId++ - equipIdOffset), new Munition(ItemSubtype.Bomb), true, true, false);
            // AddSlot("BombSlot" + i, parent, equipId++, new Munition(ItemSubtype.Bomb), true, true, false);
        }
        // Consumable / food / tränke (2)
        for (int i = 0; i < 2; i++)
        {
            UpdateSlot(equipId, parent.GetChild(equipId++ - equipIdOffset), new Consumable(), true, false, false);
            // AddSlot("ConsumableSlot" + i, parent, equipId++, new Consumable(), true, false, false);
        }
    }

    protected override void LoadFromItemStorage()
    {
        // throw new NotImplementedException();
    }

    protected override void ResetInventory()
    {
        // throw new NotImplementedException();
    }


    public override void SetItem(Transform targetSlot, Transform itemObject)
    {
        // Debug.Log("set item to " + targetSlot);

        Item item = itemObject.GetComponent<ItemOnObject>().GetStoredItem;
        
        base.SetItem(targetSlot, itemObject);

        // Debug.Log("Equip: " + item.itemName);
        equipmentManager.Equip(item, targetSlot.GetComponent<SlotInfo>().equipId);
        
        // UpdateItemsInInventory();
    }


    public override void WithdrawItemFromSlot(Transform slot)
    {
        // Debug.Log("withdraw item from " + slot.name);

        // Debug.Log("Unequip: " + slot.GetComponentInChildren<ItemOnObject>().storedItem.itemName);
        equipmentManager.Unequip(slot.GetComponentInChildren<ItemOnObject>().GetStoredItem, slot.GetComponent<SlotInfo>().equipId);
        
        base.WithdrawItemFromSlot(slot);
    }

    public SlotInfo[] GetDrawableWeapons()
    {
        // get all weaponSlots
        Transform weaponParent = transform.Find("WeaponSet");
        SlotInfo[] allWeaponSlots = weaponParent.GetComponentsInChildren<SlotInfo>();

        SlotInfo[] mainWeaponSlots = new SlotInfo[Mathf.CeilToInt(allWeaponSlots.Length / 2.0f)];
        // sort out offWeapon
        int weaponSlotCounter = 0;
        for (int i = 0; i < allWeaponSlots.Length; i += 2)
        {
            // Debug.Log(allWeaponSlots.Length + ", " + i + ", " + mainWeaponSlots.Length + ", " + weaponSlotCounter);
            mainWeaponSlots[weaponSlotCounter] = allWeaponSlots[i];
            weaponSlotCounter++;
        }

        return mainWeaponSlots;
    }

    public SlotInfo[] GetDrawableTools()
    {
        Transform toolParent = transform.Find("ConsumableSet");
        SlotInfo[] toolSet = new SlotInfo[4];
        for(int i = 0; i< toolSet.Length;i++)
        {
            toolSet[i] = toolParent.GetChild(i).GetComponent<SlotInfo>();
        }

        return toolSet;
    }

    public override bool FindSlotForItem(Transform startSlot, ref Transform targetSlot, ref int moveType)
    {
        for (int i = 0; i < slotContainer.Count; i++)
        {
            // slot is active?
            if(!slotContainer[i].GetComponent<CanvasGroup>().blocksRaycasts)
            {
                continue;
            }

            moveType = ItemManager.ItemMatch(startSlot, slotContainer[i]);

            // allowed to set
            if (moveType > 0)
            {
                targetSlot = slotContainer[i];
                // Debug.Log("targetslot found: " + targetSlot.name);
                return true;
            }
        }

        return false;
    }
}
