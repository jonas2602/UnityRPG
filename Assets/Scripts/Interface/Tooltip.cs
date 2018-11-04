using UnityEngine;
using System.Collections;

public class Tooltip
{
    public GUISkin skin;


    public Vector2 windowSize = new Vector2(350f, 350f);

    // public Vector2 namePos = new Vector2(0f, 0f);
    public Vector2 nameSize = new Vector2(350f, 46f);

    // public Vector2 rarityPos = new Vector2(0f, 46f);
    public Vector2 raritySize = new Vector2(350f, 40f);

    // public Vector2 itemTypePos = new Vector2(0f, 86f);
    public Vector2 itemTypeSize = new Vector2(50f, 50f);

    // public Vector2 statPos = new Vector2(0f, 86f);
    public Vector2 statSize = new Vector2(350f, 90f);

    // public Vector2 runePos = new Vector2(0f, 176f);
    public Vector2 runeSize = new Vector2(350f, 100f);

    // public Vector2 levelPos = new Vector2(50f, 276f);
    public Vector2 levelSize = new Vector2(350f, 36f);

    // public Vector2 infoPos = new Vector2(0f, 312f);
    public Vector2 infoSize = new Vector2(350f, 38f);
    

    
    public Tooltip(GUISkin skin)
    {
        this.skin = skin;
    }
    

    public void DrawToolTip(Item item)
    {
        Rect windowRect = new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, windowSize.x, windowSize.y);
        GUI.Box(windowRect, "", skin.GetStyle("Inventory"));

        // Name
        Rect nameRect = new Rect(windowRect.xMin, windowRect.yMin, windowSize.x, nameSize.y);
        GUI.Box(nameRect, item.itemName, skin.GetStyle("Inventory"));

        // Rarity
        Rect rarityRect = new Rect(windowRect.xMin, nameRect.yMax, windowSize.x, raritySize.y);
        GUI.Box(rarityRect, "Rarity", skin.GetStyle("Inventory"));

        // itemType
        Rect itemTypeRect = new Rect(windowRect.xMin, rarityRect.yMax, windowSize.x, itemTypeSize.y);
        GUI.Box(itemTypeRect, item.GetType().ToString(), skin.GetStyle("Inventory"));

        // Stats
        Rect statRect = new Rect(windowRect.xMin, itemTypeRect.yMax, windowSize.x, statSize.y);
        GUI.Box(statRect, "Stats", skin.GetStyle("Inventory"));

        // Runes
        Rect runeRect = new Rect(windowRect.xMin, statRect.yMax, windowSize.x, runeSize.y);
        GUI.Box(runeRect, "Runes", skin.GetStyle("Inventory"));

        // required level
        Rect levelRect = new Rect(windowRect.xMin, runeRect.yMax, windowSize.x, levelSize.y);
        GUI.Box(levelRect, "Required level: ", skin.GetStyle("Inventory"));

        // general info
        Rect infoRect = new Rect(windowRect.xMin, levelRect.yMax, windowSize.x, infoSize.y);
        GUI.Box(infoRect, item.itemWeight + " " + item.itemPrice, skin.GetStyle("Inventory"));
    }
}
