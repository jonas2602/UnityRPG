using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public struct SearchReference
{
    public Rect startWindow;
    public Line searchedLine;

    public SearchReference(Rect startWindow, Line searchedLine)
    {
        this.startWindow = startWindow;
        this.searchedLine = searchedLine;
    }
}

public class DialogEditor : EditorWindow
{
    private ConditionEditor conditionEditor;
    static private DialogEditor window;
    static private DialogDatabaseList dialogDatabase;
    static private Line[] lineList;
    
    // windows
    private List<Rect> lineRects = new List<Rect>();

    // static private List<LineWindow> lineWindows;

    // references
    private Rect newReferenceStart = new Rect();
    private Line startLine = null;

    // active line
    private Line activeLine;
    private string activeId;
    private string activeTeaser;
    private List<LinePart> activeTexts;
    private LineAction activeLineType;
    private List<Line> activeBranches;
    private List<Condition> activeConditions;

    
    [MenuItem("Dialog/DialogEditor")]
    static void Init()
    {
        dialogDatabase = Resources.Load("Databases/DialogDatabase") as DialogDatabaseList;
        if (dialogDatabase == null)
        {
            dialogDatabase = DatabaseFactory.CreateDialogDatabase();
        }
        /*
        lineList = dialogDatabase.GetAllLines();
        lineWindows = new List<LineWindow>();
        List<SearchReference> lineReferences = new List<SearchReference>();

        // create windows
        for (int i = 0; i < lineList.Length; i++)
        {
            lineWindows.Add(new LineWindow(new Rect(), lineList[i]));

            // add references to other lines to the list
            for (int j = 0; j < lineList[j].lineBranches.Length; j++)
            {
                lineReferences.Add(new SearchReference(lineWindows[i], lineList[i].lineBranches[j]));
            }
        }

        // setup references
        for (int i = 0; i < lineReferences.Count; i++)
        {
            for (int j = 0; j < lineWindows.Count; j++)
            {
                Debug.Log(lineReferences[i].searchedLine.lineId + ", " + lineWindows[j].storedLine.lineId);
                if (lineReferences[i].searchedLine == lineWindows[j].storedLine)
                {
                    Debug.Log(lineReferences[i].searchedLine + " found");
                    lineWindows[i].referencedWindows.Add(lineWindows[j].lineRect);
                }
            }
        }
        */
        window = GetWindow<DialogEditor>(typeof(DialogEditor));
        window.Show();
    }

    public void OnGUI()
    {
        if (activeLine == null)
        {
            // show line overview
            ShowLineOverview();
        }
        else
        {
            // edit single line
            EditLine();
        }

        // Repaint the window as wantsMouseMove doesnt trigger a repaint automatically
        if (Event.current.type == EventType.MouseMove)
            Repaint();
    }
    
    void ShowLineOverview()
    {
        // show existing lines
        if (!dialogDatabase)
        {
            return;
        }

        lineList = dialogDatabase.GetAllLines();

        List<SearchReference> linesToDraw = new List<SearchReference>();
        /*
        // create windows
        BeginWindows();
        for (int i = 0; i < lineWindows.Count; i++)
        {
            // draw windows
            lineWindows[i].lineRect = GUILayout.Window(i, lineWindows[i].lineRect, CreateLine, lineList[i].lineId);

            // draw references
            for (int j = 0; j < lineWindows[i].referencedWindows.Count; j++)
            {
                curveFromTo(lineWindows[i].lineRect, lineWindows[i].referencedWindows[j]);
            }
        }
        EndWindows();
        */

        // extend list
        // Vector2 editorCenter = window.position.center;
        while(lineRects.Count < lineList.Length)
        {
            lineRects.Add(new Rect(window.position.width / 2, window.position.height / 2, 0, 0));
        }
        
        // create windows
        BeginWindows();
        for (int i = 0; i < lineList.Length; i++)
        {
            lineRects[i] = GUILayout.Window(i, lineRects[i], CreateLine, lineList[i].lineId);
            
            // add references to other lines to the list
            for(int j = 0; j < lineList[i].lineBranches.Count;j++)
            {
                linesToDraw.Add(new SearchReference(lineRects[i], lineList[i].lineBranches[j]));
            }
        }
        EndWindows();

        // draw existing references
        for (int i = 0; i < linesToDraw.Count; i++)
        {
            for (int j = 0; j < lineList.Length; j++)
            {
                if (linesToDraw[i].searchedLine.lineId == lineList[j].lineId)
                {
                    // draw line
                    Drawing.CurveFromTo(linesToDraw[i].startWindow, lineRects[j]);
                    break;
                }     
            }
        }
        
        // draw new reference
        if (newReferenceStart.position != Vector2.zero)
        {
            Drawing.CurveFromTo(newReferenceStart, new Rect(Event.current.mousePosition, Vector2.zero));
            
            // right click
            if (Event.current.button == 1 && Event.current.type == EventType.MouseDown)
            {
                // stop referencing
                newReferenceStart.position = Vector2.zero;
            }
        }

        // create new line
        if (GUILayout.Button("Create Line"))
        {
            LoadLine(dialogDatabase.CreateNewLine());
        }
        
        // Handles.DrawBezier(new Vector3(100, 200), new Vector3(200, 400), Vector2.down, Vector3.right, Color.red, null, HandleUtility.GetHandleSize(Vector3.zero)*0.1f);
    }

    public void CreateLine(int id)
    {
        Line line = lineList[id];

        // show lineInfo

        // show edit button
        if (GUILayout.Button("Edit Line"))
        {
            LoadLine(line);
        }
        // show delete button
        if (GUILayout.Button("Delete Line"))
        {
            dialogDatabase.DeleteLine(line);
        }

        if (GUILayout.Button("Add Reference"))
        {
            if (newReferenceStart.position == Vector2.zero)
            {
                newReferenceStart = lineRects[id];
                startLine = line;
            }
        }

        // left click
        if (Event.current.button == 0 && Event.current.type == EventType.MouseDown)
        {
            // mouse is over lineRect?
            if (newReferenceStart.position != Vector2.zero)
            {
                // setup reference
                startLine.lineBranches.Add(line);

                // reset temp data
                newReferenceStart.position = Vector2.zero;
            }
        }

        GUI.DragWindow();
    }

    void EditLine()
    {
        // show Id
        activeId = EditorGUILayout.TextField("LineId:", activeId);

        // show lineTeaser
        activeTeaser = EditorGUILayout.TextField("Teaser:", activeTeaser);

        // show lineTexts
        GUILayout.Label("Texts:", EditorStyles.boldLabel);
        for (int i = 0; i < activeTexts.Count;i++)
        {
            EditorGUILayout.BeginHorizontal();
            // text
            activeTexts[i].text = EditorGUILayout.TextField(activeTexts[i].text);

            // speaker
            // activeTexts[i].speaker = EditorGUILayout.Toggle("Spoken by Player", activeTexts[i].player);

            // remove text
            if (GUILayout.Button("Delete"))
            {
                activeTexts.RemoveAt(i);
            }
            EditorGUILayout.EndHorizontal();
        }
        // create new text
        if (GUILayout.Button("Add Text"))
        {
            activeTexts.Add(new LinePart(new QuestAlias(), "new Text"));
        }


        // show connectedLines
        GUILayout.Label("Branches:", EditorStyles.boldLabel);
        for (int i = 0; i < activeBranches.Count;i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(activeBranches[i].lineId);
            EditorGUILayout.LabelField(activeBranches[i].lineTeaser);

            // remove branch
            if (GUILayout.Button("Delete"))
            {
                activeBranches.RemoveAt(i);
            }
            EditorGUILayout.EndHorizontal();
        }

        // show lineAction
        activeLineType = (LineAction)EditorGUILayout.EnumPopup("Action", activeLineType);

        // show conditions
        GUILayout.Label("Conditions:", EditorStyles.boldLabel);
        for (int i = 0; i < activeConditions.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();

            // show function
            if(activeConditions[i].function != null)
                EditorGUILayout.TextField(activeConditions[i].function.ToString());
            else
                EditorGUILayout.TextField("");
            // show comparison
            if (activeConditions[i].comparison != null)
                EditorGUILayout.TextField(activeConditions[i].comparison.ToString());
            else
                EditorGUILayout.TextField("");
            // show result
            EditorGUILayout.TextField(activeConditions[i].result.ToString());

            // show target
            // EditorGUILayout.TextField(activeConditions[i].target.ToString());

            // show edit button
            if (GUILayout.Button("Edit Condition"))
            {
                if (conditionEditor)
                {
                    conditionEditor.Close();
                }
                conditionEditor = GetWindow<ConditionEditor>();
                // conditionEditor.Init(activeConditions[i]);
            }
            // show delete button
            if (GUILayout.Button("Delete Condition"))
            {
                activeConditions.RemoveAt(i);
            }
            EditorGUILayout.EndHorizontal();
        }
        // create new text
        if (GUILayout.Button("Add Condition"))
        {
            activeConditions.Add(new Condition());
        }

        // save changes
        if (GUILayout.Button("Save"))
        {
            SaveLine();
            activeLine = null;
        }

        if (GUILayout.Button("Cancel"))
        {
            activeLine = null;
        }
    }

    void LoadLine(Line line)
    {
        activeLine = line;
        activeId = activeLine.lineId;
        activeTeaser = activeLine.lineTeaser;
        activeTexts = activeLine.lineTexts;
        activeLineType = activeLine.lineAction;
        activeBranches = activeLine.lineBranches;
        activeConditions = activeLine.lineConditions;
    }

    void SaveLine()
    {
        activeLine.lineId = activeId;
        activeLine.lineTeaser = activeTeaser;
        activeLine.lineTexts = activeTexts;
        activeLine.lineAction = activeLineType;
        activeLine.lineBranches = activeBranches;
        activeLine.lineConditions = activeConditions;
    }
}
