using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LineOnObject : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
{
    [SerializeField]
    private Line storedLine;
    [SerializeField]
    private int optionId;

    private Image lineSymbol;
    private Text lineText;
    private DialogOptionsManager dialogOptionsManager;

    
    void Awake()
    {
        lineSymbol = GetComponentInChildren<Image>();
        lineText = GetComponentInChildren<Text>();
    }

    // Use this for initialization
    void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public void SetupLine(Line line, int id)
    {
        optionId = id;
        storedLine = line;

        // set icon
        if (line.lineIcon)
        {
            lineSymbol.sprite = line.lineIcon;
        }
        else
        {
            lineSymbol.enabled = false;
        }
        // set text
        lineText.text = line.lineTeaser;

        dialogOptionsManager = GetComponentInParent<DialogOptionsManager>();

        if (!dialogOptionsManager)
        {
            throw new MissingReferenceException();
        }
    }

    public Line GetStoredLine
    {
        get
        {
            return this.storedLine; 
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // set as active dialogOption
        dialogOptionsManager.SetActiveOption(optionId);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // start next dialogPart
        dialogOptionsManager.StartNextPart();
    }
}
