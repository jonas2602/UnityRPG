using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatTypeList : MonoBehaviour {

    /*       (MainWeapon, OffWeapon)       ->        (FunctionName, AnimatorLayerName) */
    public Dictionary<Vector2, string> combatTypes = new Dictionary<Vector2, string>();

    void Awake()
    {
        combatTypes.Add(new Vector2(-1, -1), "Fist");
        combatTypes.Add(new Vector2((int)ItemSubtype.Schwert, -1), "SingleSword");
        combatTypes.Add(new Vector2((int)ItemSubtype.Schwert, (int)ItemSubtype.Schild), "SwordShield");
        combatTypes.Add(new Vector2((int)ItemSubtype.Bogen, -1), "Bow");
        combatTypes.Add(new Vector2((int)ItemSubtype.Großschwert, -1), "Claymore");
    }



    // Update is called once per frame
    void Update () {
	
	}

    public bool ContainsType(Vector2 key)
    {
        return combatTypes.ContainsKey(key);
    }

    public bool GetCombatType(Vector2 key, out string value)
    {
        int mainHand = (int)key.x;
        int offHand = (int)key.y;
        
        // Bothhand combat
        if (offHand != -1 && combatTypes.TryGetValue(new Vector2(mainHand, offHand), out value))
        {
            return true;
            // Debug.Log("Bothhand combat: " + (ItemSubtype)mainHand + "," + (ItemSubtype)offHand);
        }
        // Mainhand combat
        else if (combatTypes.TryGetValue(new Vector2(mainHand, -1), out value))
        {
            return true;
            // Debug.Log("Mainhand combat: " + (ItemSubtype)mainHand);
        }
        else
        {
            Debug.LogWarning("WeaponConstellation: " + (ItemSubtype)mainHand + "," + (ItemSubtype)offHand + " not setup");
            return false;
        }
    }
}
