using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TargetInfoScript : MonoBehaviour
{
    // aggro, dialog is on every npc
    // health, interation, name is controlled by this script

    // all time references
    private Camera cam;
    private Text nameField;
    private GameObject enemyInfo;
    private HealthbarScript healthbarScript;
    private GameObject interactableInfo;
    private Image interactionImage;
    private Text interactionText;

    // target references
    private Transform player;
    private Transform target;
    private Transform uiDock;
    private Health targetHealth;
    private TargetInformationType informationType;
    private IEnumerator symbolUpdater;

    [SerializeField]
    private float distanceToPlayer;
    [SerializeField]
    private bool interactable;

    public bool GetInteractable
    {
        get
        {
            return this.interactable;
        }
    }


    void Awake()
    {
        cam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        nameField = transform.Find("Name").GetComponent<Text>();
        enemyInfo = transform.Find("EnemyInfo").gameObject;
        interactableInfo = transform.Find("InteractableInfo").gameObject;
        interactionImage = interactableInfo.GetComponentInChildren<Image>();
        interactionText = interactableInfo.GetComponentInChildren<Text>();
        healthbarScript = GetComponentInChildren<HealthbarScript>();

        enemyInfo.SetActive(false);
        interactableInfo.SetActive(false);
    }

	// Use this for initialization
	void Start ()
    {
	
	}

    public void SetTarget(Transform player, Transform target, TargetInformationType informationType, float interactRadius)
    {
        this.player = player;
        this.target = target;
        this.informationType = informationType;

        // set active
        this.gameObject.SetActive(true);

        // find parent
        Transform targetParent = target.Find("UiDock");
        if(targetParent == null)
        {
            targetParent = target;
            Debug.LogWarning(target.name + " has no parent for targetInfo");
        }

        // set parent
        this.transform.SetParent(targetParent);
        this.transform.localPosition = Vector3.zero;
        this.transform.localRotation = Quaternion.identity;

        // get references

        // all
        nameField.text = target.name;
        uiDock = targetParent;

        // enemy
        if (informationType == TargetInformationType.Enemy)
        {
            enemyInfo.SetActive(true);
            targetHealth = transform.root.GetComponent<Health>();
            targetHealth.AddHealthbarScript(healthbarScript);
        }
        // interactable
        else
        {
            interactableInfo.SetActive(true);
            // npc
            if (target.gameObject.layer == LayerMask.NameToLayer("Creature"))
            {
                interactionText.text = "talk";
            }
            // interactable
            else
            {
                switch (target.tag)
                {
                    case "Usable":
                    case "Interactable":
                        {
                            interactionText.text = "interact";
                            break;
                        }
                    case "Loot":
                        {
                            interactionText.text = "take";
                            break;
                        }
                    default:
                        {
                            interactionText.text = "Not setup";
                            break;
                        }
                }
            }
            symbolUpdater = UpdateInferactionSymbol(interactRadius);
            StartCoroutine(symbolUpdater);
        }
    }

    public void ResetTarget()
    {
        // reset parent
        this.transform.SetParent(null);

        // clear references
        // enemy
        if (informationType == TargetInformationType.Enemy)
        {
            targetHealth.RemoveHealthbarScript(healthbarScript);
            enemyInfo.SetActive(false);
        }
        // interactable
        else
        {
            StopCoroutine(symbolUpdater);
            interactableInfo.SetActive(false);
        }
        // set inactive
        this.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // update rotation
        if (uiDock)
        {
            uiDock.forward = cam.transform.forward;
        }
    }

    IEnumerator UpdateInferactionSymbol(float interactRadius)
    {
        for (;;)
        {
            // update interactionSymbol
            distanceToPlayer = Vector3.Distance(player.position, target.position);
            if (distanceToPlayer > interactRadius)
            {
                // show symbol
                interactionImage.enabled = false;
                interactable = false;
            }
            else
            {
                // show "e"
                interactionImage.enabled = true;
                interactable = true;
            }
            
            yield return new WaitForSeconds(0.5f);
        }
    }
}

public enum TargetInformationType
{
    Enemy,
    Interactable
}
