using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Quest
{
    public string name;
    public int id;
    public int level;
    public EventTriggerType triggerType; // index of authorizedTrigger in StoryManager 
    public bool startGameEnabled;
    public bool repeat;
    public int priority;
    public List<Part> parts;
    public Region region;
    public QuestStatus status;
    public Quest required;
    public Item[] rewards;
    public List<DialogBranch> dialogBranches;
    public List<Line> misc;
    public List<QuestAlias> questAliases;
    public List<QuestStage> questStages;
    public QuestAlias testAlias;

    
    public enum Region
    {
        RegionA,
        RegionB,
        RegionC
    }

    public Quest()
    {
        name = "New Quest";
        startGameEnabled = true;
        repeat = false;
        dialogBranches = new List<DialogBranch>();
        questAliases = new List<QuestAlias>();
        misc = new List<Line>();
        testAlias = new QuestAlias();
    }

    public Quest(string name, int id, int lvl, QuestStatus status)
    {
        this.name = name;
        this.id = id;
        level = lvl;
        this.status = status;
    }

    public Quest(string name, int id, int level, Region region, List<Part> parts, Item[] rewards)
    {
        this.name = name;
        this.id = id;
        this.level = level;
        this.region = region;
        this.parts = parts;
        this.rewards = rewards;
    }

    public Quest(string name, int id, int level, Region region, Part[] parts, Item[] rewards)
    {
        
    }

    public bool StartQuest(StoryManagerEvent eventData)
    {
        // setup aliases
        for(int i = 0; i < questAliases.Count;i++)
        {
            if(!questAliases[i].FillAlias(eventData))
            {
                return false;
            }
        }

        // set quest to startstage


        status = QuestStatus.Active;
        return true;
    }

    public Quest GetCopy()
    {
        return this.MemberwiseClone() as Quest;
    }
}
public enum QuestStatus
{
    Available,
    Active,
    Finished,
    NotAvailable,
    Failed
}


[System.Serializable]
public class Part
{
    public bool finished;
    public string task;
    public TaskType taskType;
    public GameObject taskObject;
    public string story;
    public Marker marker;
    public int[] partsRequired;

    public enum TaskType
    {
        Kill,
        Defeat,
        Find
    }

    public Part(string task/*, TaskType taskType, GameObject taskObject*/, string story)
    {
        finished = false;
        this.task = task;
        /*this.taskType = taskType;
        this.taskObject = taskObject;*/
        this.story = story;
    }

    public Part(string task/*, TaskType taskType, GameObject taskObject*/, string story, int[] partsRequired)
    {
        finished = false;
        this.task = task;
        /*this.taskType = taskType;
        this.taskObject = taskObject;*/
        this.story = story;
        this.partsRequired = partsRequired;
    }
}

[System.Serializable]
public class QuestStage
{
    public int stageIndex;
    public string stageInfo;
}

[System.Serializable]
public class ReferenceClass
{
    public string name;
}

[System.Serializable]
public class QuestAlias
{
    // Alias info
    public string aliasName;
    // public GameObject aliasGameObject;
    // public Actor aliasActor;
    public AliasType aliasType;

    // fillInfo
    public FillType fillType;
    public bool optional;
    public int eventDataIndex;
    // public QuestAlias nearAlias;
    public List<Condition> conditions;

    public QuestAlias()
    {
        aliasName = "New QuestAlias";
        // aliasGameObject = null;
        optional = false;
        fillType = FillType.UniqueActor;
        aliasType = AliasType.Actor;
        conditions = new List<Condition>();
    }

    public virtual bool FillAlias(StoryManagerEvent eventData)
    {
        switch (aliasType)
        {
            case AliasType.Actor:
                {
                    switch (fillType)
                    {
                        case FillType.FindMatchingReferenceFromEvent:
                            {
                                // get avatar from triggerEvent
                                GameObject avatar = eventData.GetEventData()[eventDataIndex];

                                // check conditions
                                for (int i = 0; i < conditions.Count;i++)
                                {
                                    if (!conditions[i].CheckCondition())
                                    {
                                        return false;
                                    }
                                }

                                // setup alias
                                Debug.Log("Setup " + aliasName + " with " + avatar);
                                // aliasGameObject = avatar;
                                // aliasActor = avatar.GetComponent<PlayerAttributes>().parentActor;
                                break;
                            }
                    }
                    break;
                }
        }

        return true;
    }
}

/*public class ActorAlias: QuestAlias
{

}

public class LocaltionAlias : QuestAlias
{
    
}*/

public enum AliasType
{
    Actor,
    Location,
    Object
}

public enum FillType
{
    UniqueActor,
    FindMatchingReferenceFromEvent,
    FindMatchingReferenceNearAlias
}

[System.Serializable]
public class DialogBranch
{
    public string branchName;
    public int rootIndex;
    public List<Line> dialogLines;
    public BranchType branchType;
    public bool overrideGreeting;

    public DialogBranch()
    {
        branchName = "New Branch";
        dialogLines = new List<Line>();
        overrideGreeting = false;
    }
}

public enum BranchType
{
    Normal,
    Blocking // hides all other branches until its done
}
 