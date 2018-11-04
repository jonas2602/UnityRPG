using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponWheel : MonoBehaviour 
{
    public GUISkin skin;
    public Profil profil;

    public ItemStack[] itemList = new ItemStack[8];
    public int slotCount = 8;
    public int currentMainWeapon;
    public int currentOffWeapon;
    public int currentSlot;
    public float mouseAngle180 = 0;
    public float mouseAngle360 = 0;
    public float currentAngle = 0;
    public float rotationSpeed = 1f;
    public float mouseSpeed;
    public float mouseSensitivity = 1;

    // Window
    public float posX;
    public float posY;
    public float windowWidth;

    // slots
    public float slotWidth = 60;
    public float slotPosX;
    public float slotPosY;
    public float slotCenterDistance;

    // outWheel
    public float posXWeapon;
    public float posYWeapon;
    public float widthWeapon = 70;
    
    
    public int SetCurrentWeapon
    {
        set
        {
            currentMainWeapon = value;
            currentAngle = value * 45;
        }
    }

    public Vector2 GetCurrentWeapons
    {
        get
        {
            return new Vector2(currentMainWeapon,currentOffWeapon);
        }
    }


	// Use this for initialization
	void Start () 
    {
        profil = GetComponent<Profil>();
        windowWidth = Screen.height * 0.9f;
        posX = (Screen.width - windowWidth)/2;
        posY = (Screen.height - windowWidth) / 2;
        slotPosX = (Screen.width - slotWidth) / 2;
        slotPosY = (Screen.height - slotWidth) / 2;
        posXWeapon = (Screen.width - widthWeapon) * 0.1f;
        posYWeapon = (Screen.height - widthWeapon) * 0.9f;
        slotCenterDistance = windowWidth / 2 - slotWidth;
	}
	
	// Update is called once per frame
    void Update()
    {
        
    }

    public void DrawWheel()
    {
        // Draw Window
        Rect windowRect = new Rect(posX, posY, windowWidth, windowWidth);
        GUI.Box(windowRect, "", skin.GetStyle("Circle"));

        // Draw WeaponSlots
        for (int i = 0; i < slotCount; i++)
        {
            // calculate slot position
            float slotCenterX = windowRect.center.x + Mathf.Cos(i * 360 / slotCount * Mathf.Deg2Rad) * slotCenterDistance;
            float slotCenterY = windowRect.center.y + Mathf.Sin(i * 360 / slotCount * Mathf.Deg2Rad) * slotCenterDistance;
            Rect slotRect = new Rect(slotCenterX - slotWidth / 2, slotCenterY - slotWidth / 2, slotWidth, slotWidth);
            GUI.Box(slotRect, "" + i, skin.GetStyle("Inventory"));

            // Slot is used
            if (itemList[i].slotAmount != 0)
            {
                // GUI.DrawTexture(slotRect, itemList[i].slotItem.itemIcon);
            }
            // not used
            else
            {
                Texture texture = Resources.Load<Texture>("SlotIcons/" + ((WeaponType)i).ToString());
                if (texture)
                {
                    GUI.DrawTexture(slotRect, texture);
                }
            }
        }

        // currentWeapon
        Rect currentRect = new Rect(slotPosX, slotPosY, slotWidth, slotWidth);
        GUI.Box(currentRect, "", skin.GetStyle("Inventory"));
        Texture curTexture = Resources.Load<Texture>("SlotIcons/" + ((WeaponType)currentSlot).ToString());
        if (curTexture)
        {
            GUI.DrawTexture(currentRect, curTexture);
        }

        // SetWeapon
        if (Event.current.button == 0 && Event.current.type == EventType.MouseDown)
        {
            Usable item = itemList[currentSlot].slotItem as Usable;

            // switch main
            if (item.handInfo.preferedHand == ItemHand.Right)
            {
                currentMainWeapon = currentSlot;
            }
            // switch off
            else
            {
                currentOffWeapon = currentSlot;
            }
        }

        // arrow
        GUIUtility.RotateAroundPivot(currentAngle, windowRect.center);
        GUI.Box(new Rect(posX + windowWidth / 2 + 50, posY + windowWidth / 2 - 35, 130, 70), "", skin.GetStyle("Inventory"));

        // GetMouseDirection
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        if (mouseX != 0 || mouseY != 0)
        {
            GetMouseAngle(mouseX, mouseY);

            float difference = Mathf.DeltaAngle(currentAngle, mouseAngle360);

            if (Mathf.Abs(difference) > 5)
            {
                CalculkateCurrentAngle(difference, new Vector2(mouseX, mouseY));
            }

            int nextSlot = (int)(currentAngle + 22.5) / 45;
            nextSlot %= 8;
            currentSlot = nextSlot;
            
        }
    }

    public void DrawActiveWeapon()
    {
        Rect weaponRect = new Rect(posXWeapon, posYWeapon, widthWeapon, widthWeapon);
        GUI.Box(weaponRect, "", skin.GetStyle("Inventory"));
        GUI.DrawTexture(weaponRect, profil.GetSlotSkin(currentMainWeapon, 1));
    }

    void GetMouseAngle(float mouseX, float mouseY)
    {
        Vector2 moveDirection = new Vector2(mouseX, mouseY);
        Vector2 normalizedDirection = Vector3.Normalize(moveDirection);
        mouseAngle180 = Vector2.Angle(new Vector2(1, 0), normalizedDirection);

        // y > 0 -> mouseAngle = 360 - newAngle else mouseAngle = newAngle
        mouseAngle360 = mouseY > 0 ? 360 - mouseAngle180 : mouseAngle180;
        mouseAngle180 = mouseY > 0 ? -mouseAngle180 : mouseAngle180;
    }

    void CalculkateCurrentAngle(float difference, Vector2 moveDirection)
    {
        mouseSpeed = Vector2.SqrMagnitude(moveDirection) * mouseSensitivity;
        float newAngle = currentAngle + difference / Mathf.Abs(difference) /* * mouseSpeed*/ * rotationSpeed;
        
        newAngle %= 360;
        if(newAngle < 0)
        {
            newAngle += 360;
        }
      /*Debug.Log("newAngle: " + newAngle + ", currentAngle: " + currentAngle + ", mouseAngle: " + mouseAngle360 + ", mouseAngle180: " + mouseAngle180);
        if (mouseAngle180 > 0)
        {
            Mathf.Clamp(newAngle, currentAngle, mouseAngle360);
        }
        else
        {
            Mathf.Clamp(newAngle, mouseAngle360, currentAngle);
        }*/

        currentAngle = newAngle;
    }

    public void CreateList()
    {
        int count = 0;

        // add weaponSets (3)
        for (int i = 0; i < profil.weaponSet.Count; i++)
        {
            Debug.Log(profil.weaponSet[i].slotItem.GetType().ToString());
            if (profil.weaponSet[i].slotItem.GetType().ToString() != "Munition")
            {
                itemList[count] = profil.weaponSet[i];
                count++;
            }
        }

        // add tools (2)
        // add food / tränke (2)

    }
}
