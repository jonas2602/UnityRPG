using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EquipmentManager : MonoBehaviour
{
    private Transform avatar;
    private Animator anim;
    private CombatTypeList combatTypes;
    private Stitcher stitcher;
    private ItemList itemDatabase;

    public GameObject[] activeInteractionTools;

    public GameObject[] equipedCloth;
    public EquipedItem[] equipedUsables;        // { meleeMain , meleeOff , rangeMain , rangeOff , smallMain , tool , tool , bomb , bomb , food }
    // public GameObject[] equipedWeapons;
    // public Usable[] equipedWeaponsInfo;
    private Transform[] itemBones = new Transform[2];       // { left , right }
    private string[] stringAddition = new string[] { "Left", "Right" };

    // references equipedweapon array position
    [SerializeField]
    private int[] selectedSlots;                            // { main , off , tool }   selected in weaponWheel
    // [SerializeField]
    // private int[] activeSlots;                              // { main , off , tool }   may be drawn
    [SerializeField]
    private int[] currentSlots;                             // { left , right }        currently drawing
    [SerializeField]
    private int[] armedSlots;                               // { left , right }        currently armed
    [SerializeField]
    private EquipedItem[] armedUsables;                     // { left , right }        currently armed

    [SerializeField]
    private ItemHand[] handInfo = new ItemHand[2];          // { main , off }
    private string currentWeaponLayerName = "Fist";
    
    public Item[] UpdateSelectedSlots(int[] selectedSlots)
    {
        // update selected slots
        this.selectedSlots = selectedSlots;

        // return list of active items { main , off , tool , food }
        Item main = equipedUsables[selectedSlots[0]].itemInfo;
        Item off = equipedUsables[selectedSlots[1]].itemInfo;
        Item tool = equipedUsables[selectedSlots[2]].itemInfo;
        Item food = equipedUsables[9].itemInfo;
        return new Item[] { main, off, tool, food }; 

        /*/ update active slots
        if (activeSlots[0] == -1 || activeSlots[2] == -1)
        {
            UpdateActiveSlots();
        }*/
    }

    public ItemHand GetMainHand
    {
        get
        {
            return handInfo[0];
        }
    }

    public ItemHand GetOffHand
    {
        get
        {
            return handInfo[1];
        }
    }

    public EquipedItem[] GetArmedSet()
    {
        return this.armedUsables;
    }

    public Item GetMunition()
    {
        return equipedUsables[selectedSlots[1]].itemInfo;
    }

    public bool WeaponSetArmed()
    {
        int armedWeapon = anim.GetInteger("armed" + GetMainHand + "Weapon");
        bool drawing = anim.GetBool("draw" + GetMainHand);
        bool hiding = anim.GetBool("hide" + GetMainHand);

        // no weapon armed
        if(armedWeapon == -1)
        {
            if(drawing)
            {
                // will be armed
                return true;
            }
            else
            {
                // not armed
                return false;
            }
        }
        // weapon armed
        else
        {
            if(hiding)
            {
                // will be unarmed
                return false;
            }
            else
            {
                // armed
                return true;
            }
        }
    }



    // Use this for initialization
    void Awake()
    {
        stitcher = new Stitcher();
        anim = GetComponent<Animator>();
        combatTypes = GameObject.FindWithTag("Database").GetComponent<CombatTypeList>();
        itemDatabase = GameObject.FindWithTag("Database").GetComponent<ItemList>();
        avatar = this.transform;


        itemBones[0] = avatar.Find("Bones/Bones|Steuerbone/Bones|Bauch/Bones|Brustkorb/Bones|Schulter.L/Bones|Oberarm.L/Bones|Unterarm.L/Bones|Hand.L/Bones|Shield");
        itemBones[1] = avatar.Find("Bones/Bones|Steuerbone/Bones|Bauch/Bones|Brustkorb/Bones|Schulter.R/Bones|Oberarm.R/Bones|Unterarm.R/Bones|Hand.R/Bones|Sword");
        equipedCloth = new GameObject[5];
        equipedUsables = new EquipedItem[10];
        // equipedWeapons = new GameObject[9];
        // equipedWeaponsInfo = new Usable[9];

        activeInteractionTools = new GameObject[2];
        
        selectedSlots = new int[] { -1, -1, -1 };
        // activeSlots = new int[] { -1, -1, -1 };
        currentSlots = new int[2];
        armedSlots = new int[] { -1, -1 };
        armedUsables = new EquipedItem[] { new EquipedItem(), new EquipedItem() };
    }

    void Start()
    {
        for (int i = 0; i < equipedCloth.Length; i++)
        {
            GameObject part = GetNude(i);
            if (part != null)
            {
                EquipArmor(part, i);
            }
        }

        for(int i = 0; i < equipedUsables.Length;i++)
        {
            equipedUsables[i] = new EquipedItem();
        }

        // CombineMeshes();
    }

    public void UpdateHands()
    {
        ItemHand mainHand = ItemHand.Right;
        ItemHand offHand = ItemHand.Left;

        // set armed?
        /*if (anim.GetInteger("armedLeftWeapon") != -1 || anim.GetInteger("armedRightWeapon") != -1)
        {
            return;
        }*/
        // weapons selected?
        // else
        {
            // get mainWeaponSlot
            int mainWeaponSlot = selectedSlots[0];

            // get mainWeapon
            Usable mainWeapon = equipedUsables[mainWeaponSlot].itemInfo; // equipedWeaponsInfo[mainWeaponSlot];

            // has mainWeapon
            if (mainWeapon != null)
            {
                // get mainHand
                mainHand = mainWeapon.handInfo.preferedHand;

                // get offHand
                offHand = (mainHand == ItemHand.Left ? ItemHand.Right : ItemHand.Left);
            }
        }

        // set new handInfo
        handInfo = new ItemHand[] { mainHand, offHand }; 
    }
    /*
    public void UpdateActiveSlots()
    {
        // weaponSet is not armed?
        if (armedSlots[(int)GetMainHand] != activeSlots[0])
        {
            activeSlots[0] = selectedSlots[0];
            activeSlots[1] = selectedSlots[1];
        }

        // tool is not armed
        if (armedSlots[(int)GetOffHand] != activeSlots[2])
        {
            activeSlots[2] = selectedSlots[2];
        }
    }*/

    public void DrawHideWeaponSet()
    {
        // get selected main weapon
        int mainWeaponSlot = selectedSlots[0];
        Usable selectedMainWeapon = equipedUsables[mainWeaponSlot].itemInfo;

        int offWeaponSlot = selectedSlots[1];
        Usable selectedOffWeapon = equipedUsables[offWeaponSlot].itemInfo;

        int toolSlot = selectedSlots[2];
        Usable selectedTool = equipedUsables[toolSlot].itemInfo;
        
        // selected main weapon armed?
        if (armedUsables[(int)GetMainHand].itemInfo == selectedMainWeapon)
        {
            // => selected main weapon armed -> hide set, selected set = armedSet

            // hide mainWeapon
            anim.SetBool("hide" + GetMainHand, true);

            // is offWeapon equiped?
            if (selectedOffWeapon.itemID != -1)
            {
                // is offWeapon armed?
                if (armedUsables[(int)GetOffHand].itemInfo != selectedTool)
                {
                    // hide offWeapon
                    anim.SetBool("hide" + GetOffHand, true);
                }
            }
        }
        else
        {
            // selected main weapon not armed -> draw set, selected set != armedSet

            // other set armed?
            if(armedUsables[(int)GetMainHand].itemObject)
            {
                // => hide other set

                // hide mainWeapon
                anim.SetBool("hide" + GetMainHand, true);

                // is offWeapon equiped?
                if (selectedOffWeapon.itemID != -1)
                {
                    // is offWeapon armed?
                    if (armedUsables[(int)GetOffHand].itemInfo != selectedTool)
                    {
                        // hide offWeapon
                        anim.SetBool("hide" + GetOffHand, true);
                    }
                }
            }

            // draw new set
            UpdateHands();

            // has active main weapon?
            if (selectedMainWeapon.itemID != -1)
            {
                DrawUsable(GetMainHand, mainWeaponSlot);
            }

            // has active offWeapon
            if (selectedOffWeapon.itemID != -1)
            {
                DrawUsable(GetOffHand, offWeaponSlot);
            }
        }
    }

    public void DrawHideTool()
    {
        // get selected tool
        int toolSlot = selectedSlots[2];
        Usable selectedTool = equipedUsables[toolSlot].itemInfo;

        int offWeaponSlot = selectedSlots[1];
        Usable selectedOffWeapon = equipedUsables[offWeaponSlot].itemInfo;

        // tool armed?
        // Debug.Log(selectedTool);
        if (armedUsables[(int)GetOffHand].itemInfo == selectedTool)
        {
            // -> tool is armed -> hide tool
            
            // is offWeapon equiped?
            if (selectedOffWeapon.itemID != -1)
            {
                // draw off weapon
                DrawUsable(GetOffHand, offWeaponSlot);
            }
            else
            {
                // hide tool
                HideUsable(GetOffHand);
            }
        }
        else
        {
            // -> tool not armed -> draw tool

            // has active tool
            if (selectedTool.itemID != -1)
            {
                DrawUsable(GetOffHand, toolSlot);
            }
        }
    }

    public void DrawUsable(ItemHand itemHand, int equipSlot)
    {
        // usable armed in hand?
        if(armedUsables[(int)itemHand].itemObject)
        {
            // hide usable
            HideUsable(itemHand);
        }

        // draw usable
        anim.SetInteger("current" + itemHand + "Weapon", (int)equipedUsables[equipSlot].itemInfo.itemSubtype);
        anim.SetBool("draw" + itemHand, true);

        currentSlots[(int)itemHand] = equipSlot;
    }

    public void HideUsable(ItemHand itemHand)
    {
        // hide offWeapon
        anim.SetBool("hide" + itemHand, true);
    }

    public void Equip(Item item, int slot)
    {
        if (item is Weapon)
        {
            EquipWeapon(item as Weapon, slot);
        }
        else if (item is Armor)
        {
            EquipArmor(item.itemMesh, slot);
        }
        else if(item is Munition)
        {
            EquipMunition(item as Munition, slot);
        }
        else if(item is Tool)
        {
            EquipTool(item as Tool, slot);
        }
        else
        {
            Debug.Log(item.itemType +  " is not equipable");
        }
    }

    public void Unequip(Item item, int slot)
    {
        // Debug.Log("unequip: " + item.itemName);

        if (item is Weapon)
        {
            UnequipWeapon(slot);
        }
        else if (item is Armor)
        {
            UnequipArmor(slot);
        }
        else if (item is Munition)
        {
            UnequipMunition(slot);
        }
        else if (item is Tool)
        {
            UnequipTool(slot);
        }
        else
        {
            Debug.Log(item.itemType + " is not unequipable");
        }
    }
    
    #region Weapon
    void ArmWeapon(int itemHand)
    {
        // get current weapon from weaponWheel
        int weaponSlot = currentSlots[itemHand];
        Transform weaponToDraw = equipedUsables[weaponSlot].itemObject.transform; // equipedWeapons[weaponSlot].transform;

        // set animator values
        anim.SetBool("draw" + stringAddition[itemHand], false);
        anim.SetInteger("armed" + stringAddition[itemHand] + "Weapon", anim.GetInteger("current" + stringAddition[itemHand] + "Weapon"));

        // get item
        Weapon weapon = equipedUsables[weaponSlot].itemInfo as Weapon; // equipedWeaponsInfo[weaponSlot] as Weapon;

        // add weapon to hand
        weaponToDraw.parent = itemBones[itemHand];
        weaponToDraw.localPosition = weapon.handParentPosition;
        weaponToDraw.localRotation = Quaternion.Euler(weapon.handParentRotation);

        // update weaponScript
        weaponToDraw.SendMessage("UpdateAnim",SendMessageOptions.DontRequireReceiver);

        // set script variables
        armedSlots[itemHand] = currentSlots[itemHand];
        armedUsables[itemHand] = equipedUsables[currentSlots[itemHand]];

        // activate new animatorLayer
        SetNewWeaponLayer();
    }
    
    void UnarmWeapon(int itemHand)
    {
        Weapon weaponInfo = equipedUsables[armedSlots[itemHand]].itemInfo as Weapon; // equipedWeaponsInfo[armedSlots[itemHand]] as Weapon;
        Transform weaponToHide = equipedUsables[armedSlots[itemHand]].itemObject.transform; // equipedWeapons[armedSlots[itemHand]].transform;

        // Find parent to store at
        Transform storedParent = this.transform.Find(weaponInfo.weaponUnusedParentPath);

        // store weapon
        weaponToHide.parent = storedParent;
        weaponToHide.localPosition = weaponInfo.weaponUnusedParentPosition;
        weaponToHide.localRotation = Quaternion.Euler(weaponInfo.weaponUnusedParentRotation);

        // set animator values
        anim.SetBool("hide" + stringAddition[itemHand], false);
        anim.SetInteger("armed" + stringAddition[itemHand] + "Weapon", -1);

        // set script variables
        armedSlots[itemHand] = -1;
        armedUsables[itemHand] = new EquipedItem();

        // activate new animatorLayer
        SetNewWeaponLayer();

        // UpdateActiveSlots();
    }

    void EquipWeapon(Weapon weapon, int slot)
    {
        // Debug.Log("Equip: " + weapon.itemName);

        // unequip old weapon
        if (equipedUsables[slot].itemObject /*equipedWeapons[slot]*/)
        {
            UnequipWeapon(slot);
        }
        // equip additional items
        for (int i = 0; i < weapon.additionalGameObjectNames.Length; i++)
        {
            // create item
            EquipAddition equipItem = itemDatabase.GetEquipAdditionByName(weapon.additionalGameObjectNames[i]) as EquipAddition;
            Transform equipObject = Instantiate(equipItem.itemMesh).transform;
            // Debug.Log("Instantiate: " + equipObject);
            equipObject.name = equipItem.itemName;

            // find parent
            Transform parentObject = avatar.Find(equipItem.boneParentPath);
            if (parentObject)
            {
                // set parent
                equipObject.parent = parentObject;
                equipObject.localPosition = equipItem.boneParentPosition;
                equipObject.localRotation = Quaternion.Euler(equipItem.boneParentRotation);
            }
            else
            {
                // parent not found
                Debug.LogWarning("parent not found");
            }
        }

        // equip new weapon
        Transform weaponObject = Instantiate(weapon.itemMesh).transform;
        // equipedWeapons[slot] = weaponObject.gameObject;
        // equipedWeaponsInfo[slot] = weapon;
        equipedUsables[slot] = new EquipedItem(weapon, weaponObject.gameObject);
        
        // find parent
        Transform weaponParent = avatar.Find(weapon.weaponUnusedParentPath);
        if (weaponParent)
        {
            // set weaponParent
            weaponObject.parent = weaponParent;
            weaponObject.localPosition = weapon.weaponUnusedParentPosition;
            weaponObject.localRotation = Quaternion.Euler(weapon.weaponUnusedParentRotation);
        }
        else
        {
            // parent not found
            Debug.LogWarning("parent not found");
        }
    }

    void UnequipWeapon(int slot)
    {
        // remove additions
        Weapon weapon = equipedUsables[slot].itemInfo as Weapon; // equipedWeaponsInfo[slot] as Weapon;
        // Debug.Log(weapon.additionalGameObjectNames.Length);
        for (int i = 0; i < weapon.additionalGameObjectNames.Length;i++)
        {
            EquipAddition equipItem = itemDatabase.GetEquipAdditionByName(weapon.additionalGameObjectNames[i]) as EquipAddition;
            GameObject equipObject = avatar.Find(equipItem.boneParentPath + "/" + equipItem.itemName).gameObject;
            Debug.Log("Destroy: " + equipObject);
            Destroy(equipObject);
        }

        // remove weapon
        Destroy(equipedUsables[slot].itemObject);
        equipedUsables[slot] = new EquipedItem();
        // equipedWeapons[slot] = null;
        // equipedWeaponsInfo[slot] = null;
    }
    #endregion

    #region Armor
    void EquipArmor(GameObject cloth, int slot)
    {
        GameObject.Destroy(equipedCloth[slot]);
        GameObject clothMesh = GameObject.Instantiate(cloth);
        equipedCloth[slot] = stitcher.Stitch(clothMesh, avatar.Find("Bones"));
        GameObject.Destroy(clothMesh);

    }

    void UnequipArmor(int ID)
    {
        GameObject.Destroy(equipedCloth[ID]);
        EquipArmor(GetNude(ID), ID);
    }

    GameObject GetNude(int index)
    {
        GameObject bodypart = null;
        switch (index)
        {
            case 0:
                {
                    bodypart = Resources.Load<GameObject>("Bodyparts/" + "Head");
                    break;
                }
            case 1:
                {
                    bodypart = Resources.Load<GameObject>("Bodyparts/" + "Torso2");
                    break;
                }
            case 2:
                {
                    bodypart = Resources.Load<GameObject>("Bodyparts/" + "Hands");
                    break;
                }
            case 3:
                {
                    bodypart = Resources.Load<GameObject>("Bodyparts/" + "Legs1");
                    break;
                }
            case 4:
                {
                    bodypart = Resources.Load<GameObject>("Bodyparts/" + "Feet");
                    break;
                }
        }

        return bodypart;
    }
    #endregion

    #region Munition
    void ArmMunition(int itemHand)
    {
        // get current tool from weaponWheel
        int munitionSlot = currentSlots[itemHand];
        
        // set animator values
        anim.SetBool("draw" + stringAddition[itemHand], false);
        anim.SetInteger("armed" + stringAddition[itemHand] + "Weapon", anim.GetInteger("current" + stringAddition[itemHand] + "Weapon"));

        // get item
        Munition munition = equipedUsables[munitionSlot].itemInfo as Munition; // equipedWeaponsInfo[toolSlot] as Tool;

        Transform munitionToDraw = Instantiate(munition.itemMesh).transform; // equipedWeapons[toolSlot].transform;

        // add tool to hand
        munitionToDraw.parent = itemBones[itemHand];
        munitionToDraw.localPosition = munition.handParentPosition;
        munitionToDraw.localRotation = Quaternion.Euler(munition.handParentRotation);

        // set script variables
        armedSlots[itemHand] = currentSlots[itemHand];
        armedUsables[itemHand] = new EquipedItem(munition, munitionToDraw.gameObject);
    }

    void UnarmMunition(int itemHand)
    {
        // shot munition
        MunitionController controller = armedUsables[itemHand].itemObject.GetComponent<MunitionController>();
        controller.Shoot();

        // decrement munition amount
        armedUsables[itemHand].itemInfo.itemAmount--;

        // set animator values
        anim.SetBool("hide" + stringAddition[itemHand], false);
        anim.SetInteger("armed" + stringAddition[itemHand] + "Weapon", -1);

        // set script variables
        armedSlots[itemHand] = -1;
        armedUsables[itemHand] = new EquipedItem();
    }

    void EquipMunition(Munition munition, int slot)
    {
        // unequip old
        UnequipMunition(slot);

        // create new
        // equipedWeapons[slot] = munitionObject.gameObject;
        // equipedWeaponsInfo[slot] = munition;
        equipedUsables[slot] = new EquipedItem(munition);
        
        // hide tool
        /*equipedWeapons[slot] equipedUsables[slot].itemObject.SetActive(false);*/
    }

    void UnequipMunition(int slot)
    {
        // remove munition
        // equipedWeapons[slot]);

        equipedUsables[slot] = new EquipedItem();
        // equipedWeapons[slot] = null;
        // equipedWeaponsInfo[slot] = null;
    }
    #endregion

    #region Tool
    void ArmTool(int itemHand) 
    {
        // get current tool from weaponWheel
        int toolSlot = currentSlots[itemHand];
        Transform toolToDraw = equipedUsables[toolSlot].itemObject.transform; // equipedWeapons[toolSlot].transform;

        // set animator values
        anim.SetBool("draw" + stringAddition[itemHand], false);
        anim.SetInteger("armed" + stringAddition[itemHand] + "Weapon", anim.GetInteger("current" + stringAddition[itemHand] + "Weapon"));

        // get item
        Tool tool = equipedUsables[toolSlot].itemInfo as Tool; // equipedWeaponsInfo[toolSlot] as Tool;

        // activate tool
        toolToDraw.gameObject.SetActive(true);

        // add tool to hand
        toolToDraw.parent = itemBones[itemHand];
        toolToDraw.localPosition = tool.handParentPosition;
        toolToDraw.localRotation = Quaternion.Euler(tool.handParentRotation);

        // set script variables
        armedSlots[itemHand] = currentSlots[itemHand];
        armedUsables[itemHand] = equipedUsables[currentSlots[itemHand]];

        // activate new animatorLayer
        SetNewWeaponLayer();
    }

    void UnarmTool(int itemHand)
    {
        // hide tool
        GameObject toolToHide = equipedUsables[armedSlots[itemHand]].itemObject; // equipedWeapons[armedSlots[itemHand]];
        toolToHide.SetActive(false);

        // set animator values
        anim.SetBool("hide" + stringAddition[itemHand], false);
        anim.SetInteger("armed" + stringAddition[itemHand] + "Weapon", -1);

        // set script variables
        armedSlots[itemHand] = -1;
        armedUsables[itemHand] = new EquipedItem();

        // activate new animatorLayer
        SetNewWeaponLayer();
    }

    void EquipTool(Tool tool, int slot)
    {
        // unequip old
        UnequipTool(slot);

        // create new
        Transform toolObject = Instantiate(tool.itemMesh).transform;
        // equipedWeapons[slot] = toolObject.gameObject;
        // equipedWeaponsInfo[slot] = tool;
        equipedUsables[slot] = new EquipedItem(tool, toolObject.gameObject);

        // set parent
        toolObject.parent = this.transform;
        toolObject.localPosition = Vector3.zero;

        // hide tool
        equipedUsables[slot].itemObject.SetActive(false); // equipedWeapons[slot].SetActive(false);
    }

    void UnequipTool(int slot)
    {
        // remove tool
        Destroy(/*equipedWeapons[slot]*/ equipedUsables[slot].itemObject);

        equipedUsables[slot] = new EquipedItem();
        // equipedWeapons[slot] = null;
        // equipedWeaponsInfo[slot] = null;
    }

    void ArmInteractionTool(AnimationEvent info)
    {
        // evaluate animationInfos
        int hand = info.intParameter;
        string itemName = info.stringParameter;

        Usable tool = itemDatabase.GetInteractionToolByName(itemName) as Usable;
        Transform toolObject = Instantiate(tool.itemMesh).transform;
        activeInteractionTools[hand] = toolObject.gameObject;

        toolObject.parent = itemBones[hand];
        toolObject.localPosition = tool.handParentPosition;
        toolObject.localRotation = Quaternion.Euler(tool.handParentRotation);
    }

    void UnarmInteractionTool(int itemHand)
    {
        Destroy(activeInteractionTools[itemHand]);
        activeInteractionTools[itemHand] = null;
    }
    #endregion

    public void Reload()
    {
        Debug.Log("reload");
    } 

    public void Unload()
    {
        Debug.Log("unload");
    }

    void SetNewWeaponLayer()
    {
        // get new layerValues
        int mainHand = anim.GetInteger("armed" + GetMainHand + "Weapon");
        int offHand = anim.GetInteger("armed" + GetOffHand + "Weapon");

        string newLayerName;
        // new layer exists?
        if (combatTypes.GetCombatType(new Vector2(mainHand, offHand), out newLayerName))
        {
            // new layer?
            if (newLayerName != currentWeaponLayerName)
            {
                // deactivate old layer
                // Debug.Log("deactivate layer: " + currentWeaponLayerName);
                StartCoroutine(DeactivateLayer(new Vector2(anim.GetLayerIndex(currentWeaponLayerName), 2f)));

                // activate new layer
                currentWeaponLayerName = newLayerName;
                // Debug.Log("activate layer: " + currentWeaponLayerName);
                StartCoroutine(ActivateLayer(new Vector2(anim.GetLayerIndex(currentWeaponLayerName), 2f)));
            }
        }
    }

    IEnumerator ActivateLayer(Vector2 info)
    {
        int layerIndex = (int)info.x;
        float multiplier = info.y;

        float weight = 0;
        while (weight <= 1)
        {
            weight += Time.deltaTime * multiplier;
            anim.SetLayerWeight(layerIndex, weight);

            yield return null;
        }
    }

    IEnumerator DeactivateLayer(Vector2 info)
    {
        int layerIndex = (int)info.x;
        float multiplier = info.y;

        float weight = 1;
        while (weight >= 0)
        {
            weight -= Time.deltaTime * multiplier;
            anim.SetLayerWeight(layerIndex, weight);

            yield return null;
        }
    }


    /*
    void CombineMeshes()
    {
        SkinnedMeshRenderer[] meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        CombineInstance[] combine = new CombineInstance[meshRenderers.Length];
        int i = 0;
        while (i < meshRenderers.Length)
        {
            combine[i].mesh = meshRenderers[i].sharedMesh;
            combine[i].transform = meshRenderers[i].transform.localToWorldMatrix;
            meshRenderers[i].gameObject.SetActive(false);
            i++;
        }
        transform.GetComponent<SkinnedMeshRenderer>().sharedMesh = new Mesh();
        transform.GetComponent<SkinnedMeshRenderer>().sharedMesh.CombineMeshes(combine);
        transform.gameObject.SetActive(true);
    }
    */
}

[System.Serializable]
public class EquipedItem
{
    public Usable itemInfo = null;
    public GameObject itemObject = null;

    public EquipedItem(Usable itemInfo, GameObject itemObject)
    {
        this.itemInfo = itemInfo;
        this.itemObject = itemObject;
    }

    public EquipedItem(Usable itemInfo)
    {
        this.itemInfo = itemInfo;
    }

    public EquipedItem()
    {
        itemInfo = new Usable();
        itemObject = null;
    }
}



