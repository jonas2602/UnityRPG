using UnityEngine;
using System.Collections;

public class PlayerInventory : Inventory
{
    protected override void Awake()
    {
        containerParent = this.transform;
        isMainInventory = true;
        base.Awake();
    }

    // Use this for initialization
    protected override void Start ()
    {
        base.Start();
    }

    public override void SetupPlayer(GameObject player)
    {
        SetupInventory(player);
    }

    protected override void AddStartSlots()
    {

    }
}
