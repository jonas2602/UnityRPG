using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Interface : NetworkBehaviour
{
    public CharakterControler characterController;
    public NetworkIdentity identity;
    public Profil profil;
    public OldInventory inventory;
    public ItemManager itemManager;
    public Dialog dialog;
    public WeaponWheel weaponWheel;
    public ControlInfo controlInfo;
    public Questlog questlog;
    public Traits traits;
    public Map map;
    public ItemList database;
    public EquipmentManager equipment;
    public Tooltip tooltip;
    public GUISkin skin;
    public HoldObject holdObject;
    public AvatarInfo avatarInfo;
    public Animator animChar;
    public bool onGUI;

    public Canvas canvas;
    public Camera cam;

    public GUIType currentGUI;
    public bool showCharacterGUI;
    
    public bool showDialog = false;
    public bool showWheel = false;
    public bool showInteract = false;

    private int currentWeaponType = 4;

    public enum GUIType
    {
        None,
        Overview,
        // Monster,
        // Charaktere,
        // Handwerk,
        // Alchemie,
        Inventory,
        Map,
        Questlog,
        Character//(Traits)
    }


    public bool GetOnGUI
    {
        get
        {
            return this.onGUI;
        }
    }

    public int GetCurrentWeaponType
    {
        get
        {
            return this.currentWeaponType;
        }
    }

    public bool SetShowInteract
    {
        set
        {
            showInteract = value;
        }
    }



    // Use this for initialization
    void Awake()
    {
        Transform player = this.transform.root;

        animChar = player.GetComponent<Animator>();
        identity = player.GetComponent<NetworkIdentity>();
        characterController = player.GetComponent<CharakterControler>();
        equipment = player.GetComponent<EquipmentManager>();
        tooltip = new Tooltip(skin);
        holdObject = player.GetComponent<HoldObject>();
        database = GameObject.FindWithTag("Database").GetComponent<ItemList>();
        inventory = GetComponent<OldInventory>();
        profil = GetComponent<Profil>();
        itemManager = GetComponent<ItemManager>();
        dialog =GetComponent<Dialog>();
        questlog = GetComponent<Questlog>();
        traits = GetComponent<Traits>();
        map = GetComponent<Map>();
        weaponWheel = GetComponent<WeaponWheel>();
        controlInfo = GetComponent<ControlInfo>();
        avatarInfo = GetComponent<AvatarInfo>(); 
    }


    // Update is called once per frame
    void Update()
    {
        /*
        if (Input.GetButtonDown("Monster"))
        {
            ChoseUI(InterfaceType.Monster);
        }

        if (Input.GetButtonDown("Charaktere"))
        {
            ChoseUI(InterfaceType.Charaktere);
        }

        if (Input.GetButtonDown("Handwerk"))
        {
            ChoseUI(InterfaceType.Handwerk);
        }

        if (Input.GetButtonDown("Alchemie"))
        {
            ChoseUI(InterfaceType.Alchemie);
        }
        */
        if (Input.GetButtonDown("Inventory"))
        {
            ChoseUI(GUIType.Inventory);
        }

        if (Input.GetButtonDown("Map"))
        {
            ChoseUI(GUIType.Map);
        }

        if (Input.GetButtonDown("Questlog"))
        {
            questlog.RearrangeQuests();
            ChoseUI(GUIType.Questlog);
        }

        if (Input.GetButtonDown("Character"))
        {
            //(Traits)
            ChoseUI(GUIType.Character);
        }

        if (Input.GetKey("escape"))
        {
            if (showCharacterGUI)
            {
                if (currentGUI == GUIType.Overview)
                {
                    showCharacterGUI = false;
                    currentGUI = GUIType.None;
                }
                else
                {
                    currentGUI = GUIType.Overview;
                }
            }
        }

        if (Input.GetButtonDown("WeaponWheel"))
        {
            weaponWheel.CreateList();
            // weaponWheel.SetCurrentWeapon = currentWeaponType;
            showWheel = true;
        }

        if (Input.GetButtonUp("WeaponWheel"))
        {
            showWheel = false;
            // currentWeaponType = weaponWheel.GetCurrentWeapons;
            // animChar.SetInteger("currentWeapon", currentWeaponType);
        }
    }
    

    void ChoseUI(GUIType buttonUI)
    {
        if(currentGUI == buttonUI)
        {
            showCharacterGUI = false;
            currentGUI = GUIType.None;
        }
        else
        {
            currentGUI = buttonUI;
            showCharacterGUI = true;
        }
    }


    void OnGUI()
    {
        if (identity.isLocalPlayer)
        {
            onGUI = false;

            if (showCharacterGUI)
            {
                onGUI = true;

                Item tooltipItem = null;

                switch (currentGUI)
                {
                    case GUIType.Overview:
                        {

                            break;
                        }
                    case GUIType.Inventory:
                        {
                            inventory.DrawInventory(ref tooltipItem);
                            profil.DrawProfil(ref tooltipItem);
                            break;
                        }
                    case GUIType.Map:
                        {
                            map.DrawMap();
                            break;
                        }
                    case GUIType.Questlog:
                        {
                            questlog.DrawQuestlog();
                            break;
                        }
                    case GUIType.Character:
                        {
                            traits.DrawTraits();
                            break;
                        }
                }

                // draw dragged item
                ItemStack draggedItem = itemManager.draggedItem;
                if (draggedItem.slotAmount != 0)
                {
                    // GUI.DrawTexture(new Rect(Event.current.mousePosition.x + 15f, Event.current.mousePosition.y, 50, 50), draggedItem.slotItem.itemIcon);
                }
                // draw tooltip
                else if (tooltipItem != null)
                {
                    tooltip.DrawToolTip(tooltipItem);
                }

               
            }
            else if (showDialog)
            {
                onGUI = true;
                dialog.DrawDialog();
            }
            else
            {
                if (showWheel)
                {
                    onGUI = true;
                    weaponWheel.DrawWheel();
                }
                else
                {
                    weaponWheel.DrawActiveWeapon();
                    avatarInfo.DrawPlayerInfo();
                    avatarInfo.DrawGroupInfo();
                    avatarInfo.DrawEnemyInfo();
                    // controlInfo.DrawControl();
                    map.DrawMinimap();
                    if (showInteract)
                    {
                        // characterController.DrawButton();
                    }
                }
            }
        }
    }
}
