using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Item
{
    public string itemName;
    public int itemID;
    public string itemDesc;
    public Sprite itemIcon;
    public ItemType itemType;
    public ItemSubtype itemSubtype;
    public int requiredLevel = 1;
    public Rarity itemRarity;
    public int itemAmount = 1;
    public bool itemStackable = false;
    public float itemWeight;
    public int itemPrice;
    public GameObject itemMesh;

    public List<ItemAttribute> itemAttributes;
    // public List<Upgrade> itemUpgrades;
    

    public enum Rarity
    {
        Basic,
        Fine,
        Masterwork,
        Rare,
        Exotic,
        Legendary
    }

    // item
    public Item(string name, int id, string desc, ItemType type, ItemSubtype subtype, Rarity rarity, bool stackable, float weight, int price, List<ItemAttribute> attributes)
    {
        itemName = name;
        itemID = id;
        itemDesc = desc;
        itemIcon = Resources.Load<Sprite>("ItemIcons/" + name);
        itemType = type;
        itemSubtype = subtype;
        itemRarity = rarity;
        itemStackable = stackable;
        itemWeight = weight;
        itemPrice = price;
        itemAttributes = attributes;
        itemMesh = Resources.Load<GameObject>(type.ToString() + "/" + name);
    }

    public Item(string name, ItemType type)
    {
        itemName = name;
        itemType = type;
        itemMesh = Resources.Load<GameObject>(type.ToString() + "/" + name);
    }

    public Item(ItemType type, ItemSubtype subtype)
    {
        itemType = type;
        itemSubtype = subtype;
    }

    public Item(ItemType type)
    {
        itemType = type;
    }
    
    public Item ()
    {
        itemID = -1;
        itemAmount = 0;
    }


    public Item GetCopy()
    {
        return this.MemberwiseClone() as Item;
    }
}
public enum ItemType
{
    Weapon,
    RangedWeapon,
    MeleeWeapon,
    Tool,
    Armor,
    EquipAddition,
    // WeaponCase,
    Consumable,
    Munition,
    Upgrade
}
public enum ItemSubtype
{
    // ArmorType
    Helm,               // 0
    Harnisch,           // 1
    Handschuhe,         // 2
    Hose,               // 3
    Schuhe,             // 4
    
    // WeaponType
    Schwert,            // 5
    Axt,                // 6
    Dolch,              // 7
    Schild,             // 8
    Bogen,              // 9
    Speer,              // 10
    Stab,               // 11
    Großschwert,        // 12
    
    // ConsumableType
    Nahrung,            // 13
    Trank,              // 14
    Geld,               // 15
    
    // MunitionType
    Arrow,              // 16
    Bolt,               // 17
    Bomb,               // 18
    Wurfmesser,         // 19

    // UpgradeType
    Rune,               // 20
    Glyphe,             // 21

    Tool                // 22
}

[System.Serializable]
public class Usable : Item
{                                                                             // Sword:      Both         Right
    public HandInfo handInfo;           // public ItemHand[] itemHand = new ItemHand[2] { posibilities , prefered };
    public Vector3 handParentPosition;
    public Vector3 handParentRotation;

    
    public Usable(string name, int id, string desc, ItemType type, ItemSubtype subtype, Rarity rarity, bool stackable, float weight, int price, List<ItemAttribute> attributes, HandInfo handInfo, Vector3 handParentPosition, Vector3 handParentRotation)
        : base(name, id, desc, type, subtype, rarity, stackable, weight, price, attributes)
    {
        this.handInfo = handInfo;
        this.handParentPosition = handParentPosition;
        this.handParentRotation = handParentRotation;
    }

    public Usable(ItemType type, ItemSubtype subtype)
        : base(type,subtype)
    {

    }

    public Usable(ItemType type)
        : base(type)
    {

    }

    public Usable(HandInfo handInfo)
        : base()
    {
        this.handInfo = handInfo;
    }

    public Usable()
        : base()
    {

    }
}

[System.Serializable]
public class HandInfo
{
    public ItemHand preferedHand;
    public bool twoHanded;             // two hands needed to use
    public bool bothHands;            // able to use in both hands

    public HandInfo(ItemHand preferedHand, bool bothHands,bool twoHanded)
    {
        this.preferedHand = preferedHand;
        this.bothHands = bothHands;
        this.twoHanded = twoHanded;
    }

    public HandInfo(ItemHand preferedHand)
    {
        this.preferedHand = preferedHand;
        this.bothHands = false;
        this.twoHanded = false;
    }
}
public enum ItemHand
{
    Left,
    Right
}


[System.Serializable]
public class Weapon : Usable
{
    /* klingenschaden, stichschaden, schmetterschaden/hiebschaden, angriffsgeschw, krit, kritchance, reichweite, elementarschaden,
       rüstungsdurchdringung,adrenalingewinn, skillverstärkung, preis, zustand(kaputt), runenslots,slottype*/


    // public float attackSpeed; // abhängig vom gewicht

    public int runeSlots;
    // public float zustand;

    public WeaponType weaponType;
    
    public Vector3 weaponUnusedParentPosition;
    public Vector3 weaponUnusedParentRotation;
    public string weaponUnusedParentPath;
    public string[] additionalGameObjectNames;


    public Weapon(string name, int id, string desc, float weight, int price, List<ItemAttribute> attributes, HandInfo weaponHandInfo, Vector3 weaponHandParentPosition, Vector3 weaponHandParentRotation, int runeslots, ItemSubtype subtype, Rarity rarity, Vector3 weaponUnusedParentPosition, Vector3 weaponUnusedParentRotation, string weaponUnusedParentPath, string[] additionalGameObjectNames)
        : base(name, id, desc, ItemType.Weapon, subtype, rarity, false, weight, price, attributes, weaponHandInfo, weaponHandParentPosition, weaponHandParentRotation)
    {
        this.runeSlots = runeslots;
        // this.weaponType = weaponType;
        this.weaponUnusedParentPosition = weaponUnusedParentPosition;
        this.weaponUnusedParentRotation = weaponUnusedParentRotation;
        this.weaponUnusedParentPath = weaponUnusedParentPath;
        this.additionalGameObjectNames = additionalGameObjectNames;
    }

    public Weapon(HandInfo weaponHandInfo)
        : base(weaponHandInfo)
    {

    }

    public Weapon(ItemSubtype subtype)
        : base(ItemType.Weapon, subtype)
    {

    }

    public Weapon()
        : base()
    {

    }
}
public enum WeaponType
{
    Schwert,
    Axt,
    Dolch,
    Wurfmesser,
    Schild,
    Bogen,
    Speer,
    Stab,
    Großschwert
}


[System.Serializable]
public class RangedWeapon : Weapon
{
    public ItemSubtype munitionType;
    public float maxRange;

    public RangedWeapon(string name, int id, string desc, float weight, int price, List<ItemAttribute> attributes, HandInfo weaponHandInfo, Vector3 weaponHandParentPosition, Vector3 weaponHandParentRotation, int runeslots, ItemSubtype subtype, Rarity rarity, Vector3 weaponUnusedParentPosition, Vector3 weaponUnusedParentRotation, string weaponUnusedParentPath, string[] additionalGameObjectNames, ItemSubtype munitionType)
        : base(name, id, desc/*, ItemType.RangedWeapon*/, weight, price, attributes, weaponHandInfo, weaponHandParentPosition, weaponHandParentRotation, runeslots, subtype, rarity, weaponUnusedParentPosition, weaponUnusedParentRotation, weaponUnusedParentPath, additionalGameObjectNames)
    {
        this.munitionType = munitionType;
    }

    public RangedWeapon(ItemSubtype munitionType)
        : base()
    {
        this.munitionType = munitionType;
    }

    public RangedWeapon()
        : base()
    {

    }
}


[System.Serializable]
public class MeleeWeapon : Weapon
{

    public MeleeWeapon(string name, int id, string desc, float weight, int price, List<ItemAttribute> attributes, HandInfo weaponHandInfo, Vector3 weaponHandParentPosition, Vector3 weaponHandParentRotation, int runeslots, ItemSubtype subtype, Rarity rarity, Vector3 weaponUnusedParentPosition, Vector3 weaponUnusedParentRotation, string weaponUnusedParentPath, string[] additionalGameObjectNames)
        : base(name, id, desc/*, ItemType.MeleeWeapon*/, weight, price, attributes, weaponHandInfo, weaponHandParentPosition, weaponHandParentRotation, runeslots, subtype, rarity, weaponUnusedParentPosition, weaponUnusedParentRotation, weaponUnusedParentPath, additionalGameObjectNames)
    {

    }

    public MeleeWeapon(HandInfo weaponHandInfo)
        : base(weaponHandInfo)
    {

    }

    public MeleeWeapon(ItemSubtype subtype)
        : base(subtype)
    {

    }

    public MeleeWeapon()
        : base()
    {

    }
}

[System.Serializable]
public class EquipAddition : Item
{
    public Vector3 boneParentPosition;
    public Vector3 boneParentRotation;
    public string boneParentPath;

    public EquipAddition(string name, Vector3 boneParentPosition, Vector3 boneParentRotation, string boneParentPath)
        :base(name, ItemType.EquipAddition)
    {
        this.boneParentPosition = boneParentPosition;
        this.boneParentRotation = boneParentRotation;
        this.boneParentPath = boneParentPath;
    }
}


[System.Serializable]
public class Armor : Item
{
    // weight , armor, block, ablenken, durchdringungsschutz, schadensresistenzen, elementarresistenzen, preis, zustand(kaputt), runenslots,slottype
    
    public ArmorType armorType;
    public int runeSlots;
    

    public Armor(string name, int id, string desc, float weight, int price, List<ItemAttribute> attributes, ItemSubtype subtype, Rarity rarity, int runeSlots)
        : base(name, id, desc, ItemType.Armor, subtype, rarity, false, weight, price, attributes)
    {
        this.itemWeight = weight;
        // this.armorType = armorType;
        this.runeSlots = runeSlots;
    }

    public Armor(ItemSubtype subtype)
        : base(ItemType.Armor, subtype)
    {

    }
}
public enum ArmorType
{
    Helm,
    Harnisch,
    Handschuhe,
    Hose,
    Schuhe
}


[System.Serializable]
public class Consumable : Item
{
    public Consumable(string name, int id, string desc, ItemSubtype subtype, Rarity rarity, float weight, int price, List<ItemAttribute> attributes)
        : base(name, id, desc, ItemType.Consumable,subtype, rarity, true, weight, price, attributes)
    {

    }

    public Consumable()
        :base(ItemType.Consumable)
    {

    }
}


[System.Serializable]
public class Munition : Usable
{
    /* bonusschaden, effekte(feuer, gift, ...) */

    // public MunitionType munitionType;
    // public GameObject container;

    public Munition(string name, int id, string desc, ItemSubtype subtype, Rarity rarity, float weight, int price, List<ItemAttribute> attributes, HandInfo weaponHandInfo, Vector3 handParentPosition, Vector3 handParentRotation/*, GameObject container*/)
        : base(name, id, desc, ItemType.Munition, subtype, rarity, true, weight, price, attributes, weaponHandInfo, handParentPosition, handParentRotation)
    {
        // this.container = container;
    }

    public Munition(ItemSubtype subtype)
        : base(ItemType.Munition,subtype)
    {

    }

    public Munition()
        : base()
    {
        
    }
}
public enum MunitionType
{
    Arrow,
    Bolt,
    Bomb
}


[System.Serializable]
public class Tool : Usable
{
    public Tool(string name, int id, string desc, ItemSubtype subtype, Rarity rarity, float weight, int price, List<ItemAttribute> attributes, HandInfo weaponHandInfo, Vector3 handParentPosition, Vector3 handParentRotation)
        : base(name, id, desc, ItemType.Tool, subtype, rarity, true, weight, price, attributes, weaponHandInfo, handParentPosition, handParentRotation)
    {
        
    }

    public Tool()
        :base(ItemType.Tool)
    {

    }
}


[System.Serializable]
public class Upgrade : Item
{
    public Upgrade(string name, int id, string desc, ItemSubtype subtype, Rarity rarity, float weight, int price, List<ItemAttribute> attributes)
        :base(name, id, desc, ItemType.Upgrade, subtype, rarity, true, weight, price, attributes)
    {

    }

    public Upgrade()
        :base()
    {

    }
}

