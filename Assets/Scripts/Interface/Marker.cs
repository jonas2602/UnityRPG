using UnityEngine;
using System.Collections;

[System.Serializable]
public class Marker
{
    public string name;
    public int id;
    public Vector3 position;
    public GameObject marked;
    public Texture2D icon;
    public Texture2D extendsIcon;
    public Type type;

    public enum Type
    {
        Personal,   // Ring
        Character,  // <-
        Quest,      // !
        Merchant,   // Geldbeutel    
        Dungeon,    // Cave
        Craftsman,  // Craftingstation
        Enemy,      // Red Point
        Unit,       // BluePoint
        NPC,        // WhitePoint
        Unknown     // Questionmark
    }

    //Legend
    public Marker(string name, int id, Type type)
    {
        this.name = name;
        this.id = id;
        this.type = type;
    }

    //Standing ref
    public Marker(string name, int id, Vector3 position, Type type)
    {
        this.name = name;
        this.id = id;
        this.position = position;
        this.icon = Resources.Load<Texture2D>("MarkerIcons/" + type);
        this.type = type;
    }
    //Moving ref
    public Marker(string name, int id, GameObject marked, Type type)
    {
        this.name = name;
        this.id = id;
        this.marked = marked;
        this.icon = Resources.Load<Texture2D>("MarkerIcons/" + type);
        this.type = type;
    }

    //PersonalMarker
    public Marker(string name, Vector3 position, Type type)
    {
        this.name = name;
        this.position = position;
        this.icon = Resources.Load<Texture2D>("MarkerIcons/" + type);
        this.type = type;
    }

    //PersonalMarker over Other
    public Marker(string name, Vector3 position, Type type, Type extendsIcon)
    {
        this.name = name;
        this.position = position;
        this.icon = Resources.Load<Texture2D>("MarkerIcons/" + type);
        this.extendsIcon = Resources.Load<Texture2D>("MarkerIcons/" + extendsIcon);
        this.type = type;
    }

    //Empty Marker
    public Marker()
    {

    }

    public Vector3 GetPosition
    {
        get
        {
            if (marked != null)
            {
                return this.marked.transform.position;
            }
            else
            {
                return this.position;
            }

        }
    }
}
