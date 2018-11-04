using UnityEngine;
using System.Collections;

public class ControlInfo : MonoBehaviour 
{
    /*
    public struct KeyInfo
    {
        public string key;
        public string[] actions;

        public void KeyInfo(string key, string[] actions)
        {
            this.key = key;
            this.actions = actions;
        }
    }
    public string[] actionsE = new string[]{"interagieren","bla"};
    public KeyInfo[] info;
    */
    public GUISkin skin;

    public int controlCount = 4;

    public Vector2 windowPos = new Vector2(920, 400);
    public Vector2 windowSize = new Vector2(160, 190);

    public float keyPosY = 50;
    public float keySize = 40;

    public Vector2 textPos = new Vector2(-115, 5);
    public Vector2 textSize = new Vector2(110, 30);



	// Use this for initialization
	void Start () 
    {
        //info = new KeyInfo[] { new KeyInfo().KeyInfo("E", actionsE) };
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void DrawControl()
    {
        // Draw Window
        Rect windowRect = new Rect(windowPos.x, windowPos.y, windowSize.x, windowSize.y);
        GUI.Box(windowRect, "", skin.GetStyle("Inventory"));

        // Draw Keys
        for(int i = 0 ; i < controlCount; i++)
        {
            Rect keyRect = new Rect(windowRect.xMax - keySize, windowRect.yMin + keyPosY * i, keySize, keySize);
            GUI.Box(keyRect, "", skin.GetStyle("Inventory"));

            Rect textRect = new Rect(keyRect.xMin + textPos.x, keyRect.yMin + textPos.y, textSize.x, textSize.y);
            GUI.Box(textRect, "", skin.GetStyle("Inventory"));
        }
    }
}
