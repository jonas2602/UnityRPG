using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CreatureItemStorage : ItemStorage
{
    [SerializeField]
    private List<Item> profilItems = new List<Item>();

    public List<Item> GetProfilItemList
    {
        get
        {
            return this.profilItems;
        }
    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
