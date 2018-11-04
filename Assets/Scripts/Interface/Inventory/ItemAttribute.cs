using UnityEngine;
using System.Collections;

[System.Serializable]
public class ItemAttribute
{
    public AttributeList attributeName;
    public int attributeValue;

    public ItemAttribute(AttributeList attributeName, int attributeValue)
    {
        this.attributeName = attributeName;
        this.attributeValue = attributeValue;
    }

    public ItemAttribute()
    {

    }
}

public enum AttributeList
{
    BladeDamage,
    StitchDamage,
    SmashDamage,
    AttackSpeed,
    Protection
}


