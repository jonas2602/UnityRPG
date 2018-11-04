using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Dialog : MonoBehaviour
{

    public GameObject partner;
    public GUISkin skin;
    private DialogList database;
    private OldInventory inventory;
    private Questlog questlog;
    public Interface Interface;


    //text
    public Line question;
    public List<Line> answers = new List<Line>();


    //position
    public float posX;
    public float posY;
    public float windowHeight = 207f;
    public float windowWidth = 480f;
    public float posXQ = 6.5f;
    public float posYQ = 6.5f;
    public float windowHeightQ = 50f;
    public float windowWidthQ = 467f;
    public float posXA = 12f;
    public float posYA = 3f;
    public float windowHeightA = 47f;
    public float windowWidthA = 456f;



    // Use this for initialization
    void Start()
    {
        posX = (Screen.width - windowWidth) / 2;
        posY = Screen.height * 0.97f - windowHeight;

        database = GameObject.FindWithTag("Database").GetComponent<DialogList>();
        inventory = GetComponent<OldInventory>();
        questlog = GetComponent<Questlog>();
        Interface = GetComponent<Interface>();
    }


    // Update is called once per frame
    void Update()
    {

    }


    void Input(string id)
    {
        // get Question
        question = GetLine(id);

        // add answers from question
        answers = new List<Line>();
        for (int i = 0; i < question.lineOptions.Length; i++)
        {
            answers.Add(GetLine(question.lineOptions[i]));
        }
    }


    Line GetLine(string id)
    {
        Line line = null;
        /*for (int i = 0; i < database.lines.Count; i++)
        {
            if (database.lines[i].lineID == id)
            {
                line = database.lines[i];
                break;
            }
        }*/
        
        return line;
    }


    public void DrawDialog()
    {
        // create DialogWindow
        GUI.Box(new Rect(posX, posY, windowWidth, windowHeight), "", skin.GetStyle("Inventory"));

        // create NPC-Text
        if (question != null)
        {
            GUI.Box(new Rect(posX + posXQ, posY + posYQ, windowWidthQ, windowHeightQ), question.lineTeaser, skin.GetStyle("Tooltip"));
        }
        //create Answers
        for (int i = 0; i < answers.Count; i++)
        {
            Rect AnsRect = new Rect(posX + posXA, posY + posYQ + windowHeightQ + posYA + i * windowHeightA, windowWidthA, windowHeightA);
            GUI.Box(AnsRect, answers[i].lineTeaser, skin.GetStyle("Tooltip"));

            // mouse over answer?
            if (AnsRect.Contains(Event.current.mousePosition))
            {
                // mouse down?
                if (Event.current.button == 0 && Event.current.type == EventType.MouseDown)
                {
                    // activate action
                    if (answers[i].lineAction != LineAction.Nothing)
                    {
                        PerformAction(answers[i]);
                    }
                    /*
                    // continue talk
                    if (answers[i].lineNextQuestion != null)
                    {
                        Input(answers[i].lineNextQuestion);
                    }*/
                }
            }
        }
    }


    public void StartDialog(GameObject npc)
    {
        partner = npc;
        Interface.showDialog = true;
        database.AddLines(partner);
        Input("Q");

    }


    public void FinishDialog()
    {
        Interface.showDialog = false;
        partner.GetComponent<DialogManager>().dialogPartner = null;
        partner = null;
        database.ClearLines();
    }


    void PerformAction(Line line)
    {/*
        switch (line.lineAction)
        {
            case LineType.Add:
                {
                    inventory.AddItem(((LineItem)line).itemName, ((LineItem)line).amount);
                    break;
                }
            case LineType.Delete:
                {
                    inventory.RemoveItem(((LineItem)line).itemName, ((LineItem)line).amount);
                    break;
                }
            case LineType.QuestStatus:
                {
                    ((LineQuest)line).quest.status = ((LineQuest)line).status;
                    break;
                }
            case LineType.QuestContinue:
                {
                    questlog.SetPartProgress(((LineQuest)line).quest, ((LineQuest)line).partId);
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
                    switch(((LineGroup)line).groupAction)
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
        }*/
    }
}

