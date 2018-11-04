using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class LineManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    private int optionId; 


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // set as active dialogOption
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // start next dialogPart
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // set as inactive dialogOption
    }
}
