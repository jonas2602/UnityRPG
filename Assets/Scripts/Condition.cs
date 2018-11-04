using UnityEngine;
using System.Collections;

[System.Serializable]
public class Condition
{
    // static public string[] authorizedTargets = new string[] { "Player", "Self", "Target" };
    public static string[] authorizedComparisons = new string[] { "==", "!=", "<", ">", "<=", ">=" };
    public static System.Type[] authorizedFunctions = new System.Type[] { typeof(InGroup), typeof(IsFollowing), typeof(AttributeValue) , typeof(IsActor), typeof(HasFaction)};

    public ConditionFunction function;
    public string comparison;
    // public object reference;
    public int result;
    // public TargetType target;
    public QuestAlias alias;

    // delegate bool ConditionOperator<T>(T left, T right);
    // ConditionOperator<int> notEquals = (x, y) => x != y;
    // Func<int, int, bool> op = (x, y) => x != y;
    public Condition()
    {
        this.function = new InGroup();
        this.comparison = authorizedComparisons[0];
        result = 1;
        alias = null;
    }
    /*public Condition(ConditionFunction function, int result)
    {
        this.function = function;
        this.result = result;
    }*/

    public bool CheckCondition()
    {
        // get parameter from target
        int functionValue = function.GetValue(alias);

        // compare to allowed value;
        switch (comparison)
        {
            case "==": return functionValue == result;
            case "!=": return functionValue != result;
            case "<": return functionValue < result;
            case ">": return functionValue > result;
            case "<=": return functionValue <= result;
            case ">=": return functionValue >= result;
            default:
                {
                    Debug.LogError("unexpected comparison");
                    return false;
                }
        }
    }
}
/*
public enum TargetType
{
    Player,
    Target,
    Self
}*/

public abstract class ConditionFunction
{
    public abstract int GetValue(QuestAlias alias);
}

[System.Serializable]
public class InGroup : ConditionFunction
{
    public InGroup()
    {

    }

    public override int GetValue(QuestAlias alias)
    {
        // Group groupInfo = alias.aliasGameObject.GetComponent<GroupManager>().group;
        return 0; // groupInfo.groupMember[0] == alias.aliasGameObject ? 1 : 0;
    }
}

public class IsFollowing : ConditionFunction
{
    public IsFollowing()
    {

    }

    public override int GetValue(QuestAlias alias)
    {
        // NPCAttributes aiInfos = alias.aliasGameObject.GetComponentInChildren<NPCAttributes>();
        return 0; // aiInfos.groupStatus == GroupStatus.Follow ? 1 : 0;
    }
}


public class AttributeValue: ConditionFunction
{
    private ItemAttribute attribute;

    public AttributeValue(ItemAttribute attribute)
    {
        this.attribute = attribute;
    }

    public override int GetValue(QuestAlias alias)
    {
        // PlayerAttributes targetAttributes = alias.aliasGameObject.GetComponent<PlayerAttributes>();
        return 0; // attribute.attributeValue;
    }
}

public class IsActor : ConditionFunction
{
    private Actor actor;

    public IsActor(Actor actor)
    {
        this.actor = actor;
    }

    public override int GetValue(QuestAlias alias)
    {
        return 0; // alias.aliasActor == actor ? 1 : 0;
    }
}

public class HasFaction : ConditionFunction
{
    Faction requiredFaction;

    public HasFaction(Faction requiredFaction)
    {
        this.requiredFaction = requiredFaction;
    }

    public override int GetValue(QuestAlias alias)
    {
        return 1;
    }
}