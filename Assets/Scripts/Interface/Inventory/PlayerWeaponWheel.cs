using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerWeaponWheel : UIWindow
{
    private EquipmentManager equipmentManager;
    private UsableInfo usableInfo;

    private Transform arrow;
    private Transform arrowedSlot;
    private ActiveWheelSlot[] activeSlots = new ActiveWheelSlot[2];

    public Vector2 startDirection;
    public WheelSlot[] wheelSlots;
    public int startToolSlotsId;

    public int bestSlot = 0;
    public int[] selectedSlots;  // { main , off , tool }    
    // public Transform[] offSlots = new Transform[3];

    public float rotationSpeed;
    
    public override void SetupPlayer(GameObject player)
    {
        equipmentManager = player.GetComponent<EquipmentManager>();
        usableInfo = transform.root.GetComponentInChildren<UsableInfo>(true);

        // update hands
        Item[] equipedUsables = equipmentManager.UpdateSelectedSlots(selectedSlots);
        usableInfo.UpdateUsables(equipedUsables);
    }


    // Use this for initialization
    void Awake()
    {
        // setup refs
        arrow = transform.Find("Arrow");
        arrowedSlot = transform.Find("ArrowedSlot");
        activeSlots[0] = transform.Find("ActiveToolSlot").GetComponent<ActiveWheelSlot>();
        activeSlots[1] = transform.Find("ActiveWeaponSlot").GetComponent<ActiveWheelSlot>();

        // get slots
        wheelSlots = GetComponentsInChildren<WheelSlot>();
        // get border between weapon,tool slots
        for (int i = 0; i < wheelSlots.Length;i++)
        {
            if(!(wheelSlots[i] is WheelWeaponSlot))
            {
                startToolSlotsId = i;
                break;
            }
        }

        selectedSlots = new int[] { -1, -1 , -1};        // references equipedweapon arrayPos
        startDirection = new Vector2(0, 1);
        rotationSpeed = 0.05f;
        
    }

    void Start()
    {
        PlayerProfil playerProfil = GetComponentInParent<UIManager>().GetWindow<PlayerProfil>() as PlayerProfil;
        
        // get reference weaponSlots
        SlotInfo[] weaponSets = playerProfil.GetDrawableWeapons();
        for(int i = 0; i < weaponSets.Length;i++)
        {
            wheelSlots[i].SetupSlot(weaponSets[i]);
        }

        // get reference toolSlots
        SlotInfo[] toolSlots = playerProfil.GetDrawableTools();
        for (int i = 0; i < toolSlots.Length; i++)
        {
            wheelSlots[startToolSlotsId + i].SetupSlot(toolSlots[i]);
        }


        // set start Weaponet/tool
        // get weaponset
        wheelSlots[0].GetReferencedItem(ref selectedSlots);

        // update ui
        activeSlots[0].SetParent(wheelSlots[0].transform, 0);

        // get tool
        wheelSlots[startToolSlotsId].GetReferencedItem(ref selectedSlots);

        // update ui
        activeSlots[1].SetParent(wheelSlots[startToolSlotsId].transform, startToolSlotsId);
    }

    // Update is called once per frame
    void Update()
    {
        // GetMouseMovement
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        if (mouseX != 0 || mouseY != 0)
        {
            Vector2 mouseMovement = new Vector2(mouseX, mouseY);

            // get mouseAngle relative to startDirection
            float mouseMoveAngle = GetAngle(mouseMovement);

            // difference between mouseDirection, arrowDirection
            float difference = Mathf.DeltaAngle(arrow.eulerAngles.z, mouseMoveAngle);

            // Calculate new arrowDirection
            if (Mathf.Abs(difference) > 2f)
            {
                CalculkateNewArrowAngle(difference, mouseMovement);
            }

            // Update currentSlot
            bestSlot = BestFitSlot();

            // Update arrowedSlot
            Vector3 wheelCenter = GetComponent<RectTransform>().position;
            Vector2 slotDirection = wheelSlots[bestSlot].transform.position - wheelCenter;
            float angle = GetAngle(slotDirection);
            arrowedSlot.eulerAngles = new Vector3(0, 0, angle);

        }

        if (Input.GetButtonDown("LeftKlick"))
        {
            // setup animator
            if(wheelSlots[bestSlot] is WheelWeaponSlot)
            {
                // update weaponSet
                // Debug.Log("update weaponSet");

                // get weaponset
                wheelSlots[bestSlot].GetReferencedItem(ref selectedSlots);

                // update hands
                Item[] equipedUsables = equipmentManager.UpdateSelectedSlots(selectedSlots);
                usableInfo.UpdateUsables(equipedUsables);

                // update ui
                activeSlots[0].SetParent(wheelSlots[bestSlot].transform, bestSlot);

            }
            else
            {
                // update tool
                // Debug.Log("Update tool");

                // get tool
                wheelSlots[bestSlot].GetReferencedItem(ref selectedSlots);

                // update hands
                Item[] equipedUsables = equipmentManager.UpdateSelectedSlots(selectedSlots);
                usableInfo.UpdateUsables(equipedUsables);

                // update ui
                activeSlots[1].SetParent(wheelSlots[bestSlot].transform, bestSlot);
            }
        }
    }


    float GetAngle(Vector2 moveDirection)
    {
        Vector2 normalizedDirection = Vector3.Normalize(moveDirection);
        // Debug.DrawRay(GetComponent<RectTransform>().position, normalizedDirection * 100);
        float mouseAngle180 = Vector2.Angle(startDirection, normalizedDirection);

        // mouseMoveDirection.y > 0 -> mouseAngle180 = mouseAngle180 else mouseAngle180 = -mouseAngle180
        return moveDirection.x < 0 ? mouseAngle180 : -mouseAngle180;
    }


    void CalculkateNewArrowAngle(float difference, Vector2 mouseMovement)
    {

        // float mouseSpeed = Vector2.SqrMagnitude(mouseMovement);
        float newAngle = arrow.eulerAngles.z + difference * rotationSpeed;
        arrow.eulerAngles = new Vector3(0, 0, newAngle);
        /*
        newAngle %= 360;
        if (newAngle < 0)
        {
            newAngle += 360;
        }
        Debug.Log("newAngle: " + newAngle + ", currentAngle: " + currentAngle + ", mouseAngle: " + mouseAngle360 + ", mouseAngle180: " + mouseAngle180);
        if (mouseAngle180 > 0)
        {
            Mathf.Clamp(newAngle, currentAngle, mouseAngle360);
        }
        else
        {
            Mathf.Clamp(newAngle, mouseAngle360, currentAngle);
        }

        currentAngle = newAngle;
        */
    }


    int BestFitSlot()
    {
        int bestSlot = -1;
        float bestAngle = -1;

        Vector2 arrowDirection = arrow.up;
        Vector3 wheelCenter = GetComponent<RectTransform>().position;
        for (int i = 0; i < wheelSlots.Length; i++)
        {

            Vector2 slotDirection = wheelSlots[i].transform.position - wheelCenter;
            float angle = Mathf.Abs(Vector2.Angle(arrowDirection, slotDirection));
            if (angle < bestAngle || bestAngle == -1)
            {
                bestAngle = angle;
                bestSlot = i;
            }
        }

        return bestSlot;
    }
    

    public override void OpenWindow()
    {
        for(int i = 0; i < wheelSlots.Length;i++)
        {
            wheelSlots[i].UpdateConnection();
        }

        base.OpenWindow();
    }
}
