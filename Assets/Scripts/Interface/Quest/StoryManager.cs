using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;

public class StoryManager : MonoBehaviour
{
    public static Type[] authorizedTrigger = new Type[] {null, typeof(ActorDialogEvent), typeof(ActorDeathEvent) };
    // public static string[] authorizedTriggerNames = new string[] {"None", "ActorDialogEvent", "ActorDeathEvent" };
    
    /*
    public delegate void ActorDialogEvent(GameObject adresser, GameObject receiver /*, Location location);
    public static event ActorDialogEvent OnActorDialog;

    public delegate void ActorDeathEvent(Actor dead, Actor murder /*, Location location);
    public static event ActorDeathEvent OnActorDead;
    */


        
    private QuestDatabaseList questDatabase;
    private PlayerDialog playerDialog;

    [SerializeField]
    private List<Quest> referencedQuests = new List<Quest>();
    [SerializeField]
    private List<Quest> availableQuests = new List<Quest>();
    [SerializeField]
    private List<Quest> activeQuests = new List<Quest>();
    [SerializeField]
    private List<Quest> finishedQuests = new List<Quest>();
    

    void Awake()
    {
        questDatabase = Resources.Load("Databases/QuestDatabase") as QuestDatabaseList;
        playerDialog = GameObject.FindWithTag("Interface").GetComponentInChildren<PlayerDialog>();
        // QuestTrigger.OnActorDialog += ActorDialogEvent;

        // test database for new quests 
        List<Quest> existingQuests = questDatabase.GetQuestListCopy();
        for (int i = 0; i < existingQuests.Count; i++)
        {
            // quest is not in lists?
            if (!referencedQuests.Contains(existingQuests[i]))
            {
                referencedQuests.Add(existingQuests[i]);
                availableQuests.Add(existingQuests[i]);
                // quest is start enabled?
                if (existingQuests[i].startGameEnabled)
                {
                    if (existingQuests[i].triggerType == EventTriggerType.None)
                    {
                        TriggerQuest(existingQuests[i], null);
                    }
                    else
                    {
                        Debug.LogWarning("Quest with TriggerEvent is startEnabled");
                    }
                }
            }
        }
    }

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        QuestAlias follower = availableQuests[0].questAliases[1];
        QuestAlias conditionAlias = availableQuests[0].dialogBranches[0].dialogLines[0].lineConditions[0].alias;

        Debug.Log(follower.aliasName + ", " + conditionAlias.aliasName + ", " + (follower == conditionAlias));
    }

    public void CallActorDialogEvent(GameObject addressor, GameObject receiver /*, Vector3 location*/)
    {
        Debug.Log("CallActorDialogEvent(" + addressor.name + "," + receiver.name + ")");

        // trigger quests
        for(int i = 0; i < availableQuests.Count;i++)
        {
            if(availableQuests[i].triggerType == EventTriggerType.ActorDialogEvent)
            {
                TriggerQuest(availableQuests[i], new ActorDialogEvent(addressor, receiver));
            }
        }

        // any actor is player?
        ControlMode actor1control = addressor.GetComponent<PlayerAttributes>().controlMode;
        ControlMode actor2control = receiver.GetComponent<PlayerAttributes>().controlMode;
        if (actor1control == ControlMode.Player || actor2control == ControlMode.Player)
        {
            List<Line> availableGreetings = new List<Line>();
            List<Line> availableLines = new List<Line>();

            // search active quests for lines to speak
            for(int i = 0; i < activeQuests.Count;i++)
            {
                // search for greeting
                for (int j = 0; j < activeQuests[i].misc.Count; j++)
                {
                    if(activeQuests[i].misc[j].lineType == LineType.Hello)
                    {
                        if(CheckConditions(activeQuests[i].misc[j].lineConditions))
                        {
                            availableGreetings.Add(activeQuests[i].misc[j]);
                        }
                    }
                }
                // search for lines
                for(int j = 0; j < activeQuests[i].dialogBranches.Count;j++)
                {
                    DialogBranch branch = activeQuests[i].dialogBranches[j];
                    if (branch.dialogLines.Count > 0)
                    {
                        List<Condition> conditions = branch.dialogLines[branch.rootIndex].lineConditions;

                        if (CheckConditions(conditions))
                        {
                            availableLines.Add(branch.dialogLines[branch.rootIndex]);
                        }
                    }
                }
            }

            // setup player dialog manager
            playerDialog.SetAvailableLines(availableLines, availableGreetings);
        }
        else
        {
            // enable SceenManager with new quest
        }
    }

    bool CheckConditions(List<Condition> conditions)
    {
        for (int k = 0; k < conditions.Count; k++)
        {
            if (!conditions[k].CheckCondition())
            {
                return false;
            }
        }

        return true;
    }

    void TriggerQuest(Quest quest, StoryManagerEvent eventData)
    {
        if (quest.StartQuest(eventData))
        {
            availableQuests.Remove(quest);
            activeQuests.Add(quest);
            Debug.Log("trigger " + quest.name + " successfull");
        }
        else
        {
            Debug.Log("trigger " + quest.name + " failed");
        }
    }
}

public enum EventTriggerType
{
    None,
    ActorDialogEvent,
    ActorDeathEvent
}