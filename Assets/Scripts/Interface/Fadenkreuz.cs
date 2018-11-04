using UnityEngine;
using System.Collections;

public class Fadenkreuz : MonoBehaviour {

    public Texture2D skin;
    public GUISkin skin2;
    public float TextureScale = 1;
    Rect positionCrossHair;
    public bool CrossHairOn;
    public bool showArrows = true;
    public float posX = 775;
    public float posY = 363;
    public float windowHeight = 189.1f;
    public float windowWidth = 192.7f;
    public float posXA1 = 51.8f;
    public float posYA1 = -3.74f;
    public float posXA2 = 105.6f;
    public float posYA2 = 36f;
    public float posXA3 = 118.5f;
    public float posYA3 = 103.5f;
    public float slotScaleA = 75.9f;
    public float slotScaleW = 135f;
    public float posXW = -5.5f;
    public float posYW = 57.9f;


	// Use this for initialization
	void Start () 
    {
        positionCrossHair = new Rect(((Screen.width - skin.width * TextureScale) / 2), ((Screen.height - skin.height * TextureScale) / 2), skin.width * TextureScale, skin.height * TextureScale);
        posX = (Screen.width - windowWidth) * 0.95f;
        posY = (Screen.height - windowHeight) * 0.95f;
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    void OnGUI()
    {
        if (CrossHairOn)
        {
            GUI.DrawTexture(positionCrossHair, skin);
        }

        if(showArrows)
        {
            ArrowWindow();
        }
    }

    void ArrowWindow()
    {
        // Draw Window
        GUI.Box(new Rect(posX, posY, windowWidth, windowHeight), "", skin2.GetStyle("Inventory"));

        // Draw WeaponSlot
        GUI.Box(new Rect(posX + posXW, posY + posYW, slotScaleW, slotScaleW), "", skin2.GetStyle("Circle"));

        // Draw ArrowSlot
        GUI.Box(new Rect(posX + posXA1, posY + posYA1, slotScaleA, slotScaleA), "", skin2.GetStyle("Circle"));

        // Draw ArrowSlot
        GUI.Box(new Rect(posX + posXA2, posY + posYA2, slotScaleA, slotScaleA), "", skin2.GetStyle("Circle"));

        // Draw ArrowSlot
        GUI.Box(new Rect(posX + posXA3, posY + posYA3, slotScaleA, slotScaleA), "", skin2.GetStyle("Circle"));
    }
}
