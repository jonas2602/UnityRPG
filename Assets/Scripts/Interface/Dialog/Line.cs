using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Line // dialogPart
{
    public string lineId;
    public List<LinePart> lineTexts;
    public string lineTeaser;
    public bool lineFinishDialog;
    public bool skipGreeting;
    public Sprite lineIcon;
    // public string lineNextQuestion;
    public string[] lineOptions;
    public List<Line> lineBranches;
    public LineType lineType;
    public LineAction lineAction;         // trigger event, add event listener
    public List<Condition> lineConditions;           // must be true before line may be shown 
    // public TreeChange[] treeChanges;
    public LineVariableStorage storage;

    // default
    public Line()
    {
        lineId = "S";
        lineTeaser = "Line Preview";
        lineTexts = new List<LinePart>();
        lineOptions = new string[0];
        lineBranches = new List<Line>();
        lineConditions = new List<Condition>();
    }

    // start + finish
    public Line(List<LinePart> texts)
    {
        lineTexts = texts;
    }

    // start + continue
    public Line(List<LinePart> texts, string[] options)
    {
        lineTexts = texts;
        lineOptions = options;
    }

    // talk + finish
    public Line(string teaser, List<LinePart> texts)
    {
        lineTeaser = teaser;
        lineTexts = texts;
    }

    // talk + action + finish
    public Line(string teaser, List<LinePart> texts, LineAction action)
    {
        lineTeaser = teaser;
        lineTexts = texts;
        lineAction = action;
    }

    /*
    public Line(string teaser, LinePart[] texts, LineType action, TreeChange[] treeChanges)
    {
        lineTeaser = teaser;
        lineTexts = texts;
        lineAction = action;
        this.treeChanges = treeChanges;
    }*/

    // talk
    public Line(string teaser, List<LinePart> texts, string[] options)
    {
        lineTeaser = teaser;
        lineTexts = texts;
        lineOptions = options;
    }
    
    // talk + action
    public Line(string teaser, List<LinePart> texts, LineAction action, string[] options)
    {
        lineTeaser = teaser;
        lineTexts = texts;
        lineAction = action;
        lineOptions = options;
    }

    /*
    // answer + continue Talk
    public Line(string id, string text, string nextQuestion)
    {
        lineID = id;
        lineTeaser = text;
        lineNextQuestion = nextQuestion;
    }

    // answer + action
    public Line(string id, string text, LineType action)
    {
        lineID = id;
        lineTeaser = text;
        lineAction = action;
    }

    // answer + action + continue Talk
    public Line(string id, string text, string nextQuestion, LineType action)
    {
        lineID = id;
        lineTeaser = text;
        lineNextQuestion = nextQuestion;
        lineAction = action;
    }

    // question + continue Talk
    public Line(string id, string text, string[] answers)
    {
        lineID = id;
        lineTeaser = text;
        lineAnswers = answers;
    }

    // question + action
    public Line(string id, string text, string[] answers, LineType action)
    {
        lineID = id;
        lineTeaser = text;
        lineAnswers = answers;
        lineAction = action;
    }*/
}

public enum LineType
{
    Default,
    Hello,
    Goodbye
}

public enum LineAction
{
    Nothing,
    Add,
    Open,
    Delete,
    QuestStatus,
    QuestContinue,
    SetMarker,
    Send,
    JoinGroup,
    LeaveGroup,
    WaitGroup,
    FollowGroup,
    Arena,
    Rage,
    Spawn,
    Trade
    // replace line
}

[System.Serializable]
public class LineVariableStorage
{
    public object[] variables;

    public LineVariableStorage(object[] variables)
    {
        this.variables = variables;
    }
}


[System.Serializable]
public class LinePart
{
    public QuestAlias speaker;     // who speaks?
    public QuestAlias testAlias;
    public string text;     // what did he say
    public AudioClip audio;

    public LinePart(QuestAlias speaker, string text)
    {
        this.speaker = speaker;
        this.text = text;
    }
}
/*
public abstract class TreeChange
{
    public abstract void ChangeTree(DialogStatusManager tree);

    enum ChangeType
    {
        Insert,
        CutRelation,
        Remove,
        Replace
    }
}

public class InsertLine : TreeChange
{
    private string lineId;
    private string parentId;
    private string[] childrenIds;

    public InsertLine(string lineId, string parentId)
    {
        this.lineId = lineId;
        this.parentId = parentId;
        this.childrenIds = new string[] { };
    }

    public InsertLine(string lineId, string parentId, string[] childrenIds)
    {
        this.lineId = lineId;
        this.parentId = parentId;
        this.childrenIds = childrenIds;
    }

    public override void ChangeTree(DialogStatusManager tree)
    {
        DialogTree node = tree.CreateNode(lineId, childrenIds);

        tree.InsertNode(parentId, node);
    }
}

public class ReplaceLine : TreeChange
{
    private string newLineId;
    private string oldLineId;

    public ReplaceLine(string newLineId, string oldLineId)
    {
        this.newLineId = newLineId;
        this.oldLineId = oldLineId;
    }

    public override void ChangeTree(DialogStatusManager tree)
    {
        // find parents

        // replace reference
        
    }
}

public class RemoveLine : TreeChange
{
    private string lineId;

    public RemoveLine(string lineId)
    {
        this.lineId = lineId;
    }

    public override void ChangeTree(DialogStatusManager tree)
    {
        tree.RemoveNode(lineId);
    }
}
*/
/*
[System.Serializable]
public class LineItem : Line
{/*
    public string itemName;
    public int amount;

    public LineItem(string id, string text, LineType action, string itemName, int amount)
        : base(id, text, action)
    {
        this.itemName = itemName;
        this.amount = amount;
    }

    public LineItem(string id, string text, string nextQuestion, LineType action, string itemName, int amount)
        : base(id, text, nextQuestion, action)
    {
        this.itemName = itemName;
        this.amount = amount;
    }
}

[System.Serializable]
public class LineSpawn : Line
{/*
    public NPC npc;
    public Vector3 position;

    public LineSpawn(string id, string text, LineType action, NPC npc, Vector3 position)
        : base(id, text, action)
    {
        this.npc = npc;
        this.position = position;
    }

    public LineSpawn(string id, string text, string nextQuestion, LineType action, NPC npc, Vector3 position)
        : base(id, text, nextQuestion, action)
    {
        this.npc = npc;
        this.position = position;
    }
}

[System.Serializable]
public class LineQuest : Line
{/*
    public Quest quest;
    public int partId;
    public Quest.Status status;


    // Quest Continue
    public LineQuest(string id, string text, LineType action, Quest quest, int partId)
        : base(id, text, action)
    {
        this.quest = quest;
        this.partId = partId;
    }
    public LineQuest(string id, string text, string nextQuestion, LineType action, Quest quest, int partId)
        : base(id, text, nextQuestion, action)
    {
        this.quest = quest;
        this.partId = partId;
    }

    // Quest Status
    public LineQuest(string id, string text, LineType action, Quest quest, Quest.Status status)
        : base(id, text, action)
    {
        this.quest = quest;
        this.status = status;
    }
    public LineQuest(string id, string text, string nextQuestion, LineType action, Quest quest, Quest.Status status)
        : base(id, text, nextQuestion, action)
    {
        this.quest = quest;
        this.status = status;
    }
}

[System.Serializable]
public class LineArena : Line
{/*
    public GameObject arena;
    public ItemStack[] rewards;
    public Quest quest;
    public int partIndex;


    // Quest Continue
    public LineArena(string id, string text, LineType action, GameObject arena, Quest quest, int partIndex)
        : base(id, text, action)
    {
        this.arena = arena;
        this.quest = quest;
        this.partIndex = partIndex;
    }
    public LineArena(string id, string text, string nextQuestion, LineType action, GameObject arena, Quest quest, int partIndex)
        : base(id, text, nextQuestion, action)
    {
        this.arena = arena;
        this.quest = quest;
        this.partIndex = partIndex;
    }
}

[System.Serializable]
public class LineGroup : Line
{/*
    public enum GroupAction
    {
        Join,
        Leave,
        Wait,
        Follow
    }

    public GroupAction groupAction;


    // Quest Continue
    public LineGroup(string id, string text, LineType action, GroupAction groupAction)
        : base(id, text, action)
    {
        this.groupAction = groupAction;
    }
    public LineGroup(string id, string text, string nextQuestion, LineType action, GroupAction groupAction)
        : base(id, text, nextQuestion, action)
    {
        this.groupAction = groupAction;
    }
}*/