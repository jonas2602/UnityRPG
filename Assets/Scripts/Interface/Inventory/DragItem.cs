using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragItem : MonoBehaviour , IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public ItemOnObject itemOnObject;
    public CanvasGroup canvasGroup;
    public Transform draggedItemParent;

    public Transform startSlot;
    public Transform targetSlot;
    public int draggingStatus = -1;

    // Use this for initialization
    void Start ()
    {
        itemOnObject = GetComponent<ItemOnObject>();
        canvasGroup = GetComponent<CanvasGroup>();
        draggedItemParent = transform.root.Find("DraggedItem - Image");
	}
	
	// Update is called once per frame
	void Update ()
    {

	}

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // Debug.Log("OnBeginDrag");
            
            // save startSlot
            startSlot = this.transform.parent;

            // create copy of this item
            Transform duplicate = Instantiate(this.transform);
            duplicate.SetParent(startSlot.transform);
            duplicate.position = this.transform.position;
            duplicate.GetComponent<CanvasGroup>().alpha = 0.5f;

            // set correct item
            duplicate.GetComponent<ItemOnObject>().SetupItem(itemOnObject.GetStoredItem);

            // move to draggingItem
            this.transform.SetParent(draggedItemParent);
          
            // stop tooltip
            GetComponent<ShowTooltip>().tooltip.DeactivateTooltip();
            
            // stop blocking raycasts
            canvasGroup.blocksRaycasts = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Debug.Log("OnDrag");
        // set item to cursor
        this.transform.position = eventData.position;

        // check for slot
        GameObject mouseOver = eventData.pointerEnter;
        // over ui element?
        if (mouseOver)
        {
            Transform newSlot = null;
            if (mouseOver.GetComponentInParent<SlotInfo>())
            {
                newSlot = mouseOver.GetComponentInParent<SlotInfo>().transform;
            }

            // over slot?
            if (newSlot)
            {
                // slot is new?
                if (newSlot != targetSlot)
                {
                    // check new slot
                    targetSlot = newSlot;
                    draggingStatus = ItemManager.ItemMatch(startSlot, targetSlot);
                }
            }
            // not over slot
            else
            {
                draggingStatus = 0;
            }
        }
        // not over ui
        else
        {
            draggingStatus = 0;
        }

        // Debug.Log(draggingStatus);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Debug.Log("OnEndDrag");
        
        // set or reset item
        ItemManager.MoveItems(startSlot, targetSlot, draggingStatus);
        // GetComponentInParent<PlayerInventory>().OnUpdateItemList();
        // transform.root.GetComponentInChildren<Inventory>().OnUpdateItemsInInventory();
        // Destroy this item
        Destroy(this.gameObject);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // rightclick
        if( eventData.button == PointerEventData.InputButton.Right)
        {
            // Debug.Log(itemOnObject.GetStoredItem.itemName + " wants to move");
            
            // move to other inventory
            ItemManager.SendToOtherInventory(this.transform.parent);
            /*
            // test found slot
            draggingStatus = ItemManager.ItemMatch(startSlot, targetSlot);

            // move item
            ItemManager.MoveItems(startSlot, targetSlot, draggingStatus);*/
        }
    }
}
