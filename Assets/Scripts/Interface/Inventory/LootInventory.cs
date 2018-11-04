using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class LootInventory : Inventory
{
    GameObject player;

    protected override void AddStartSlots()
    {
        
    }

    public override void SetupPlayer(GameObject player)
    {
        this.player = player;
    }

    protected override List<Item> LoadItemList()
    {
        return itemStorage.GetLoot();
    }

    protected override void Awake()
    {
        containerParent = transform.GetChild(0);
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
	    if(Input.GetButtonDown("Jump"))
        {
            TakeAll();
        }
	}

    public override void OpenWindow()
    {
        SetupInventory(player.GetComponent<PlayerAttributes>().target);

        base.OpenWindow();
    }
    
    public void TakeAll()
    {
        // move all items to inventory
        for (int i = 0; i < slotContainer.Count; i++)
        {
            if (slotContainer[i].GetComponentInChildren<ItemOnObject>())
            {
                // move item to player inventory
                ItemManager.SendToOtherInventory(slotContainer[i]);

                // remove from this inventory
                Destroy(slotContainer[i].GetComponentInChildren<ItemOnObject>().gameObject);

            }
        }
        // close window
        GetComponentInParent<UIManager>().ExitLootWindow();
    }
}
