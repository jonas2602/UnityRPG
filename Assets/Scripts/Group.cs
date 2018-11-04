using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Group
{
    public List<GameObject> groupMember;
    public int maxMember;
    public int deathCount;

    public Group()
    {
        groupMember = new List<GameObject>();
    }

    public Group(GameObject[] member)
    {
        maxMember = 4;
        groupMember = new List<GameObject>();
        for(int i = 0; i < member.Length;i++)
        {
            AddMember(member[i]);
        }
    }


    public void AddMember(GameObject newMember)
    {
        if (groupMember.Count < maxMember)
        {
            groupMember.Add(newMember);
        }
        newMember.GetComponent<GroupManager>().group = groupMember[0].GetComponent<GroupManager>().group;
    }


    public void DeleteMember(GameObject member)
    {
        groupMember.Remove(member);
        member.GetComponent<GroupManager>().group = new Group(new GameObject[] { member });
    }
    


    public void DissolveGroup()
    {
        // delete group
    }


    public bool GroupContains()
    {
        return true;
    }


    public Health.Condition[] GetConditions
    {
        get
        {
            Health.Condition[] conditions = new Health.Condition[groupMember.Count];
            for (int i = 0; i < groupMember.Count; i++)
            {
                conditions[i] = groupMember[i].GetComponent<Health>().condition;
            }

            return conditions;
        }
    }
}
