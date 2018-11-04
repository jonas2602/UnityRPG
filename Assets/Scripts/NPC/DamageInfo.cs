using UnityEngine;
using System.Collections;

[System.Serializable]
public class DamageInfo
{
    public Item causedItem;
    public Item damagedItem;
    public Vector3 direction;
    public DamageType damageType;
    

    public DamageInfo(Item causedItem, Item damagedItem, Vector3 direction, DamageType damageType)
    {
        this.causedItem = causedItem;
        this.damagedItem = damagedItem;
        this.direction = direction;
        this.damageType = damageType;
    }
}


public enum DamageType
{
    Projectile,
    Strike,
    Strike_Heavy,
    Kick,
    Explosion
}
/*
public class StrikeDamageInfo : DamageInfo
{

}*/


public class ProjectileDamageInfo : DamageInfo
{
    public float speed;
    public Collision collision;

    public ProjectileDamageInfo(Item causedItem, Item damagedItem, Vector3 direction, Collision collision, DamageType damageType, float speed)
        : base(causedItem, damagedItem, direction, damageType)
    {
        this.collision = collision;
        this.speed = speed;
    }

}
