using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TooltipScript : MonoBehaviour {

    public GameObject tooltipObject;
    public RectTransform tooltipRectTransform;

    public GameObject statPanelPrefab;
    public GameObject statPrefab;
    public GameObject runePrefab;

    // TypeInfo
    public Text nameField;
    public Text rarityField;
    public Text typeField;

    // StatInfo
    public GameObject statPanel;
    public List<GameObject> stats = new List<GameObject>();
    public GameObject runePanel;
    public List<GameObject> runes = new List<GameObject>();
    public Text requiredLevelField;

    // ItemInfo
    public Text priceField;
    public Text weightField;


    void Awake()
    {
        tooltipObject = this.gameObject;
        tooltipRectTransform = GetComponent<RectTransform>();


        nameField = transform.Find("Name - Text").GetComponent<Text>();
        rarityField = transform.Find("TypeInfo - Panel/Rarity - Text").GetComponent<Text>();
        typeField = transform.Find("TypeInfo - Panel/Type - Text").GetComponent<Text>();

        statPanel = transform.Find("Stat - Panel").gameObject;
        runePanel = transform.Find("Rune - Panel").gameObject;
        requiredLevelField = transform.Find("RequiredLevel - Text").GetComponent<Text>();

        priceField = transform.Find("ItemInfo - Panel/Price - Text").GetComponent<Text>();
        weightField = transform.Find("ItemInfo - Panel/Weight - Text").GetComponent<Text>();
    }


	// Use this for initialization
	void Start ()
    {

    }
	

	// Update is called once per frame
	void Update ()
    {
	
	}


    public void ActivateTooltip(Camera eventCam, Vector2 screenPoint, Item item)
    {
        // get local position
        Vector2 localSlotPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(), screenPoint, eventCam, out localSlotPosition);

        // move to upperleft corner
        // localSlotPosition += new Vector2(tooltipRectTransform.rect.width / 2, -tooltipRectTransform.rect.height / 2);
        tooltipObject.GetComponent<RectTransform>().localPosition = localSlotPosition;

        // create tooltip
        CreateTooltip(item);

        // show tooltip
        tooltipObject.SetActive(true);
    }


    public void DeactivateTooltip()
    {
        tooltipObject.SetActive(false);
        for (int i = 0; i < stats.Count; i++)
        {
            Destroy(stats[i]);
        }
        stats.Clear();
    }


    void CreateTooltip(Item item)
    {
        // float tooltipHeight = 0;
        // type info
        nameField.text = item.itemName;
        // tooltipHeight += nameField.GetComponent<RectTransform>().rect.height;

        rarityField.text = item.itemRarity.ToString();
        // tooltipHeight += rarityField.GetComponent<RectTransform>().rect.height;

        typeField.text = item.itemType.ToString();
        // tooltipHeight += typeField.GetComponent<RectTransform>().rect.height;

        // stat info
        for(int i = 0; i < item.itemAttributes.Count; i++)
        {
            GameObject stat = Instantiate(statPrefab);
            stats.Add(stat);
            stat.transform.SetParent(statPanel.transform);
            stat.transform.position = Vector3.zero;
            stat.GetComponent<Text>().text = "+" + item.itemAttributes[i].attributeValue + " " + item.itemAttributes[i].attributeName; 
        }
        // tooltipHeight += statPanel.GetComponent<RectTransform>().rect.height;
        for (int i = 0; i < 0; i++)
        {
            GameObject rune = Instantiate(runePrefab);
            runes.Add(rune);
            rune.transform.SetParent(runePanel.transform);
        }
        // tooltipHeight += runePanel.GetComponent<RectTransform>().rect.height;

        requiredLevelField.text = "Required Level " + item.requiredLevel;
        // tooltipHeight += requiredLevelField.GetComponent<RectTransform>().rect.height;
        
        // item data
        priceField.text = item.itemPrice.ToString();
        // tooltipHeight += priceField.GetComponent<RectTransform>().rect.height;

        weightField.text = item.itemWeight.ToString();
        // tooltipHeight += weightField.GetComponent<RectTransform>().rect.height;

        // set tooltip height
        // tooltipRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, tooltipHeight);
    }
}
