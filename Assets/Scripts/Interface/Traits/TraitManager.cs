using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TraitManager : MonoBehaviour
{
    [SerializeField]
    private Trait storedTrait;

    private Image traitImage;
    private Text traitName;

    public Vector3 localPosition;


    void Awake()
    {
        traitImage = transform.Find("TraitImage").GetComponent<Image>();
        traitName = transform.Find("TraitName").GetComponent<Text>();
    }

    public void SetupTrait(Trait trait)
    {
        storedTrait = trait;
        this.name = trait.name;

        traitImage.sprite = trait.icon;

        string nameString = trait.name;
        if(trait.maxLevel > 1)
        {
            nameString += "(" + trait.curLevel + "/" + trait.maxLevel + ")";
        }
        traitName.text = nameString;

        // set position
        RectTransform parent = transform.parent.GetComponent<RectTransform>();
        float posX = trait.posXPercent / 100f * parent.sizeDelta.x;
        float posY = trait.pointsToLearn / 100f * parent.sizeDelta.y;
        GetComponent<RectTransform>().anchoredPosition3D = new Vector3(posX, posY, 0);
        // Debug.Log(trait.name + ": " + parent.sizeDelta + ", " + posX + ", " + posY + ", " + transform.localPosition);
    } 

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        
	}
}
