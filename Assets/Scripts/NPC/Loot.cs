using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Loot : MonoBehaviour
{

    public List<Item> lootItems;
    public GameObject bag;
    public List<Item> lootPool;

    public void Init(string name, Vector3 pos, List<Item> lootPool)
    {
        bag = this.gameObject;

        bag.name = name;
        bag.tag = "Loot";
        bag.transform.position = pos;

        this.lootPool = lootPool;

        CalculateItems();
    }

    void CalculateItems()
    {
        lootItems = new List<Item>();
        for (int i = 0; i < lootPool.Count; i++)
        {
            if (Random.value > 0.75)
            {
                lootItems.Add(lootPool[i]);
            }
        }
    }
}
