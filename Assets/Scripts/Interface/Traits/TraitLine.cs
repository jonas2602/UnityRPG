using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TraitLine
{
    public string name;
    public int id;
    public int level;
    public float levelProgress;
    public Sprite image;
    public int points;
    public List<Trait> lineTraits = new List<Trait>();

    public TraitLine(string name, int id)
    {
        this.name = name;
        this.id = id;
        this.points = 0;
    }

    public TraitLine(string name, int id, int points)
    {
        this.name = name;
        this.id = id;
        this.points = points;
    }
}
