using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InventoryConnector : MonoBehaviour {

    [SerializeField]
    private List<Inventory> openInventories = new List<Inventory>();
    [SerializeField]
    private Inventory mainInventory;

    void Awake()
    {
        mainInventory = GetComponentInChildren<PlayerInventory>();
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void AddInventory(Inventory inventory)
    {
        if (openInventories.Contains(inventory))
        {   
            // inventory still in list
            return;
        }
        else
        {
            // add new inventory
            openInventories.Add(inventory);
        }
    }

    public void RemoveInventory(Inventory inventory)
    {
        // remove inventory
        openInventories.Remove(inventory);
    }
    
    public Inventory GetOtherInventory(Inventory current)
    {
        for(int i = 0; i < openInventories.Count; i++)
        {
            if(current != openInventories[i])
            {
                return openInventories[i];
            }
        }

        if(current != mainInventory)
        {
            return mainInventory;
        }

        return null;
    }
}
