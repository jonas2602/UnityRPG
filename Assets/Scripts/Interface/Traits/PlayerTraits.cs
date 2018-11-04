using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerTraits : UIWindow
{
    private TraitList traitList;
    [SerializeField]
    private GameObject traitLinePrefab;
    [SerializeField]
    private GameObject traitPrefab;

    private Transform traitLinesParent;
    private Transform traitTreesParent;
    [SerializeField]
    private List<Transform> traitLines = new List<Transform>();
    [SerializeField]
    private int activeTraitLineIndex;

    private Vector2 regularSize = new Vector2(280f, 455f);
    private Vector2 extendedSize = new Vector2(300f, 475f);

    void Awake()
    {
        traitList = GameObject.FindWithTag("Database").GetComponent<TraitList>();
        traitLinesParent = transform.Find("TraitLinesParent");
        traitTreesParent = transform.Find("TraitTreesParent");

        if (!traitLinePrefab)
        {
            Debug.LogError("traitLinePrefab missing");
        }
        if (!traitPrefab)
        {
            Debug.LogError("traitPrefab missing");
        }
    }

	// Use this for initialization
	void Start ()
    {
        List<TraitLine> existingTraitLines = traitList.GetTraitLineList();
	    
        // create traitLines
        for(int i = 0; i < existingTraitLines.Count;i++)
        {
            Transform lineTransform = Instantiate(traitLinePrefab).transform;
            lineTransform.SetParent(traitLinesParent);
            traitLines.Add(lineTransform);

            lineTransform.GetComponent<TraitLineManager>().SetupTraitLine(existingTraitLines[i], traitPrefab);
        }

        // create traitTrees

        // center active line
        activeTraitLineIndex = traitLines.Count / 2;
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if(Input.GetKeyDown("a"))
        {
            // Debug.Log("oldActive: " + traitLines[activeTraitLineIndex].name);

            // shrink old
            ResetActiveTraitLine();

            // move sibling
            traitLinesParent.GetChild(traitLines.Count - 1).SetAsFirstSibling(); // traitLines[traitLines.Count - 1].SetAsFirstSibling();
            
            /*// move in list
            Transform traitLine = traitLines[traitLines.Count - 1];
            traitLines.RemoveAt(traitLines.Count - 1);
            traitLines.Insert(0, traitLine);*/
            
            // set new active
            activeTraitLineIndex = (activeTraitLineIndex + traitLines.Count - 1) % traitLines.Count;

            // extend new
            SetActiveTraitLine();
            
            // Debug.Log("newActive: " + traitLines[activeTraitLineIndex].name);
        }
        if(Input.GetKeyDown("d"))
        {
            // shrink old
            ResetActiveTraitLine();

            // move sibling
            traitLinesParent.GetChild(0).SetAsLastSibling(); // traitLines[0].SetAsLastSibling();

            /*// move in list
            Transform traitLine = traitLines[0];
            traitLines.RemoveAt(0);
            traitLines.Add(traitLine);*/

            // set new active
            activeTraitLineIndex = (activeTraitLineIndex + traitLines.Count + 1) % traitLines.Count;

            // extend new
            SetActiveTraitLine();
        }
        if (Input.GetKeyDown("w"))
        {
            // move in line
        }
        if (Input.GetKeyDown("s"))
        {
            // move out of line
        }
    }

    public override void OpenWindow()
    {
        base.OpenWindow();

        CenterActiveLine();
        SetActiveTraitLine();
    }

    void SetActiveTraitLine()
    {
        LayoutElement activeLine = traitLines[activeTraitLineIndex].GetComponent<LayoutElement>();

        // extend
        activeLine.minWidth = extendedSize.x;
        activeLine.minHeight = extendedSize.y;
    }

    void ResetActiveTraitLine()
    {
        LayoutElement activeLine = traitLines[activeTraitLineIndex].GetComponent<LayoutElement>();

        // shrink
        activeLine.minWidth = regularSize.x;
        activeLine.minHeight = regularSize.y;
    }

    void CenterActiveLine()
    {
        float difference = Screen.width / 2 - traitLines[activeTraitLineIndex].position.x;
        Vector3 newPosition = traitLinesParent.position + new Vector3(difference, 0, 0);

        traitLinesParent.position = newPosition;
    }
}
