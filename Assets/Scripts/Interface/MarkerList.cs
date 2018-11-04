using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MarkerList : MonoBehaviour
{

    public List<Marker> marker = new List<Marker>();
    public List<Marker> legend = new List<Marker>();

    // Use this for initialization
    void Awake()
    {
        //marker
        marker.Add(new Marker("character", 0, GameObject.FindWithTag("Player"), Marker.Type.Character));
        marker.Add(new Marker("1.Marker", 1, new Vector3(0, 0, 0), Marker.Type.Unknown));

        //legend
        legend.Add(new Marker("Character", 0, Marker.Type.Character));
        legend.Add(new Marker("Quest", 1, Marker.Type.Quest));
        legend.Add(new Marker("Merchant", 2, Marker.Type.Merchant));
        legend.Add(new Marker("Dungeon", 3, Marker.Type.Dungeon));
        legend.Add(new Marker("Craftsman", 4, Marker.Type.Craftsman));
        legend.Add(new Marker("Enemy", 5, Marker.Type.Enemy));
        legend.Add(new Marker("Unit", 6, Marker.Type.Unit));
        legend.Add(new Marker("Unknown", 7, Marker.Type.Unknown));
    }

}
