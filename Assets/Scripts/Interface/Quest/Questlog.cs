using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Questlog : MonoBehaviour
{
    public QuestList questDatabase;
    public OldInventory inventory;
    public GUISkin skin;

    [SerializeField]
    private List<Quest> active = new List<Quest>();
    public List<Quest> finished = new List<Quest>();
    public List<Quest> failed = new List<Quest>();

    public bool showActive = true;
    public bool showFinished = true;
    public bool showFailed = true;

    public Quest descQuest = null;
    public float mouseWheel = 0;
    public float scrollSpeed = 1;


    public float posX = 100;
    public float posY = 50;
    public float windowWidth = 800;
    public float windowHeight = 440;

    public float barWidth = 200;

    public float questPosY;
    public float questPosX;
    public float questWidth;
    public float questHeight = 55;

    public float taskHeight = 40;
    public float doneWidth = 60;



    // Use this for initialization
    void Start()
    {
        questDatabase = GameObject.FindWithTag("QuestDatabase").GetComponent<QuestList>();
        inventory = GetComponent<OldInventory>();
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("v"))
        {
            RearrangeQuests();
        }
    }


    public void DrawQuestlog()
    {
        mouseWheel += Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;

        // Draw Window
        GUI.Box(new Rect(posX, posY, windowWidth, windowHeight), "", skin.GetStyle("Inventory"));

        //Draw Questbar
        Rect barRect = new Rect(posX, posY, barWidth, windowHeight);
        GUI.Box(barRect, "", skin.GetStyle("Inventory"));

        //Draw Quests
        int distanceCount = Mathf.RoundToInt(mouseWheel);
        float first = 10000;
        float last = 10000;

        // Draw Active
        DrawQuests(ref distanceCount, active, QuestStatus.Active, "Active", ref showActive, barRect, ref first, ref last);
        // Draw Finished
        DrawQuests(ref distanceCount, finished, QuestStatus.Finished, "Finished", ref showFinished, barRect, ref first, ref last);
        // Draw Failed
        DrawQuests(ref distanceCount, failed, QuestStatus.Failed, "Failed", ref showFailed, barRect, ref first, ref last);

        TestForOverscroll(first, last);

        // Draw Tasks
        Rect taskRect = new Rect(posX + barWidth, posY, (windowWidth - barWidth)/2, windowHeight);
        GUI.Box(taskRect, "", skin.GetStyle("Inventory"));
        // any quest selected
        if (descQuest != null)
        {
            for (int i = 0; i < descQuest.parts.Count; i++)
            {
                bool draw = true;
                // requires something
                if (descQuest.parts[i].partsRequired != null)
                {
                    // check if any required part is't finished
                    for (int j = 0; j < descQuest.parts[i].partsRequired.Length; j++)
                    {
                        // part is not finished
                        int partIndex = descQuest.parts[i].partsRequired[j];
                        if(!descQuest.parts[partIndex].finished)
                        {
                            draw = false;
                            break;
                        }
                    }
                }
                if (draw)
                {

                    Rect textRect = new Rect(taskRect.xMin, taskRect.yMin + i * taskHeight, taskRect.width - doneWidth, taskHeight);
                    GUI.Box(textRect, descQuest.parts[i].task, skin.GetStyle("Inventory"));

                    Rect doneRect = new Rect(textRect.xMax, textRect.yMin, doneWidth, taskHeight);
                    GUI.Box(doneRect,"" + descQuest.parts[i].finished, skin.GetStyle("Inventory"));
                }
            }
        }
        // Draw Story
        Rect storyRect = new Rect(taskRect.xMax, posY, (windowWidth - barWidth) / 2, windowHeight);
        GUI.Box(storyRect, CalculateStory(), skin.GetStyle("Inventory"));
    }


    public string CalculateStory()
    {
        string story = "";
        for(int i = 0; i < descQuest.parts.Count; i++)
        {
            if(descQuest.parts[i].finished)
            {
                story += descQuest.parts[i].story;
                story += "\n\n";
            }
        }
        return story;
    }


    public void RearrangeQuests()
    {
        for (int i = 0; i < questDatabase.quests.Count; i++)
        {
            Quest sortQuest = questDatabase.quests[i];
            SortQuest(sortQuest);
        }
    }


    void SortQuest(Quest sortQuest)
    {
        switch (sortQuest.status)
        {
            case QuestStatus.Active:
                {
                    active.Add(sortQuest);
                    break;
                }
            case QuestStatus.Finished:
                {
                    finished.Add(sortQuest);
                    break;
                }
            case QuestStatus.Failed:
                {
                    failed.Add(sortQuest);
                    break;
                }
        }
    }


    void DrawQuests(ref int distanceCount, List<Quest> list, QuestStatus status, string header, ref bool show, Rect barRect, ref float first, ref float last)
    {
        if (list.Count > 0)
        {
            Rect headerRect = new Rect(posX, posY + distanceCount * questHeight, barWidth, questHeight);
            distanceCount++;
            if (first == 10000)
            {
                first = headerRect.yMin;
            }
            last = headerRect.yMax;
            if (barRect.yMin <= headerRect.yMin && barRect.yMax >= headerRect.yMax)
            {
                GUI.Box(headerRect, header, skin.GetStyle("Inventory"));
                if (headerRect.Contains(Event.current.mousePosition))
                {
                    if (Event.current.type == EventType.MouseDown)
                    {
                        show = !show;
                    }
                }
            }
            if (show)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].status == status)
                    {
                        Rect questRect = new Rect(posX, posY + distanceCount * questHeight, barWidth, questHeight);
                        distanceCount++;
                        if (first == 10000)
                        {
                            first = questRect.yMin;
                        }
                        last = questRect.yMax;
                        if (barRect.yMin <= questRect.yMin && barRect.yMax >= questRect.yMax)
                        {
                            GUI.Box(questRect, "Name: " + list[i].name + "\n" + list[i].region + "  Level: " + list[i].level, skin.GetStyle("Inventory"));
                            if (questRect.Contains(Event.current.mousePosition))
                            {
                                if (Event.current.type == EventType.MouseDown)
                                {
                                    descQuest = list[i];
                                }
                            }
                        }
                    }
                    else
                    {
                        SortQuest(list[i]);
                        list.RemoveAt(i);
                    }
                }
            }
        }
    }


    void TestForOverscroll(float first, float last)
    {
        float disFirst = (first - posY) / 30;
        float disLast = (posY + windowHeight - last) / 30;
        if (disFirst > 0)
        {
            mouseWheel -= disFirst;
            disFirst = 0;
        }
        else if (disLast > 0 && disFirst < 0)
        {
            mouseWheel += disLast;
        }
    }


    public void SetPartProgress(Quest quest, int index)
    {
        // set part as finished
        quest.parts[index].finished = true;

        // check if whole quest is finished ...
        for (int i = 0; i < quest.parts.Count; i++)
        {
            if (!quest.parts[i].finished)
            {
                // ... not finished -> quit
                return;
            }
        }


        quest.status = QuestStatus.Finished;
        // ... is finished -> get rewards
        for (int i = 0; i < quest.rewards.Length; i++)
        {
            // inventory.AddItem(quest.rewards[i].slotItem.itemName, quest.rewards[i].slotAmount);
        }
    }
}
