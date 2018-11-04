using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public ItemStack itemStack;
    public Transform newParentSlot;
    
    public Transform startSlot;

    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag");
        // create dragged item
        startSlot = this.transform.parent;
        newParentSlot = this.transform.parent;
        // remove stored item
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("OnDrag");

        // set item to cursor
        this.transform.position = eventData.position;
        
        if (newParentSlot != this.transform.parent)
        {
            this.transform.SetParent(newParentSlot);
        }
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag");
        // check if allowed to set
        // set or reset
        // destroy dragged item
    }
}

