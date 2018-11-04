using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public GameObject player;
    [SerializeField]
    private UIWindow[] windowParts;

    Dictionary<string, UIWindow[]> guiWindows = new Dictionary<string, UIWindow[]>();
    
    public GUIWindow currentWindow;
    public GUIWindow[] windowStack;
    public TooltipScript tooltip;

    public enum GUIWindow
    {
        // Overview,
        // Monster,
        // Charaktere,
        // Handwerk,
        // Alchemie,
        Interface,
        PlayerInventory,
        Dialog,
        Loot,
        Trading,
        // Map,
        // Questlog,
        Character, // (Traits)
        WeaponWheel,
        Menu
    }

    
    public object GetWindow<T>()
    {
        for(int i = 0; i < windowParts.Length;i++)
        {
            if(windowParts[i] is T)
            {
                return windowParts[i];
            }
        }

        return null;
    }
    

    public bool IsInMenu()
    {
        if(currentWindow == GUIWindow.Interface)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    

    // Use this for initialization
    void Awake()
    {
        // get references
        tooltip = GetComponentInChildren<TooltipScript>();

        // pick all windows
        windowParts = GetComponentsInChildren<UIWindow>(true);
        
        // setup guiWindows list
        AddGuiWindow("Interface", new string[] { "Interface" });
        AddGuiWindow("PlayerInventory", new string[] { "Inventory" , "Equipment" }); // playerinventory contains interface, opens inventory,profil which are individual windows
        AddGuiWindow("Dialog", new string[] { "Dialog" });
        AddGuiWindow("Loot", new string[] { "Interface" , "Loot" });
        AddGuiWindow("Trading", new string[] { "Inventory", "TraderInventory" });
        AddGuiWindow("WeaponWheel", new string[] { "WeaponWheel" });
        AddGuiWindow("Menu", new string[] { "Menu" });
        AddGuiWindow("Character", new string[] { "Character" });

        // validate windows
        int windowCount = System.Enum.GetValues(typeof(GUIWindow)).Length;
        for (int i = 0; i < windowCount; i++)
        {
            string windowName = ((GUIWindow)i).ToString();
            if (!guiWindows.ContainsKey(windowName))
            {
                // window missing
                Debug.LogError("window missing (" + windowName + ")");
            }

        }
    }

    void AddGuiWindow(string name, string[] windowNames)
    {
        // get windowParts
        UIWindow[] relevantWindowParts = new UIWindow[windowNames.Length];
        
        // go through relevant windowparts
        for (int i = 0; i < relevantWindowParts.Length;i++)
        {
            // go through found windowParts
            for(int j = 0; j < windowParts.Length; j++)
            {
                // Debug.Log("GUIWindow: " + name + ", WindowPartName: " + windowNames[i] + ", WindowPart: " + windowParts[j]);
                if (windowNames[i] == windowParts[j].name)
                {
                    relevantWindowParts[i] = windowParts[j];
                    break;
                }
            }

            // validate uiWindow
            if(!relevantWindowParts[i])
            {
                Debug.LogError("window: " + windowNames[i] + " missing");
            }
        }

        // add full window to dictionary
        guiWindows.Add(name, relevantWindowParts);
    }


    void Start()
    {
        // deactivate windows
        for(int i = 0; i < windowParts.Length; i++)
        {
            windowParts[i].gameObject.SetActive(false);
            // Debug.Log(windowParts[i].gameObject);
            // Debug.Log(windowParts[i].name + " is active " + windowParts[i].isActiveAndEnabled);
        }

        if (tooltip)
        {
            tooltip.gameObject.SetActive(false);
        }

        // activate startWindow
        ChoseWindow(GUIWindow.Interface);
    }


    public void SetupPlayer(GameObject player)
    {
        this.player = player;

        for(int i = 0; i < windowParts.Length;i++)
        {
            windowParts[i].SetupPlayer(player);
        }
    }

    public void OpenDialog(DialogManager dialogManager)
    {
        ChoseWindow(GUIWindow.Dialog);
        UIWindow[] windows;
        guiWindows.TryGetValue(currentWindow.ToString(), out windows);
        (windows[0] as PlayerDialog).StartDialog(dialogManager);
    }

    public void ExitDialog()
    {
        ChoseWindow(GUIWindow.Dialog);
    }

    public void OpenTradingWindow()
    {
        ChoseWindow(GUIWindow.Trading);
    }

    public void ExitTrading()
    {
        ChoseWindow(GUIWindow.Trading);
    }

    public void OpenLootWindow()
    {
        ChoseWindow(GUIWindow.Loot);
    }

    public void ExitLootWindow()
    {
        ChoseWindow(GUIWindow.Loot);
    }


    // Update is called once per frame
    void Update()
    {
        // in main menu
        if (currentWindow == GUIWindow.Menu)
        {
            if (Input.GetKeyDown("escape"))
            {
                ChoseWindow(GUIWindow.Menu);
            }
        }
        // in game
        else
        {
            if (Input.GetButtonDown("Inventory"))
            {
                ChoseWindow(GUIWindow.PlayerInventory);
                tooltip.DeactivateTooltip();
            }

            if (Input.GetButtonDown("WeaponWheel"))
            {
                if (currentWindow == GUIWindow.Interface || currentWindow == GUIWindow.WeaponWheel)
                {
                    ChoseWindow(GUIWindow.WeaponWheel);
                }
            }

            if (Input.GetKeyDown("escape"))
            {
                // in game
                if (currentWindow == GUIWindow.Interface)
                {
                    ChoseWindow(GUIWindow.Menu);
                }
                // in any other ui window
                else
                {
                    ChoseWindow(GUIWindow.Interface);
                }
            }
            /*
            if (Input.GetButtonDown("Map"))
            {
                ChoseUI(GUIType.Map);
            }

            if (Input.GetButtonDown("Questlog"))
            {
                questlog.RearrangeQuests();
                ChoseUI(GUIType.Questlog);
            }
            */
            if (Input.GetButtonDown("Character"))
            {
                //(Traits)
                ChoseWindow(GUIWindow.Character);
            }
        }
    }


    public void ChoseWindow(GUIWindow id)
    {
        // close current
        CloseWindow();

        // chosen window was open
        if (currentWindow == id)
        {
            // open interface
            OpenWindow(GUIWindow.Interface);
        }
        // chosen window was not
        else
        {
            // open chosen
            OpenWindow(id);
        }
    }

    void OpenWindow(GUIWindow id)
    {
        // set as active window
        currentWindow = id;

        // get windows
        UIWindow[] windows;
        guiWindows.TryGetValue(currentWindow.ToString(), out windows);

        // open windows
        for (int i = 0; i < windows.Length; i++)
        {
            windows[i].OpenWindow();
        }
    }

    void CloseWindow()
    {
        // get windows
        UIWindow[] windows;
        guiWindows.TryGetValue(currentWindow.ToString(), out windows);
        
        // close windows
        for (int i = 0; i < windows.Length; i++)
        {
            windows[i].CloseWindow();
        }
    }
}
