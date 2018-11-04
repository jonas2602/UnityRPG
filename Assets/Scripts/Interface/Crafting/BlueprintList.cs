using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlueprintList : MonoBehaviour {

    private ItemList itemDatabase;
    [SerializeField]
    private List<Blueprint> blueprints = new List<Blueprint>();
    
    void Awake()
    {
        itemDatabase = GetComponent<ItemList>();
    }

	void Start ()
    {
        blueprints.Add(CreateBlueprint(new string[] { "Sword", "Torch" }, "Claymore"));
	}

    Blueprint CreateBlueprint(string[] resources, string result)
    {
        Item[] resourceItems = new Item[resources.Length];
        for (int i = 0; i < resources.Length; i++)
        {
            resourceItems[i] = itemDatabase.GetItemByName(result);
        }
        Item resultItem = itemDatabase.GetItemByName(result);

        return new Blueprint(resourceItems, resultItem);
    }
}
