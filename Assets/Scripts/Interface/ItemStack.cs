using UnityEngine;
using System.Collections;


[System.Serializable]
public class ItemStack
{
    public Item slotItem;
    public int slotAmount;

    public ItemStack(Item item,int amount)
    {
        slotItem = item;
        slotAmount = amount;
    }

    public ItemStack()
    {

    }

}
