using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class QuestDatabaseList : ScriptableObject
{
    [SerializeField]
    private List<Quest> questList = new List<Quest>();

    public Quest CreateNewQuest()
    {
        Quest newQuest = new Quest();
        questList.Add(newQuest);

        return newQuest;
    }

    public List<Quest> GetQuestList()
    {
        return questList;
    }

    public List<Quest> GetQuestListCopy()
    {
        List<Quest> listCopy = new List<Quest>();

        for(int i = 0; i < questList.Count;i++)
        {
            QuestAlias follower = questList[i].questAliases[1];
            QuestAlias conditionAlias = questList[i].dialogBranches[0].dialogLines[0].lineConditions[0].alias;

            Debug.Log(follower.aliasName + ", " + conditionAlias.aliasName + ", " + (follower == conditionAlias));
            listCopy.Add(questList[i].GetCopy());
        }

        return listCopy;
    }

    public void DeleteQuest(Quest quest)
    {
        questList.Remove(quest);
    }
}
