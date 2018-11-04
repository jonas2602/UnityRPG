using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TreasureController : MonoBehaviour
{


    Animator animother;
    private bool entered = false;
    public AnimatorStateInfo animInfo;
    public GUISkin skin;
    bool animator = false;
    string OtherTag;
    GameObject gameobject;
    private OldInventory inventory;

    

    // Use this for initialization
    void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Interface").GetComponent<OldInventory>();
    }

    void OnTriggerEnter(Collider other)
    {
        OtherTag = other.tag;
        gameobject = other.gameObject;
        if (other.tag == "Usable" || other.tag == "Treasure")
        {
            if (other.tag == "Treasure")
            {
                animother = other.GetComponent<Animator>();
                animator = true;
            }
            entered = true;
        }
    }

    void OnTriggerExit()
    {
        entered = false;
        animator = false;
    }

    void Update()
    {
        if (animator)
        {
            animInfo = animother.GetCurrentAnimatorStateInfo(0);
        }
    }

    void OnGUI()
    {
        if (entered)
        {
            if ((OtherTag == "Treasure" && animInfo.IsTag("close")) || OtherTag == "Usable")
            {
                ShowButton();
            }
            if (OtherTag == "Treasure" && animInfo.IsTag("open"))
            {
                float v = Input.GetAxis("Vertical");
                float h = Input.GetAxis("Horizontal");
                if (Input.GetButton("Interact") || v != 0 || h != 0)
                {
                    animother.SetBool("open", false);
                }
            }
        }
    }

    void ShowButton()
    {
        Rect slotRect = new Rect(400, 400, 30, 30);
        GUI.Box(slotRect, "F", skin.GetStyle("Tooltip"));

        if (slotRect.Contains(Event.current.mousePosition))
        {
            if (Input.GetButton("LeftKlick"))
            {
                Pressed();
            }
        }

        if (Input.GetButton("Interact"))
        {
            Pressed();
        }
    }

    void Pressed()
    {
        if(OtherTag == "Treasure")
        {
            animother.SetBool("open", true);
        }

        if(OtherTag == "Usable")
        {
            string meshName = gameobject.GetComponent<MeshFilter>().name;
            inventory.AddItem(meshName);
            Destroy(gameobject);
            entered = false;
        }
    }
}