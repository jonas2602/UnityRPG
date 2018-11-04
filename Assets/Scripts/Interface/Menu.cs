using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Menu : UIWindow
{
    public List<GameObject> menus = new List<GameObject>();

    public MenuList currentMenu;


    public enum MenuList
    {
        MainMenu,
        GameMenu,
        OptionsMenu,
        ControllMenu
    }

    void Awake()
    {
        // pick all windows
        for (int i = 0; i < transform.childCount; i++)
        {
            menus.Add(transform.GetChild(i).gameObject);
        }
    }

    void Start()
    {
        // deactivate all windows
        for (int i = 0; i < menus.Count; i++)
        {
            menus[i].SetActive(false);
        }

        // activate startWindow
        currentMenu = MenuList.MainMenu;
        menus[(int)MenuList.MainMenu].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ContinueGame()
    {
        GetComponentInParent<UIManager>().ChoseWindow(UIManager.GUIWindow.Menu);
    }

    public void StartNewGame()
    {
        Debug.Log("Start new Game");
    }

    public void LoadGame()
    {
        Debug.Log("Load Game");
    }

    public void SaveGame()
    {
        Debug.Log("Save Game");
    }

    public void BackToMain()
    {
        Debug.Log("Back to MainMenu");
    }

    public void ExitGame()
    {
        Application.Quit();
    }


    public void ShowOptions()
    {
        ChoseMenu(MenuList.OptionsMenu);
    }


    void ChoseMenu(MenuList menu)
    {
        // hide current
        menus[(int)currentMenu].SetActive(false);
        currentMenu = menu;
        menus[(int)currentMenu].SetActive(true);
        // show new
    }
}
