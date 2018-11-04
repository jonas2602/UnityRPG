using UnityEngine;
using System.Collections;

[System.Serializable]
public class Blueprint
{
    public Item[] resources;
    public Item result;
    public CraftsmanLevel requiredLevel;

    public Blueprint(Item[] resources, Item result)
    {
        this.resources = resources;
        this.result = result;
    }
    
}

public enum CraftsmanLevel
{

}
