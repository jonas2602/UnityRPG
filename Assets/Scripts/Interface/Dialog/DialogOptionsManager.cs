using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DialogOptionsManager : MonoBehaviour
{
    private DialogList database;
    private PlayerDialog playerDialog;
    private Transform activeOptionSprite;

    [SerializeField]
    private GameObject optionPrefab;
    [SerializeField]
    private List<GameObject> dialogOptions = new List<GameObject>();
    [SerializeField]
    private int activeOption; 

    void Awake()
    {
        database = GameObject.FindWithTag("Database").GetComponent<DialogList>();
        playerDialog = GetComponentInParent<PlayerDialog>();
        activeOptionSprite = transform.Find("ActiveOption");
        
        if (!optionPrefab)
        {
            throw new MissingReferenceException();
        }
    }

    // Use this for initialization
    void Start ()
    {
        this.gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update ()
    {
	    if(Input.GetButtonDown("Interact"))
        {
            // start next dialogPart
            StartNextPart();
        }

        if(Input.GetKeyDown("w"))
        {
            // set active to previous
            SetActiveOption(activeOption - 1);
        }

        if(Input.GetKeyDown("s"))
        {
            // set active to next
            SetActiveOption(activeOption + 1);
        }
	}

    public void AddDialogOptions(/*List<DialogTree> childTrees*/ List<Line> lineBranches)
    {
        // activate optionsPanel
        this.gameObject.SetActive(true);
        
        // add dialog options
        for (int i = 0; i < lineBranches.Count; i++)
        {
            // get answer from tree
            // Line answer = database.GetLine(lineBranches[i]);
            
            // create dialog options
            GameObject option = Instantiate(optionPrefab);
            option.name = lineBranches[i].lineTeaser;
            option.transform.SetParent(this.transform);

            // setup line
            option.GetComponent<LineOnObject>().SetupLine(lineBranches[i], i);

            // add to optionsList
            dialogOptions.Add(option);
        }

        // set active to first option
        SetActiveOption(0);
    }

    public void SetActiveOption(int id)
    {
        // validate action
        if(id < 0 || id >= dialogOptions.Count)
        {
            Debug.LogWarning(id + " is out of arrayrange");
            return;
        }

        // set active option
        activeOptionSprite.position = dialogOptions[id].transform.position;
        activeOption = id;
    }

    public void StartNextPart()
    {
        // get next line
        Line nextLine = dialogOptions[activeOption].GetComponent<LineOnObject>().GetStoredLine;

        // reset dialogOptions
        for (int i = 0; i < dialogOptions.Count; i++)
        {
            Destroy(dialogOptions[i]);
        }
        dialogOptions.Clear();

        // continue dialog
        playerDialog.StartNextDialogPart(nextLine);

        // hide panel
        this.gameObject.SetActive(false);
    }
}
