using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Actor
{
    public string id;
    public string name;
    public bool male;
    public bool unique;
    public Race race;
    public List<Faction> factions;
    public List<Relation> relationships;
     
    // public GameObject prefab;
    // public List<GameObject> sceenReferences;

    public Actor()
    {
        this.id = "id";
        this.name = "New Actor";
    }
}

public enum Race
{
    Human,
    Animal,
    Monster
}
