using UnityEngine;
using System.Collections;

[System.Serializable]
public class Trait
{
    public string name;
    public int id;
    public string description;
    public Sprite icon;
    public int pointsToLearn;
    public int line;
    public Trait extends;
    public float posXPercent;
    public int maxLevel = 5;
    public int curLevel = 0;

    public enum TraitLine
    {
        Sword,
        Onehanded,
        Bow,
        Shield,
        Sneak,
        FireMagic

    }
    public Trait(string name, int id, string description,float posXPercent, int pointsRequired, int line)
    {
        this.name = name;
        this.id = id;
        this.description = description;
        this.posXPercent = posXPercent;
        this.icon = Resources.Load<Sprite>("TraitIcons/" + name);
        this.pointsToLearn = pointsRequired;
        this.line = line;
    }

    public Trait(string name, int id, string description, int pointsRequired, Trait extends)
    {
        this.name = name;
        this.id = id;
        this.description = description;
        this.posXPercent = extends.posXPercent;
        this.icon = Resources.Load<Sprite>("TraitIcons/" + name);
        this.extends = extends;
        this.pointsToLearn = pointsRequired;
        this.line = extends.line;
    }
}
