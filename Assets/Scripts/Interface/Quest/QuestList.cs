using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QuestList : MonoBehaviour
{
    public List<Quest> quests = new List<Quest>();

    void Start()
    {
        ItemList itemDatabase = GameObject.FindWithTag("Database").GetComponent<ItemList>();
        
        // Add Quests
        quests.Add(new Quest("Arena Champion", 0, 20, Quest.Region.RegionA, new Part[] { new Part("defeat enemy 1"/*, Part.TaskType.Defeat, GameObject.FindWithTag("Gladiator")*/, "you had defeated enemy 1"), new Part("defeat enemy 2",/* Part.TaskType.Defeat, GameObject.FindWithTag("Gladiator"),*/ "you had defeated enemy 2"), new Part("defeat enemy 3"/*, Part.TaskType.Defeat, GameObject.FindWithTag("Gladiator")*/, "you had defeated enemy 3"), new Part("go back to the arena manager and get your rewards", "you had defeated all enemys and are the champion of the arena", new int[] { 0, 1, 2 }) }, new Item[] { itemDatabase.items[2], itemDatabase.items[6] }));
        /*quests.Add(new Quest("2.Quest", 1, 9000, Quest.Status.Failed));
        quests.Add(new Quest("3.Quest", 2, 9000, Quest.Status.Active));
        quests.Add(new Quest("4.Quest", 3, 9000, Quest.Status.Finished));
        quests.Add(new Quest("5.Quest", 4, 20, Quest.Status.Finished));
        quests.Add(new Quest("6.Quest", 5, 9000, Quest.Status.NotAvailable));
        quests.Add(new Quest("7.Quest", 6, 20, Quest.Status.Finished));
        quests.Add(new Quest("8.Quest", 7, 20, Quest.Status.Failed));
        quests.Add(new Quest("9.Quest", 8, 20, Quest.Status.Active));
        quests.Add(new Quest("10.Quest", 9, 20, Quest.Status.Finished));
        quests.Add(new Quest("1.Quest", 10, 9000, Quest.Status.Active));*/
    }
}
