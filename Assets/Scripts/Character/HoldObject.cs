using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HoldObject : MonoBehaviour 
{

    public List<GameObject> equipedWeapons = new List<GameObject>();
    public GameObject[] equipedTools = new GameObject[2];
    public int weaponSlots = 8;

    Animator animChar;
    public Profil profil;
    public ItemList itemDatabase;
    public OldInventory inventory;
    public Transform itemBoneL;
    public Transform itemBoneR;
    public Transform currentWeapon;
    public Weapon currentWeaponInfo;
    public Transform Quiver;
    public Transform chest;
    public Transform Sheath;
    public Transform arrowPrefab;
    public Transform zeigefingerR;
    public CharakterControler charController;
    public Transform hip;
    public bool holdArrow;
    public bool reload = false;
    public bool unload = false;


    public bool SetReload
    {
        set
        {
            reload = value;
        }
    }

    public bool SetUnload
    {
        set
        {
            unload = value;
        }
    }

    public bool HoldArrow
    {
        get
        {
            return this.holdArrow;
        }
    }

    void Awake()
    {
        for (int i = 0; i < weaponSlots; i++)
        {
            equipedWeapons.Add(null);
        }
    }

	// Use this for initialization
    void Start()
    {
        Transform player = this.transform;
        animChar = GetComponent<Animator>();
        profil = player.Find("Interface").GetComponent<Profil>();
        itemDatabase = GameObject.FindWithTag("ItemDatabase").GetComponent<ItemList>();
        inventory = player.Find("Interface").GetComponent<OldInventory>();
        charController = player.GetComponent<CharakterControler>();
        itemBoneR = player.Find("Bones/Bones|Steuerbone/Bones|Bauch/Bones|Brustkorb/Bones|Schulter.R/Bones|Oberarm.R/Bones|Unterarm.R/Bones|Hand.R/Bones|Sword");
        itemBoneL = player.Find("Bones/Bones|Steuerbone/Bones|Bauch/Bones|Brustkorb/Bones|Schulter.L/Bones|Oberarm.L/Bones|Unterarm.L/Bones|Hand.L/Bones|Shield");
        zeigefingerR = player.Find("Bones/Bones|Steuerbone/Bones|Bauch/Bones|Brustkorb/Bones|Schulter.R/Bones|Oberarm.R/Bones|Unterarm.R/Bones|Hand.R/Bones|Zeigefinger.R/Bones|Zeigefinger1.R/Bones|Zeigefinger2.R");
        chest = player.Find("Bones/Bones|Steuerbone/Bones|Bauch/Bones|Brustkorb");
        hip = player.Find("Bones/Bones|Steuerbone/Bones|Hüfte");

        Sheath = Instantiate(Resources.Load<GameObject>("Armor/Schwertscheide")).transform;
        Sheath.parent = hip;
        Sheath.localPosition = new Vector3(1.997405e-05f, 2.269274f, -0.3721265f);
        Sheath.localRotation = Quaternion.Euler(312.4084f, 345.8079f, 202.5271f);

        Quiver = Instantiate(Resources.Load<GameObject>("Armor/Quiver")).transform;
        Quiver.parent = chest;
        Quiver.localPosition = new Vector3(-0.06702069f, 0.07599175f, 0.3560029f);
        Quiver.localRotation = Quaternion.Euler(2.058294f, 7.409856f, 18.58836f);


    }
    
	
	// Update is called once per frame
    void Update()
    {
        if (holdArrow)
        {
            Munition arrow = itemDatabase.items[2] as Munition;
            arrowPrefab.localPosition = arrow.handParentPosition;
            arrowPrefab.localRotation = Quaternion.Euler(arrow.handParentRotation);
        }
       
    }


    void DrawWeapon(int itemHand)
    {
        int weaponID = animChar.GetInteger("currentWeapon");
        currentWeapon = equipedWeapons[weaponID].transform;
        currentWeaponInfo = profil.weaponSet[weaponID].slotItem as Weapon;

        animChar.SetBool("draw", false);
        animChar.SetInteger("armedWeapon", weaponID);

        Weapon weapon = profil.weaponSet[weaponID].slotItem as Weapon;
        Transform hand = itemBoneL;

        if (itemHand == 1)
        {
            hand = itemBoneR;
        }

        currentWeapon.parent = hand;
        currentWeapon.localPosition = weapon.handParentPosition;
        currentWeapon.localRotation = Quaternion.Euler(weapon.handParentRotation);
    }


    void HideWeapon()
    {
        Transform parent = this.transform.Find(currentWeaponInfo.weaponUnusedParentPath);

        currentWeapon.parent = parent;
        currentWeapon.localPosition = currentWeaponInfo.weaponUnusedParentPosition;
        currentWeapon.localRotation = Quaternion.Euler(currentWeaponInfo.weaponUnusedParentRotation);

        animChar.SetBool("hide", false);

        int current = animChar.GetInteger("currentWeapon");
        int armed = animChar.GetInteger("armedWeapon");
        if (current != armed)
        {
            animChar.SetBool("draw", true);
        }

        animChar.SetInteger("armedWeapon", -1);
    }


    public void AddWeapon(Weapon item, int id)
    {
        GameObject.Destroy(equipedWeapons[id]);
        Transform weapon = Instantiate(item.itemMesh).transform;
        Transform parent = this.transform.Find(item.weaponUnusedParentPath);
        equipedWeapons[id] = weapon.gameObject;

        weapon.parent = parent;
        weapon.localPosition = item.weaponUnusedParentPosition;
        weapon.localRotation = Quaternion.Euler(item.weaponUnusedParentRotation);
    }


    public void RemoveWeapon(int id)
    {
        GameObject.Destroy(equipedWeapons[id]);
    }

    /*
    create tempurare situation save current, restore after interacting
    */
    void DrawTool(int itemID)
    {/*
        Tool item = itemDatabase.items[itemID] as Tool;
        Transform hand = itemBoneR;

        if (item.handInfo == ItemHand.Left)
        {
            hand = itemBoneL;
        }

        equipedTools[(int)item.handInfo] = Instantiate(item.itemMesh);

        equipedTools[(int)item.handInfo].transform.parent = hand;
        equipedTools[(int)item.handInfo].transform.localPosition = item.handParentPosition;
        equipedTools[(int)item.handInfo].transform.localRotation = Quaternion.Euler(item.handParentRotation);
    */}

    void HideTool(int itemHand)
    {
        GameObject.Destroy(equipedTools[itemHand]);
    }


    // Bow
    public void Reload()
    {
        if (!holdArrow && inventory.InventoryContains(2))
        {
            Munition arrow = itemDatabase.items[2] as Munition;
            arrowPrefab = Instantiate(arrow.itemMesh).transform;
            arrowPrefab.parent = zeigefingerR;
            arrowPrefab.localPosition = arrow.handParentPosition;
            arrowPrefab.localRotation = Quaternion.Euler(arrow.handParentRotation);
            arrowPrefab.tag = "nextArrow";
            holdArrow = true;
            charController.ArrowContr = arrowPrefab.GetComponent<ArrowController>();
        }
    }


    public void Unload()
    {
        if (holdArrow)
        {
            Destroy(GameObject.FindWithTag("nextArrow"));
            holdArrow = false;
            charController.ArrowContr = null;
        }

    }


    public void Shoot()
    {
        arrowPrefab.parent = null;
        arrowPrefab.SendMessage("Shoot");
        holdArrow = false;
    }
}
