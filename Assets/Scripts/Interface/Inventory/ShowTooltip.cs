using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class ShowTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TooltipScript tooltip;
    public ItemOnObject itemOnObject;

    public float tooltipDistanceX = 5;

	// Use this for initialization
	void Awake () 
    {
        itemOnObject = GetComponent<ItemOnObject>();
    }

    void Start()
    {
        tooltip = transform.root.GetComponentsInChildren<TooltipScript>(true)[0];
    }

    // Update is called once per frame
    void Update ()
    {
	    
	}
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Debug.Log("OnPointerEnter");
        if (eventData.pointerDrag == null)
        {
            // show Tooltip

            // get rightdown corner
            Vector3[] slotCorners = new Vector3[4];
            GetComponent<RectTransform>().GetWorldCorners(slotCorners);

            // positioning
            Vector2 tooltipPos = new Vector2(slotCorners[3].x + tooltipDistanceX, slotCorners[3].y + GetComponent<RectTransform>().rect.height / 2);

            // activate tooltip
            tooltip.ActivateTooltip(eventData.pressEventCamera, tooltipPos, itemOnObject.GetStoredItem);

            return;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Debug.Log("OnPointerExit");
        tooltip.DeactivateTooltip();
    }
}
