using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class QuestEditor : EditorWindow
{
    static private QuestDatabaseList questDatabase;
    static private ActorDatabaseList actorDatabase;

    // toolBar
    private int toolbarIndex = 0;
    private string[] toolbarStrings = new string[] { "QuestData", "QuestStages","QuestAliases", "PlayerDialog", "Misc" };

    // active Quest
    private Quest activeQuest;
    private bool[] showBranch = new bool[100];
    private List<LineWindow> lineWindows = new List<LineWindow>();

    // active alias
    private QuestAlias activeAlias;
    private int selectedActorIndex = 0;

    // active line
    private Line activeLine;
    private List<int> speakerIndices;

    // branches
    private bool showBranchEditor = true;
    private bool showLineEditor = true;

    // references
    private int referenceStartIndex = -1;
    private Line startBranchLine;

    [MenuItem("Dialog/QuestEditor")]
    static void Init()
    {
        questDatabase = Resources.Load("Databases/QuestDatabase") as QuestDatabaseList;
        if (questDatabase == null)
        {
            questDatabase = DatabaseFactory.CreateQuestDatabase();
        }

        actorDatabase = Resources.Load("Databases/ActorDatabase") as ActorDatabaseList;
        if (actorDatabase == null)
        {
            actorDatabase = DatabaseFactory.CreateActorDatabase();
        }

        QuestEditor window = GetWindow<QuestEditor>();
        window.Show();
    }

    void OnGUI()
    {
        if(!questDatabase || !actorDatabase)
        {
            Init();
        }

        if (activeQuest == null)
        {
            // Chose Quest
            List<Quest> existingQuests = questDatabase.GetQuestList();

            for(int i = 0; i < existingQuests.Count;i++)
            {
                EditorGUILayout.BeginHorizontal();

                GUILayout.Label(existingQuests[i].name);

                if (GUILayout.Button("Edit"))
                {
                    LoadQuest(existingQuests[i]);
                }
                
                if (GUILayout.Button("Delete"))
                {
                    questDatabase.DeleteQuest(existingQuests[i]);
                }

                EditorGUILayout.EndHorizontal();
            }

            // create new line
            if (GUILayout.Button("Add Quest"))
            {
                LoadQuest(questDatabase.CreateNewQuest());
            }
        }
        else
        {
            if (activeQuest.dialogBranches.Count > 0)
            {
                if (activeQuest.dialogBranches[0].dialogLines.Count > 0)
                {
                    Line refLine = activeQuest.dialogBranches[0].dialogLines[0];
                    if (refLine.lineConditions.Count > 0)
                    {
                        QuestAlias follower = activeQuest.questAliases[1];
                        QuestAlias conditionAlias = refLine.lineConditions[0].alias;
                        Debug.Log(follower.aliasName + ", " + conditionAlias.aliasName + ", " + (follower == conditionAlias));
                    }
                    if (refLine.lineTexts.Count > 0)
                    {
                        if (refLine.lineTexts[0].testAlias != null)
                        {
                            Debug.Log("Name: " + refLine.lineTexts[0].testAlias.aliasName);
                        }
                    }
                }
            }
            // Edit Quest
            toolbarIndex = GUILayout.Toolbar(toolbarIndex, toolbarStrings);

            switch (toolbarIndex)
            {
                case 0:
                    {
                        EditQuestData();
                        break;
                    }
                case 1:
                    {
                        EditQuestStages();
                        break;
                    }
                case 2:
                    {
                        EditQuestAliases();
                        break;
                    }
                case 3:
                    {
                        EditQuestBranches();
                        break;
                    }
                case 4:
                    {
                        EditQuestMisc();
                        break;
                    }
            }

            if (GUILayout.Button("Save"))
            {
                if(activeLine != null)
                {
                    SaveLine();
                }

                SaveQuest();
            }

            if (GUILayout.Button("Cancel"))
            {
                activeLine = null;
                activeQuest = null;
            }
        }
    }

    void EditQuestData()
    {
        activeQuest.id = EditorGUILayout.IntField("QuestId:", activeQuest.id);

        activeQuest.name = EditorGUILayout.TextField("QuestName:", activeQuest.name);

        activeQuest.triggerType = (EventTriggerType)EditorGUILayout.EnumPopup("Event:", activeQuest.triggerType);
        // activeQuest.triggerTypeIndex = StoryManager.authorizedTrigger[triggerIndex];
        activeQuest.startGameEnabled = EditorGUILayout.Toggle("Start Game Enabled", activeQuest.startGameEnabled);

        activeQuest.repeat = EditorGUILayout.Toggle("Repeat Quest", activeQuest.repeat);

        activeQuest.priority = EditorGUILayout.IntSlider("Priority", activeQuest.priority, 0, 100);
    }

    void EditQuestStages()
    {

    }

    void EditQuestAliases()
    {
        EditorGUILayout.BeginHorizontal();

        // alias overview
        EditorGUILayout.BeginVertical();

        // show existing aliases
        for (int i = 0; i < activeQuest.questAliases.Count;i++)
        {
            EditorGUILayout.BeginHorizontal();

            // name
            EditorGUILayout.LabelField("Name: " + activeQuest.questAliases[i].aliasName);

            // optional
            EditorGUILayout.LabelField("Optional: " + activeQuest.questAliases[i].optional);

            // aliasType
            EditorGUILayout.LabelField("AliasType: " + activeQuest.questAliases[i].aliasType);

            // fillType
            EditorGUILayout.LabelField("FillType: " + activeQuest.questAliases[i].fillType);

            if (GUILayout.Button("Edit"))
            {
                activeAlias = activeQuest.questAliases[i];
            }

            if (GUILayout.Button("Delete"))
            {
                activeQuest.questAliases.RemoveAt(i);
            }

            EditorGUILayout.EndHorizontal();
        }

        // add new Alias
        if (GUILayout.Button("Add Alias"))
        {
            activeQuest.questAliases.Add(new QuestAlias());
        }
        EditorGUILayout.EndVertical();

        // alias editor
        if (activeAlias != null)
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(300f));
            // name
            activeAlias.aliasName = EditorGUILayout.TextField("Name:", activeAlias.aliasName);

            // optional
            activeAlias.optional = EditorGUILayout.Toggle("Optional:", activeAlias.optional);

            // aliasType
            activeAlias.aliasType = (AliasType)EditorGUILayout.EnumPopup("AliasType:", activeAlias.aliasType);

            // fillType
            activeAlias.fillType = (FillType)EditorGUILayout.EnumPopup("FillType:", activeAlias.fillType);

            switch(activeAlias.fillType)
            {
                case FillType.UniqueActor:
                    {
                        // show popup with all existing actors
                        if (!actorDatabase)
                        {
                            Init();
                        }

                        List<Actor> actorList = actorDatabase.GetActorList();
                        if (actorList.Count > 0)
                        {
                            string[] names = new string[actorList.Count];
                            for (int i = 0; i < actorList.Count; i++)
                            {
                                names[i] = actorList[i].name;
                            }
                            
                            selectedActorIndex = EditorGUILayout.Popup("Actor:", selectedActorIndex, names);
                            // activeAlias.aliasActor = actorList[selectedActorIndex];
                        }
                        else
                        {
                            EditorGUILayout.LabelField("No Actors available to select");
                        }

                        break;
                    }
                case FillType.FindMatchingReferenceFromEvent:
                    {
                        // activeAlias.eventDataIndex = EditorGUILayout.Popup("EventData:", activeAlias.eventDataIndex, );
                        if (activeQuest.triggerType > 0)
                        {
                            ParameterInfo[] parameter = StoryManager.authorizedTrigger[(int)activeQuest.triggerType].GetConstructors()[0].GetParameters();
                            string[] parameterNames = new string[parameter.Length];
                            for (int i = 0; i < parameterNames.Length; i++)
                            {
                                parameterNames[i] = parameter[i].Name;
                            }
                            activeAlias.eventDataIndex = EditorGUILayout.Popup("Actor:", activeAlias.eventDataIndex, parameterNames);
                        }
                        break;
                    }
                case FillType.FindMatchingReferenceNearAlias:
                    {
                        break;
                    }
                default:
                    {
                        Debug.LogError("Unexpected FillType");
                        break;
                    }
            }

            // show conditions
            ConditionEditor.EditorBuildIn(activeAlias.conditions, activeQuest.questAliases);

            if (GUILayout.Button("Cancel"))
            {
                activeAlias = null;
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndHorizontal();
    }

    void EditQuestBranches()
    {
        EditorGUILayout.BeginHorizontal(GUILayout.Height(this.position.height * 0.8f));

        // brancheditor
        if (showBranchEditor)
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(300f));
            EditBranches();
            EditorGUILayout.EndVertical();

            // flip arrow
            if (GUILayout.Button("<", GUILayout.ExpandWidth(false)))
            {
                showBranchEditor = !showBranchEditor;
            }
        }
        else
        {
            // open arrow
            if (GUILayout.Button(">", GUILayout.ExpandWidth(false)))
            {
                showBranchEditor = !showBranchEditor;
            }
        }
        
        // linetree editor
        EditorGUILayout.BeginVertical();
        // show lines
        BeginWindows();
        for (int i = 0; i < lineWindows.Count; i++)
        {
            lineWindows[i].lineRect = GUILayout.Window(i, lineWindows[i].lineRect, CreateLine, lineWindows[i].storedLine.lineTeaser);
        }

        // draw existing references
        for (int i = 0; i < lineWindows.Count; i++)
        {
            for (int j = 0; j < lineWindows[i].referencedWindows.Count; j++)
            {
                if(lineWindows[i].referencedWindows[j].storedLine != null)
                {
                    Drawing.CurveFromTo(lineWindows[i].lineRect, lineWindows[i].referencedWindows[j].lineRect);
                }
                else
                {
                    lineWindows[i].referencedWindows.RemoveAt(j);
                }
            }
        }

        // draw new reference
        if (referenceStartIndex != -1)
        {
            Drawing.CurveFromTo(lineWindows[referenceStartIndex].lineRect, new Rect(Event.current.mousePosition, Vector2.zero));

            // right click
            if (Event.current.button == 1 && Event.current.type == EventType.MouseDown)
            {
                // stop referencing
                referenceStartIndex = -1;
            }
        }
        EndWindows();
        EditorGUILayout.EndVertical();

        if (activeLine != null)
        {   
            // lineEditor
            if (showLineEditor)
            {
                // flip arrow
                if (GUILayout.Button(">", GUILayout.ExpandWidth(false)))
                {
                    showLineEditor = !showLineEditor;
                }

                EditorGUILayout.BeginVertical(GUILayout.Width(300f));
                EditLine();
                EditorGUILayout.EndVertical();
            }
            else
            {
                // open arrow
                if (GUILayout.Button("<", GUILayout.ExpandWidth(false)))
                {
                    showLineEditor = !showLineEditor;
                }
            }
        }
        EditorGUILayout.EndHorizontal();
        // GUI.Box(new Rect(100, 100, 200, 200),"");
    }

    void EditQuestMisc()
    {
        EditorGUILayout.BeginHorizontal();
        // show lines
        EditorGUILayout.BeginVertical();
        for(int i = 0; i < activeQuest.misc.Count;i++)
        {
            EditorGUILayout.LabelField("Id: " + activeQuest.misc[i].lineId);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Edit"))
            {
                activeLine = activeQuest.misc[i];
            }
            if (GUILayout.Button("Delete"))
            {
                activeQuest.misc.RemoveAt(i);
            }
            EditorGUILayout.EndHorizontal();
        }
        
        if (GUILayout.Button("Add"))
        {
            activeQuest.misc.Add(new Line());
        }
        EditorGUILayout.EndVertical();

        // edit active line
        EditorGUILayout.BeginVertical();
        if (activeLine != null)
        {
            EditLine();
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
    }

    void EditBranches()
    {
        // show branches
        for (int i = 0; i < activeQuest.dialogBranches.Count; i++)
        {
            // draw 
            showBranch[i] = EditorGUILayout.Foldout(showBranch[i], activeQuest.dialogBranches[i].branchName);
            if (showBranch[i])
            {
                activeQuest.dialogBranches[i].branchName = EditorGUILayout.TextField("BranchName:", activeQuest.dialogBranches[i].branchName);
                activeQuest.dialogBranches[i].branchType = (BranchType)EditorGUILayout.EnumPopup("Type", activeQuest.dialogBranches[i].branchType);

                string[] lineIds = new string[activeQuest.dialogBranches[i].dialogLines.Count];
                for (int j = 0; j < activeQuest.dialogBranches[i].dialogLines.Count; j++)
                {
                    lineIds[j] = activeQuest.dialogBranches[i].dialogLines[j].lineId;
                }

                activeQuest.dialogBranches[i].rootIndex = EditorGUILayout.Popup("RootLine", activeQuest.dialogBranches[i].rootIndex, lineIds);

                if (GUILayout.Button("Add Line"))
                {
                    activeQuest.dialogBranches[i].dialogLines.Add(new Line());
                    lineWindows.Add(new LineWindow(activeQuest.dialogBranches[i]));
                }

                if (GUILayout.Button("Delete Branch"))
                {
                    // remove lines

                    // remove branch
                    activeQuest.dialogBranches.RemoveAt(i);
                }
            }
        }

        if (GUILayout.Button("Add Branch"))
        {
            activeQuest.dialogBranches.Add(new DialogBranch());
        }
    }

    void CreateLine(int id)
    {
        LineWindow lineWindow = lineWindows[id];

        // show lineInfo

        // show edit button
        if (GUILayout.Button("Edit Line"))
        {
            if(activeLine != null)
            {
                SaveLine();
            }
            LoadLine(lineWindow.storedLine);
            showLineEditor = true;
        }
        // show delete button
        if (GUILayout.Button("Delete Line"))
        {
            lineWindow.parentBranch.dialogLines.Remove(lineWindow.storedLine);
            lineWindows.Remove(lineWindow);
            lineWindow.storedLine = null;
        }

        if (GUILayout.Button("Add Reference"))
        {
            if (referenceStartIndex == -1)
            {
                referenceStartIndex = id;
            }
        }

        // left click
        if (Event.current.button == 0 && Event.current.type == EventType.MouseDown)
        {
            // mouse is over lineRect?
            if (referenceStartIndex !=  -1)
            {
                // setup reference
                lineWindows[referenceStartIndex].referencedWindows.Add(lineWindow);
                lineWindows[referenceStartIndex].storedLine.lineBranches.Add(lineWindow.storedLine);

                // reset temp data
                referenceStartIndex = -1;
            }
        }

        GUI.DragWindow();
    }

    void EditLine()
    {
        // show Id
        activeLine.lineId = EditorGUILayout.TextField("LineId:", activeLine.lineId);

        // show lineTeaser
        activeLine.lineTeaser = EditorGUILayout.TextField("Teaser:", activeLine.lineTeaser);

        // show finishDialog?
        activeLine.lineFinishDialog = EditorGUILayout.Toggle("Finish:", activeLine.lineFinishDialog);

        // show lineTexts
        GUILayout.Label("Texts:", EditorStyles.boldLabel);
        // setup alias nameList
        string[] aliasNames = new string[activeQuest.questAliases.Count];
        for (int i = 0; i < aliasNames.Length; i++)
        {
            aliasNames[i] = activeQuest.questAliases[i].aliasName;
        }

        // setup update the list of indices for speaking aliases
        if (speakerIndices == null) speakerIndices = new List<int>(); 
        while(speakerIndices.Count < activeLine.lineTexts.Count)
        {
            speakerIndices.Add(0);
        }
        
        for (int i = 0; i < activeLine.lineTexts.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            // text
            activeLine.lineTexts[i].text = EditorGUILayout.TextArea(activeLine.lineTexts[i].text);

            // speaker
            if (activeQuest.questAliases.Count > 0)
            {
                speakerIndices[i] = EditorGUILayout.Popup(speakerIndices[i], aliasNames);
                activeLine.lineTexts[i].speaker = activeQuest.questAliases[speakerIndices[i]];
                activeLine.lineTexts[i].testAlias = activeQuest.testAlias;
            }
            else
            {
                EditorGUILayout.LabelField("No Speaker Available");
            }
            // remove text
            if (GUILayout.Button("Delete"))
            {
                activeLine.lineTexts.RemoveAt(i);
            }
            EditorGUILayout.EndHorizontal();
        }

        // create new text
        if (GUILayout.Button("Add Text"))
        {
            activeLine.lineTexts.Add(new LinePart(null, "new Text"));
        }


        // show connectedLines
        GUILayout.Label("Branches:", EditorStyles.boldLabel);
        for (int i = 0; i < activeLine.lineBranches.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(activeLine.lineBranches[i].lineId);

            // remove branch
            if (GUILayout.Button("Delete"))
            {
                activeLine.lineBranches.RemoveAt(i);
            }
            EditorGUILayout.EndHorizontal();
        }

        // show lineAction
        activeLine.lineAction = (LineAction)EditorGUILayout.EnumPopup("Action", activeLine.lineAction);

        // show lineType
        activeLine.lineType = (LineType)EditorGUILayout.EnumPopup("Type", activeLine.lineType);

        // show conditions
        ConditionEditor.EditorBuildIn(activeLine.lineConditions, activeQuest.questAliases);

        // save changes
        if (GUILayout.Button("Save"))
        {
            SaveLine();
        }
    }

    void LoadQuest(Quest newQuest)
    {
        activeQuest = newQuest;
        // activeBranches = newQuest.dialogBranches.ToList();
        lineWindows = new List<LineWindow>();

        // setup lines
        for (int i = 0; i < activeQuest.dialogBranches.Count; i++)
        {
            for (int j = 0; j < activeQuest.dialogBranches[i].dialogLines.Count; j++)
            {
                lineWindows.Add(new LineWindow(activeQuest.dialogBranches[i].dialogLines[j], activeQuest.dialogBranches[i]));
            }
        }

        // setup references
        for (int i = 0; i < lineWindows.Count; i++)
        {
            for (int j = 0; j < lineWindows[i].storedLine.lineBranches.Count; j++)
            {
                for (int k = 0; k < lineWindows.Count; k++)
                {
                    if (lineWindows[i].storedLine.lineBranches[j].lineId == lineWindows[k].storedLine.lineId)
                    {
                        lineWindows[i].referencedWindows.Add(lineWindows[k]);
                        break;
                    }
                }
            }
        }
    }

    void SaveQuest()
    {
        // activeQuest.dialogBranches = activeBranches.ToArray();

        EditorUtility.SetDirty(questDatabase);
        activeQuest = null;
    }

    void LoadLine(Line line)
    {
        activeLine = line;

        speakerIndices = new List<int>();
        for (int i = 0; i < line.lineTexts.Count; i++)
        {
            speakerIndices.Add(0);
        }

        /*activeTeaser = activeLine.lineTeaser;
        activeTexts = activeLine.lineTexts.ToList();
        activeLineType = activeLine.lineAction;
        activeLineBranches = activeLine.lineBranches.ToList();
        activeConditions = activeLine.lineConditions.ToList();*/
    }

    void SaveLine()
    {
        /*
        activeLine.lineTeaser = activeTeaser;
        activeLine.lineTexts = activeTexts.ToArray();
        activeLine.lineAction = activeLineType;
        activeLine.lineBranches = activeLineBranches.ToArray();
        activeLine.lineConditions = activeConditions.ToArray();*/

        activeLine = null;
    }
}

public class LineWindow
{
    public Rect lineRect;
    public Line storedLine;
    public DialogBranch parentBranch;

    public List<LineWindow> referencedWindows;

    public LineWindow(DialogBranch parentBranch)
    {
        this.lineRect = new Rect(350f, 50, 0, 0);
        this.storedLine = new Line();
        this.parentBranch = parentBranch;
        referencedWindows = new List<LineWindow>();
    }

    public LineWindow(Line storedLine, DialogBranch parentBranch)
    {
        this.lineRect = new Rect(350f, 50, 0, 0);
        this.storedLine = storedLine;
        this.parentBranch = parentBranch;
        referencedWindows = new List<LineWindow>();
    }

    public LineWindow(Rect lineRect, Line storedLine)
    {
        this.lineRect = lineRect;
        this.storedLine = storedLine;
        referencedWindows = new List<LineWindow>();
    }
}