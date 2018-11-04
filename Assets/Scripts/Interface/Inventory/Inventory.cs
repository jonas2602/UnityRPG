using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public abstract class Inventory : UIWindow
{
    // all time references
    private ItemList itemDatabase;
    [SerializeField]
    private GameObject slotPrefab;
    [SerializeField]
    private GameObject itemPrefab;
    [SerializeField]
    protected Transform containerParent;
    
    // rarely changed
    protected ItemStorage itemStorage;
    protected InventoryConnector connector;

    // changed at everyUse
    [SerializeField]
    protected int slotCount;
    [SerializeField]
    protected int minSlotCount;
    [SerializeField]
    protected int columnCount;

    public bool isMainInventory = false;
    [SerializeField]
    protected List<Item> itemsInInventory = new List<Item>();
    [SerializeField]
    protected InventoryMatrix inventoryMatrix;
    [SerializeField]
    protected List<Transform> slotContainer;


    public Transform GetContainerParent
    {
        get
        {
            return this.containerParent;
        }
    }
    
    protected virtual void SetupInventory(GameObject storageObject)
    {
        this.itemStorage = storageObject.GetComponent<ItemStorage>();
        this.inventoryMatrix = new InventoryMatrix();
    }

    protected virtual void Awake()
    {
        itemDatabase = GameObject.FindWithTag("Database").GetComponent<ItemList>();
        connector = transform.root.GetComponent<InventoryConnector>();

        if (slotPrefab == null)
        {
            Debug.LogError("slotPrefab not setup");
        }
        if (itemPrefab == null)
        {
            Debug.LogError("itemPrefab not setup");
        }
    }


    protected virtual void Start()
    {
        AddStartSlots();
    }


    protected abstract void AddStartSlots();


    protected virtual List<Item> LoadItemList()
    {
        return itemStorage.GetInventoryItemList;;
    }
    
    protected virtual void LoadFromItemStorage()
    {
        // load itemList
        itemsInInventory = LoadItemList();

        // get inventory sizes
        GridLayoutGroup grid = containerParent.GetComponent<GridLayoutGroup>();
        Rect rect = containerParent.GetComponent<RectTransform>().rect;
        columnCount = grid.constraintCount;
        int minRowCount = (int)Mathf.Ceil(rect.height / grid.cellSize.y);
        minSlotCount = minRowCount * columnCount;

        // add slots
        Transform parent = containerParent;
        int newSlotsCount = Mathf.Max(minSlotCount, itemsInInventory.Count);
        // Debug.Log("Add " + newSlotsCount + " Slots to " + containerParent);

        for (int i = 0; i < newSlotsCount; i++)
        {
            string name = "slot" + i;
            AddSlot(name, parent, null, false, false, false);
        }
        slotCount += newSlotsCount;
        
        // load items
        for (int i = 0; i < itemsInInventory.Count; i++)
        {
            // get item
            Item item = itemsInInventory[i];

            // get position
            int position = inventoryMatrix.GetPositionOfItem(item);
            
            // create itemObject
            Transform itemObject = Instantiate(itemPrefab).transform;
            itemObject.SetParent(slotContainer[position]);
            itemObject.GetComponent<ItemOnObject>().SetupItem(item);

            // update matrix
            inventoryMatrix.UpdatePosition(position, item);
        }
    }


    protected virtual void ResetInventory()
    {
        for(int i = 0; i < slotContainer.Count;i++)
        {
            Destroy(slotContainer[i].gameObject);
        }

        slotContainer.Clear();
        slotCount = 0;
    }

    public virtual bool PreSetItem(Item item)
    {
        return true;
    }
    
    // Move item to inventory
    public virtual void SetItem(Transform targetSlot, Transform itemObject)
    {
        // move to targetSlot
        itemObject.SetParent(targetSlot);
        itemObject.localPosition = Vector3.zero;

        SlotInfo slotInfo = targetSlot.GetComponent<SlotInfo>();
        // has connected slot?
        if (slotInfo.connectedSlot)
        {
            // Debug.Log("has connected slot");

            // test connected slot
            if (!slotInfo.SetItemToSlot())
            {
                // Debug.Log("tested slot dont fit");

                ItemManager.SendToOtherInventory(slotInfo.connectedSlot.transform);
            }
        }
        // update itemList
        UpdateInventoryMatrix(targetSlot.GetSiblingIndex());
    }

    public virtual bool PreWithdrawItem(Item item, Inventory targetInventory)
    {
        return true;
    }

    // withdraw item from inventory
    public virtual void WithdrawItemFromSlot(Transform slot)
    {
        // remove item from slot
        slot.GetChild(0).SetParent(null);

        SlotInfo slotInfo = slot.GetComponent<SlotInfo>();
        // has connected slot?
        if (slotInfo.connectedSlot)
        {
            // Debug.Log("has connected slot");

            // test connected slot
            if (!slotInfo.WithdrawItemFromSlot())
            {
                // Debug.Log("tested slot dont fit");

                ItemManager.SendToOtherInventory(slotInfo.connectedSlot.transform);
            }
        }

        // update itemList
        UpdateInventoryMatrix(slot.GetSiblingIndex());
    }
    
    public bool HasEnoughMoneyForItem(Item item)
    {
        Item money = itemStorage.GetMoney;

        Debug.Log(item.itemName + " costs: " + item.itemAmount * item.itemPrice + ", inventory contains: " + money.itemAmount + " Coins");
        if (money.itemAmount >= item.itemAmount * item.itemPrice)
        {
            // has enough money
            return true;
        }
        else
        {
            // has not enough money
            return false;
        }
    }

    public void AddMoney(int amount)
    {
        itemStorage.AddMoneyToInventory(amount);
    }

    public void WithdrawMoney(int amount)
    {
        itemStorage.WithdrawMoneyFromInventory(amount);
    }

    public virtual bool FindSlotForItem(Transform startSlot, ref Transform targetSlot, ref int moveType)
    {
        // get slot
        int freePos = inventoryMatrix.GetFirstFreePosition();
        targetSlot = slotContainer[freePos];

        moveType = 1;

        return true;
    }


    public virtual void GetReferenceItems(Item item)
    {

    }


    protected SlotInfo AddSlot(string name, Transform parent, Item allowedItem, bool testType, bool testSubtype, bool testHand)
    {
        // create slotGameobject
        Transform slot = Instantiate(slotPrefab).transform;
        slot.name = name;
        slot.transform.SetParent(parent);

        // add to inventory script
        slotContainer.Add(slot);

        // set slotVariables variables
        SlotInfo slotInfo = slot.GetComponent<SlotInfo>();
        slotInfo.SetupSlotInfo(allowedItem, testType, testSubtype, testHand);

        return slotInfo;
    }

    // extends AddSlot
    protected SlotInfo AddSlot(string name, Transform parent, int equipId, Item allowedItem, bool testType, bool testSubtype, bool testHand)
    {
        // Add standart slot
        SlotInfo slotInfo = AddSlot(name, parent, allowedItem, testType, testSubtype, testHand);
        
        // set additional slotVariables variables
        slotInfo.equipId = equipId;

        return slotInfo;
    }

    protected SlotInfo UpdateSlot(int equipId, Transform slot, Item allowedItem, bool testType, bool testSubtype, bool testHand)
    {
        // add to inventory script
        slotContainer.Add(slot);

        // set slotVariables variables
        SlotInfo slotInfo = slot.GetComponent<SlotInfo>();
        slotInfo.SetupSlotInfo(allowedItem, testType, testSubtype, testHand);
        slotInfo.equipId = equipId;
        return slotInfo;
    }

    protected SlotInfo UpdateSlot(int equipId, Transform slot, SlotInfo connectedSlot, Item allowedItem, bool testType, bool testSubtype, bool testHand)
    {
        SlotInfo slotInfo = UpdateSlot(equipId, slot, allowedItem, testType, testSubtype, testHand);

        slot.GetComponent<SlotInfo>().connectedSlot = connectedSlot;

        return slotInfo;
    }

    public void AddItem(Transform itemObject)
    {
        // go through slots
        for (int i = 0; i < slotContainer.Count; i++)
        {
            // find first free slot
            if (slotContainer[i].childCount == 0)
            {
                itemObject.SetParent(slotContainer[i]);

                // update itemlist
                UpdateInventoryMatrix(i);
                return;
            }
        }
    }

    public void AddToStorage(Item item)
    {
        itemStorage.AddItemToInventory(item);
    }


    protected void UpdateInventoryMatrix(int slotId)
    {
        // extend matrix up to the current slot
        inventoryMatrix.ExtendMatrixToCount(slotId);
        
        // get item from ui
        Item newItem;
        Transform slot = slotContainer[slotId];
        if (slot.childCount > 0)
        {
            newItem = slot.GetChild(0).GetComponent<ItemOnObject>().GetStoredItem;

            // add item to slot
            itemStorage.AddItemToInventory(newItem);
        }
        else
        {
            // remove item from slot
            itemStorage.WithdrawItemFromInventory(inventoryMatrix.GetItemOfPosition(slotId));

            newItem = new Item();
        }
        
        // update matrix
        inventoryMatrix.UpdatePosition(slotId, newItem);
        
        // update slotCount
        /*
        int lastSetPosition = inventoryMatrix.GetHeighestPosition();
        int lastRow = (int)Mathf.Ceil(lastSetPosition / columnCount);
        Debug.Log(lastSetPosition + ", " + lastRow);*/
    }


    public override void OpenWindow()
    {
        // load items from itemStorage
        LoadFromItemStorage();

        // add to open inventories
        connector.AddInventory(this);

        base.OpenWindow();
    }


    public override void CloseWindow()
    {
        // delete slots
        ResetInventory();

        // remove from open inentories
        connector.RemoveInventory(this);

        base.CloseWindow();
    }
}

[System.Serializable]
public class InventoryMatrix
{
    [SerializeField]
    private List<Item> matrix;
    
    public InventoryMatrix()
    {
        matrix = new List<Item>();
    }


    public int GetHeighestPosition()
    {
        for (int i = matrix.Count - 1; i >= 0; i--)
        {
            if (matrix[i].itemID != -1)
            {
                return i;
            }
        }
        return -1;
    }

    public int GetPositionOfItem(Item item)
    {
        // matrix contains item
        if (matrix.Contains(item))
        {
            // find slot
            for (int i = 0; i < matrix.Count; i++)
            {
                // searched item is in this slot
                if (matrix[i] == item)
                {
                    return i;
                }
            }
        }

        // find new position
        int itemPos = GetFirstFreePosition();

        // extend list until free slot
        ExtendMatrixToCount(itemPos);

        // return positon of item
        return itemPos;
    }

    public Item GetItemOfPosition(int id)
    {
        return matrix[id];
    }

    public int GetFirstFreePosition()
    {
        for (int i = 0; i < matrix.Count; i++)
        {
            // slot is free
            if (matrix[i].itemID == -1)
            {
                return i;
            }
        }

        // no free slot found -> return new slot
        return matrix.Count;
    }

    public void UpdatePosition(int position, Item item)
    {
        matrix[position] = item;
    }

    public void ExtendMatrixToCount(int id)
    {
        while(matrix.Count <= id)
        {
            matrix.Add(new Item());
        }
    }
}