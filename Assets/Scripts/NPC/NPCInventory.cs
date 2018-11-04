using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPCInventory : MonoBehaviour
{
    private EquipmentManager equipmentManager;
    private ItemStorage itemStorage;

    
    void Awake()
    {
        Transform avatar = transform.root;

        equipmentManager = avatar.GetComponent<EquipmentManager>();
        itemStorage = avatar.GetComponent<ItemStorage>();
    }


    // Use this for initialization
    void Start()
    {
        bool meleeEquiped = false;
        bool rangedEquiped = false;
        bool munitionEquiped = false;

        // search for weapon
        List<Item> itemsInInventory = itemStorage.GetInventoryItemList;

        for(int i = 0; i < itemsInInventory.Count;i++)
        {
            // is weapon?
            if (itemsInInventory[i] is MeleeWeapon && !meleeEquiped)
            {
                MeleeWeapon weapon = itemsInInventory[i] as MeleeWeapon;

                // is mainWeapon?
                if(weapon.handInfo.preferedHand == ItemHand.Right)
                {
                    // equip Weapon
                    equipmentManager.Equip(weapon, 0);

                    // set as active weapon
                    equipmentManager.UpdateSelectedSlots(new int[] { 0, 1, 2 });
                    meleeEquiped = true;
                }
                
            }
            // is rangedWeapon
            else if(itemsInInventory[i] is RangedWeapon && !rangedEquiped)
            {
                RangedWeapon bow = itemsInInventory[i] as RangedWeapon;
                // Debug.Log("bow found");

                // is bow?
                if (bow.munitionType == ItemSubtype.Arrow)
                {
                    // equip bow
                    equipmentManager.Equip(bow, 2);
                    rangedEquiped = true;
                }
            }

            // is munition
            else if (itemsInInventory[i] is Munition && rangedEquiped && !munitionEquiped)
            {
                Munition arrow = itemsInInventory[i] as Munition;
                // Debug.Log("arrow found");

                // is right munitionType?
                if (arrow.itemSubtype == ItemSubtype.Arrow)
                {
                    // equip arrow
                    equipmentManager.Equip(arrow, 3);
                    munitionEquiped = true;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
