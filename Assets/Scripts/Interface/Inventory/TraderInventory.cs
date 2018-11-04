using UnityEngine;
using System.Collections;
using System;

public class TraderInventory : Inventory
{
    GameObject player;

    public override void SetupPlayer(GameObject player)
    {
        this.player = player;
    }

    protected override void Awake()
    {
        containerParent = this.transform;
        base.Awake();
    }

    // Use this for initialization
    protected override void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    protected override void AddStartSlots()
    {
        // throw new NotImplementedException();
    }

    public override bool PreSetItem(Item item)
    {
        // test if trader has enough money to buy item
        return HasEnoughMoneyForItem(item);
    }

    public override void SetItem(Transform targetSlot, Transform itemObject)
    {
        base.SetItem(targetSlot, itemObject);

        Item tradeItem = itemObject.GetComponent<ItemOnObject>().GetStoredItem;
        int moneyAmount = tradeItem.itemAmount * tradeItem.itemPrice;

        // withdraw money from traderInventory
        WithdrawMoney(moneyAmount);

        // send money back to player
        connector.GetOtherInventory(this).AddMoney(moneyAmount);        
    }

    public override bool PreWithdrawItem(Item item, Inventory targetInventory)
    {
        // test if player has enough money to buy item
        return targetInventory.HasEnoughMoneyForItem(item);
    }

    public override void WithdrawItemFromSlot(Transform slot)
    {
        Item tradeItem = slot.GetComponentInChildren<ItemOnObject>().GetStoredItem;
        int moneyAmount = tradeItem.itemAmount * tradeItem.itemPrice;

        // get money from player
        connector.GetOtherInventory(this).WithdrawMoney(moneyAmount);

        // store money in traderInventory
        AddMoney(moneyAmount);

        base.WithdrawItemFromSlot(slot);
    }

    public override void OpenWindow()
    {
        SetupInventory(player.GetComponent<PlayerAttributes>().target);

        base.OpenWindow();
    }
}
