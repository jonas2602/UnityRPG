using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TraitLineManager : MonoBehaviour {

    [SerializeField]
    private TraitLine storedTraitLine;
    
    private Text lineName;
    private Text lineLevel;
    private Image lineProgressBar;
    private Transform traitParent;

    [SerializeField]
    private List<Transform> traits = new List<Transform>();

    void Awake()
    {
        lineName = transform.Find("LineName").GetComponent<Text>();
        lineLevel = transform.Find("LineLevel").GetComponent<Text>();
        lineProgressBar = transform.Find("LineProgressBar/Progress").GetComponent<Image>();
        traitParent = transform.Find("TraitParent");
    }

    public void SetupTraitLine(TraitLine traitLine, GameObject traitPrefab)
    {
        // setup line
        storedTraitLine = traitLine;
        this.name = traitLine.name;
        
        lineName.text = traitLine.name;

        // Add traits
        for(int i = 0; i < storedTraitLine.lineTraits.Count;i++)
        {
            // add to overview
            Transform traitTransform = Instantiate(traitPrefab).transform;
            traitTransform.SetParent(traitParent);
            traits.Add(traitTransform);

            traitTransform.GetComponent<TraitManager>().SetupTrait(storedTraitLine.lineTraits[i]);
        }
    } 

    void OnEnable()
    {
        // update progress
        lineLevel.text = storedTraitLine.level.ToString();
        lineProgressBar.fillAmount = storedTraitLine.levelProgress;
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
