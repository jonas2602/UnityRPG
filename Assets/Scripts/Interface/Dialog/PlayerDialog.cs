using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerDialog : UIWindow
{
    private UIManager uiManager; 

    private DialogList database;
    [SerializeField]
    private List<Line> availableRootLines;
    [SerializeField]
    private List<Line> availableGreetings;
    private Text spokenText;
    private DialogOptionsManager dialogOptionsManager;
    
    public GameObject partner;
    // public DialogStatusManager dialogStatusManager;
    public GameObject player;
    public IEnumerator activeDialogPart;

    public void SetAvailableLines(List<Line> branches, List<Line> greetings)
    {
        availableRootLines = branches;
        availableGreetings = greetings;
    }


    void Awake()
    {
        uiManager = GetComponentInParent<UIManager>();
        database = GameObject.FindWithTag("Database").GetComponent<DialogList>();
        spokenText = transform.Find("SpokenText").GetComponent<Text>();
        dialogOptionsManager = GetComponentInChildren<DialogOptionsManager>();
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(dialogPart);
    }

    public void StartDialog(DialogManager dialogManager)
    {
        this.partner = dialogManager.dialogPartner;
        // dialogStatusManager = partner.GetComponentInChildren<DialogStatusManager>();
        this.player = dialogManager.gameObject;
        database.AddLines(partner);
        // DialogTree dialogPart = dialogStatusManager.GetStartPoint();
        // Line line = database.GetLine("S");
        if (availableGreetings.Count > 0)
        {
            Line greeting = availableGreetings[Random.Range(0, availableGreetings.Count - 1)];
            StartNextDialogPart(greeting);
        }
        else
        {
            Debug.LogError("No Greetings available to start Dialog");
        }
    }

    public void StartNextDialogPart(Line line)
    {
        activeDialogPart = ContinueDialog(line);
        StartCoroutine(activeDialogPart);
    }

    // act until next break point or finish
    IEnumerator ContinueDialog(Line dialogPart)
    {
        // speak lines
        // Debug.Log("speak lines");
        for (int i = 0; i < dialogPart.lineTexts.Count; i++)
        {
            LinePart line = dialogPart.lineTexts[i];

            // add line to spokenText
            spokenText.text = line.text;

            // start audio clip
            if (line.audio != null)
            {
                // get speaker
                // AudioSource source = line.speaker.aliasGameObject.GetComponent<AudioSource>();
                // yield return StartCoroutine(PlayAudioClip(line.audio, source));

                // camera focus speaker
            }
            // beta wait for next
            else
            {
                yield return StartCoroutine(WaitForNextLine(line.text));
            }
        }

        // do action if available
        // Debug.Log("perform action");
        if (dialogPart.lineAction != LineAction.Nothing)
        {
            PerformAction(dialogPart);
        }
        /*
        // change dialogTree
        if (dialogPart.treeChanges != null)
        {
            for (int i = 0; i < dialogPart.treeChanges.Length; i++)
            {
                dialogPart.treeChanges[i].ChangeTree(dialogStatusManager);
            }
        }*/

        // finish dialog?
        if(dialogPart.lineFinishDialog)
        {
            FinishDialog();
        }
        // continue Branch?
        else if (dialogPart.lineBranches.Count > 0)
        {
            // show dialog options
            // Debug.Log("show dialogoptions");
            dialogOptionsManager.AddDialogOptions(dialogPart.lineBranches);
        }
        // finish Branch
        else
        {
            dialogOptionsManager.AddDialogOptions(availableRootLines);
        }
    }

    IEnumerator PlayAudioClip(AudioClip clip, AudioSource source)
    {
        // add clip to avatar
        source.clip = clip;

        // start audio
        source.Play();

        //wait for end of clip
        while (source.isPlaying)
        {
            yield return null;
        }
    }

    IEnumerator WaitForNextLine(string line)
    {
        float startTime = Time.time;
        float waitTime = line.Length * 0.1f;
        yield return new WaitForSeconds(waitTime);
        // Debug.Log(startTime + ", " + waitTime + ", " + Time.time);
    }

    public void FinishDialog()
    {
        // Debug.Log("finish dialog");
        partner.GetComponent<DialogManager>().QuitDialog();
        player.GetComponent<DialogManager>().QuitDialog();
        
        // database.ClearLines();
        GetComponentInParent<UIManager>().ExitDialog();
    }
    
    void PerformAction(Line line)
    {
        switch (line.lineAction)
        {
            case LineAction.Trade:
                {
                    // open trade window
                    uiManager.OpenTradingWindow();
                    break;
                }
            case LineAction.JoinGroup:
                {
                    player.GetComponent<GroupManager>().group.AddMember(partner);
                    partner.GetComponentInChildren<NPCAttributes>().groupStatus = GroupStatus.Follow;
                    break;
                }
            case LineAction.LeaveGroup:
                {
                    player.GetComponent<GroupManager>().group.DeleteMember(partner);
                    break;
                }
            /*case LineType.Add:
                {
                    // inventory.AddItem(((LineItem)line).itemName, ((LineItem)line).amount);
                    break;
                }
            case LineType.Delete:
                {
                    // inventory.RemoveItem(((LineItem)line).itemName, ((LineItem)line).amount);
                    break;
                }
            case LineType.QuestStatus:
                {
                    ((LineQuest)line).quest.status = ((LineQuest)line).status;
                    break;
                }
            case LineType.QuestContinue:
                {
                    // questlog.SetPartProgress(((LineQuest)line).quest, ((LineQuest)line).partId);
                    break;
                }
            case LineType.Arena:
                {
                    Group opposingGroup = partner.GetComponent<PlayerAttributes>().group;
                    Group playerGroup = this.transform.root.GetComponent<PlayerAttributes>().group;
                    LineArena lineArena = (LineArena)line;
                    lineArena.arena.GetComponent<MatchManager>().SetupMatch(new Group[] { playerGroup, opposingGroup }, new ItemStack[] { }, lineArena.quest, lineArena.partIndex);

                    FinishDialog();
                    break;
                }
            case LineType.Group:
                {
                    switch (((LineGroup)line).groupAction)
                    {
                        case LineGroup.GroupAction.Join:
                            {
                                this.transform.root.GetComponent<PlayerAttributes>().group.AddMember(partner);
                                break;
                            }
                        case LineGroup.GroupAction.Leave:
                            {
                                this.transform.root.GetComponent<PlayerAttributes>().group.DeleteMember(partner);
                                break;
                            }
                        case LineGroup.GroupAction.Follow:
                            {
                                partner.GetComponent<EnemyAI>().externalDestination = Vector3.zero;
                                break;
                            }
                        case LineGroup.GroupAction.Wait:
                            {
                                partner.GetComponent<EnemyAI>().externalDestination = partner.transform.position;
                                break;
                            }

                    }

                    FinishDialog();
                    break;
                }
            case LineType.Open:
                {
                    break;
                }
            case LineType.Spawn:
                {
                    break;
                }
            case LineType.Trade:
                {
                    break;
                }*/
            default:
                {
                    Debug.LogError("DialogAction not setup");
                    break;
                }
        }
    }
}
