using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemList : MonoBehaviour 
{
    public List<Item> items = new List<Item>();
    public List<Item> interactionTools = new List<Item>();
    public List<Item> equipAdditions = new List<Item>();
    public List<Item> currencies = new List<Item>();


    void Awake()
    {
        // create world items
        items.Add(new MeleeWeapon("Sword", 0, "A sword made of Silver", 1, 100, new List<ItemAttribute>() { new ItemAttribute(AttributeList.BladeDamage, 10), new ItemAttribute(AttributeList.StitchDamage, 10), new ItemAttribute(AttributeList.SmashDamage, 10) }, new HandInfo(ItemHand.Right,true,false), new Vector3(0f, 0.254f, 0f), new Vector3(270f, 270f, 0f), 2, ItemSubtype.Schwert, Item.Rarity.Basic, new Vector3(1.012f, 1.326f, -1.416f), new Vector3(0, 0, 0), "Bones/Bones|Steuerbone/Bones|Hüfte/Sword_Sheath", new string[] { "Sword_Sheath" }));
        items.Add(new MeleeWeapon("Schild", 1, "blablabla", 1, 100, new List<ItemAttribute>() { new ItemAttribute(AttributeList.BladeDamage, 10), new ItemAttribute(AttributeList.StitchDamage, 10), new ItemAttribute(AttributeList.SmashDamage, 10) }, new HandInfo(ItemHand.Left), new Vector3(0, 0, 0), new Vector3(0, 0, 0), 1, ItemSubtype.Schild, Item.Rarity.Basic, new Vector3(0.249f, -0.169f, 0.184f), new Vector3(354.3951f, 188.0336f, 353.6234f), "Bones/Bones|Steuerbone/Bones|Bauch/Bones|Brustkorb", new string[0]));
        items.Add(new Munition("Arrow", 2, "wooden Arrow", ItemSubtype.Arrow, Item.Rarity.Basic, 1, 100, new List<ItemAttribute>() { new ItemAttribute(AttributeList.BladeDamage, 10) }, new HandInfo(ItemHand.Right), new Vector3(0.644f, 0.372f, 0.31f), new Vector3(345.7398f, 62.82738f, 187.3137f)));
        items.Add(new RangedWeapon("Bogen_Connor", 3, "simply a Bow", 1, 100, new List<ItemAttribute>() { new ItemAttribute(AttributeList.BladeDamage, 10), new ItemAttribute(AttributeList.StitchDamage, 10), new ItemAttribute(AttributeList.SmashDamage, 10) }, new HandInfo(ItemHand.Left), new Vector3(-0.1969261f, -1.603304f, -1.516572f), new Vector3(353.0872f, 269.2368f, 268.7592f), 2, ItemSubtype.Bogen, Item.Rarity.Basic, new Vector3(0.609f, -1.476f, -1.491f), new Vector3(337.857f, 89.33858f, 0.2809239f), "Bones/Bones|Steuerbone/Bones|Bauch/Bones|Brustkorb", new string[] { "Quiver" }, ItemSubtype.Arrow));
        items.Add(new Armor("LotR-Harnisch", 4, "blabla", 100, 1, new List<ItemAttribute>() { new ItemAttribute(AttributeList.Protection, 100) }, ItemSubtype.Harnisch, Item.Rarity.Basic, 1));
        items.Add(new Armor("LotR-Hose", 5, "blabla", 100, 1, new List<ItemAttribute>() { new ItemAttribute(AttributeList.Protection, 40) }, ItemSubtype.Hose, Item.Rarity.Basic, 1));
        items.Add(new Armor("LotR-Handschuhe", 6, "blabla", 100, 1, new List<ItemAttribute>() { new ItemAttribute(AttributeList.Protection, 1000) }, ItemSubtype.Handschuhe, Item.Rarity.Basic, 1));
        items.Add(new Armor("LotR-Schuhe", 7, "blabla", 100, 1, new List<ItemAttribute>() { new ItemAttribute(AttributeList.Protection, 10) }, ItemSubtype.Schuhe, Item.Rarity.Basic, 1));
        items.Add(new Armor("LotR-Helm", 8, "blabla", 100, 1, new List<ItemAttribute>() { new ItemAttribute(AttributeList.Protection, 90) }, ItemSubtype.Helm, Item.Rarity.Basic, 1));
        items.Add(new Tool("Torch", 9, "blabla", ItemSubtype.Tool, Item.Rarity.Basic, 1, 100, new List<ItemAttribute>() { }, new HandInfo(ItemHand.Right,true,false), new Vector3(0, 0, 0), new Vector3(0, 0, 0)));
        items.Add(new Munition("Arrow", 10, "wooden Arrow", ItemSubtype.Bolt, Item.Rarity.Basic, 1, 100, new List<ItemAttribute>() { new ItemAttribute(AttributeList.BladeDamage, 10) }, new HandInfo(ItemHand.Right), new Vector3(-0.577f, 0.129f, 0.032f), new Vector3(350.178f, 274.0535f, 359.7505f)));
        items.Add(new RangedWeapon("Bogen_Connor", 11, "simply a Bow", 1, 100, new List<ItemAttribute>() { new ItemAttribute(AttributeList.BladeDamage, 10), new ItemAttribute(AttributeList.StitchDamage, 10), new ItemAttribute(AttributeList.SmashDamage, 10) }, new HandInfo(ItemHand.Left), new Vector3(-0.1969261f, -1.603304f, -1.516572f), new Vector3(353.0872f, 269.2368f, 268.7592f), 2, ItemSubtype.Bogen, Item.Rarity.Basic, new Vector3(0.609f, -1.476f, -1.491f), new Vector3(337.857f, 89.33858f, 0.2809239f), "Bones/Bones|Steuerbone/Bones|Bauch/Bones|Brustkorb", new string[] { "Quiver" }, ItemSubtype.Bolt));
        items.Add(new MeleeWeapon("Claymore", 12, "blabla", 1, 100, new List<ItemAttribute>() { new ItemAttribute(AttributeList.StitchDamage, 10), new ItemAttribute(AttributeList.SmashDamage, 10) }, new HandInfo(ItemHand.Right,false,true), new Vector3(0, 0, 0), new Vector3(0, 0, 0), 2, ItemSubtype.Großschwert, Item.Rarity.Basic, new Vector3(0, 0, 1.475f), new Vector3(0, 270f, 90f), "Bones/Bones|Steuerbone/Bones|Bauch/Bones|Brustkorb/Claymore_Sheath", new string[] { "Claymore_Sheath" }));
        items.Add(new Munition("Bomb", 13, "will explode", ItemSubtype.Bomb, Item.Rarity.Basic, 1, 100, new List<ItemAttribute>(), new HandInfo(ItemHand.Right,true, false), Vector3.zero, Vector3.zero));
        // items.Add(new Item("Dolch", 14, "A sword made of Silver", Item.ItemType.Weapon, Item.ItemSubtype.Schwert, Item.ItemHand.OffHand, new Vector3(0, 0, 0), new Vector3(0, 0, 0)));

        // create interactionTools
        interactionTools.Add(new Tool("Hammer", 10, "blabla", ItemSubtype.Tool, Item.Rarity.Basic, 1, 100, new List<ItemAttribute>() { }, new HandInfo(ItemHand.Right), new Vector3(0f, 0.254f, 0f), new Vector3(270f, 270f, 0f)));
        interactionTools.Add(new Tool("Sword_blank", 11, "blabla", ItemSubtype.Tool, Item.Rarity.Basic, 1, 100, new List<ItemAttribute>() { }, new HandInfo(ItemHand.Left), new Vector3(0f, 0.254f, 0f), new Vector3(270f, 270f, 0f)));

        // create weaponSheaths
        equipAdditions.Add(new EquipAddition("Quiver", new Vector3(-0.06702069f, 0.07599175f, 0.3560029f), new Vector3(2.058294f, 7.409856f, 18.58836f), "Bones/Bones|Steuerbone/Bones|Bauch/Bones|Brustkorb"));
        equipAdditions.Add(new EquipAddition("Sword_Sheath", new Vector3(1.997405e-05f, 2.269274f, -0.3721265f), new Vector3(312.4084f, 345.8079f, 202.5271f), "Bones/Bones|Steuerbone/Bones|Hüfte"));
        equipAdditions.Add(new EquipAddition("Claymore_Sheath", new Vector3(-0.008008166f, -0.6090195f, 0.2680035f), new Vector3(286.0776f, 288.8503f, 319.6609f), "Bones/Bones|Steuerbone/Bones|Bauch/Bones|Brustkorb"));

        // create currencies
        currencies.Add(new Consumable("Goldmünze", 13, "-", ItemSubtype.Geld, Item.Rarity.Basic, 0, 1, new List<ItemAttribute>() { }));
    }


    public Item GetItemByName(string name)
    {
        for(int i = 0; i < items.Count;i++)
        {
            if(items[i].itemName == name)
            {
                return items[i].GetCopy();
            }
        }
        return new Item();
    }

    public Item GetItemById(int id)
    {
        if (items[id].itemID == id)
        {
            return items[id].GetCopy();
        }
        else
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].itemID == id)
                {
                    return items[i].GetCopy();
                }
            }
        }

        return new Item();
    }

    public Item GetInteractionToolByName(string name)
    {
        for (int i = 0; i < interactionTools.Count; i++)
        {
            if (interactionTools[i].itemName == name)
            {
                return interactionTools[i].GetCopy();
            }
        }
        return new Item();
    }

    public Item GetEquipAdditionByName(string name)
    {
        for (int i = 0; i < equipAdditions.Count; i++)
        {
            if (equipAdditions[i].itemName == name)
            {
                return equipAdditions[i].GetCopy();
            }
        }
        return new Item();
    }

    public Item GetCurrencyByName(string name)
    {
        for (int i = 0; i < currencies.Count; i++)
        {
            if (currencies[i].itemName == name)
            {
                return currencies[i].GetCopy();
            }
        }
        return new Item();
    }
}
