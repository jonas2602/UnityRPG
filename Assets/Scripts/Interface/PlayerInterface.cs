﻿using UnityEngine;
using System.Collections;

public class PlayerInterface : UIWindow
{
	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public override void SetupPlayer(GameObject player)
    {
        GetComponentInChildren<CrossHairController>().SetupPlayer(player);
    }
}
