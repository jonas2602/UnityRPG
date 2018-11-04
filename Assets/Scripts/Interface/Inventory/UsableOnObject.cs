using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UsableOnObject : MonoBehaviour
{
    [SerializeField]
    private Image itemImage;
    [SerializeField]
    private Text itemName;
    [SerializeField]
    private Item storedItem;

    public void SetupItem(Item item)
    {
        storedItem = item;
        
        itemImage.sprite = item.itemIcon;
        itemImage.enabled = item.itemIcon == null ? false : true;

        if (itemName)
        {
            itemName.text = item.itemName;
        }
    }

    void Awake()
    {
        itemImage = transform.Find("ItemImage").GetComponent<Image>();
        itemName = GetComponentInChildren<Text>();
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
