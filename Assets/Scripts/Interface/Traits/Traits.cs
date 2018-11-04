using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Traits : MonoBehaviour
{

    public TraitList traitDatabase;
    public GUISkin skin;
    public PlayerAttributes attribute;

    public int showLine = -1;

    public float dragDistance;

    public Vector2 freePos = new Vector2(180, 30);
    public Vector2 freeSize = new Vector2(400, 25);

    //Overview
    public Vector2 windowPos = new Vector2(90, 30);
    public Vector2 windowSize = new Vector2(600/*width*/, 400/*height*/);

    public Vector2 treePos = new Vector2(50, 50);
    public Vector2 treeSize = new Vector2(150/*width*/, 300/*height*/);

    public float lineDistance = 15f;

    //Line
    public Vector2 barPos = new Vector2(20, 20);
    public Vector2 barSize = new Vector2(50, 350);

    public float traitSizeLine = 30;
    public float traitSizeOverview = 20;

    public Vector2 traitSpaceOverviewPos = new Vector2(10, 65);
    public Vector2 traitSpaceOverviewSize = new Vector2(130, 220);

    public Vector2 traitSpaceLinePos = new Vector2(100, 20);
    public Vector2 traitSpaceLineSize = new Vector2(470, 350);



    // Use this for initialization
    void Start()
    {
        traitDatabase = GameObject.FindWithTag("TraitDatabase").GetComponent<TraitList>();
        attribute = transform.root.GetComponent<PlayerAttributes>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DrawTraits()
    {
        int freePoints = attribute.GetFreeSkillPoints;

        // Draw Window
        GUI.Box(new Rect(windowPos.x, windowPos.y, windowSize.x, windowSize.y), "", skin.GetStyle("Inventory"));

        // Draw free points
        if (freePoints > 0)
        {
            Rect freeRect = new Rect(freePos.x, freePos.y, freeSize.x, freeSize.y);
            GUI.Box(freeRect, freePoints + " Point(s) left to Update Traits");
        }

        // Draw content
        if (showLine == -1)
        {
            DrawOverview();
        }
        else
        {
            DrawLine();
        }
    }

    void DrawOverview()
    {
        // Move Lines
        if (Event.current.button == 1 && Event.current.type == EventType.MouseDown)
        {
            dragDistance = Event.current.mousePosition.x - treePos.x;
        }
        if (Event.current.button == 1 && Event.current.type == EventType.MouseDrag)
        {
            treePos.x = Event.current.mousePosition.x - dragDistance;
            if (treePos.x > traitDatabase.traitLines.Count * treeSize.x + traitDatabase.traitLines.Count * lineDistance - windowPos.x)
            {
                treePos.x = -windowPos.x;
            }
        }

        //Draw Traitlines
        for (int i = 0; i < traitDatabase.traitLines.Count; i++)
        {
            TraitLine curLine = traitDatabase.traitLines[i];

            // Draw LineWindows
            Rect lineRect;
            if (windowPos.x + treePos.x + i * treeSize.x + i * lineDistance < Screen.width)
            {
                lineRect = new Rect(windowPos.x + treePos.x + i * treeSize.x + i * lineDistance, windowPos.y + treePos.y, treeSize.x, treeSize.y);
            }
            else
            {
                int dif = traitDatabase.traitLines.Count - i;
                lineRect = new Rect(windowPos.x + treePos.x - dif * treeSize.x - dif * lineDistance, windowPos.y + treePos.y, treeSize.x, treeSize.y);
            }
            GUI.Box(lineRect, curLine.name + "\n" + GetTitel(curLine.points) + ": " + curLine.points, skin.GetStyle("Inventory"));

            // Look for interaction
            if (lineRect.Contains(Event.current.mousePosition))
            {
                if (Event.current.button == 0 && Event.current.type == EventType.MouseDown)
                {
                    showLine = i;
                }
            }
            if (curLine.lineTraits.Count > 0)
            {
                //Draw TraitSpace
                Rect spaceRect = new Rect(lineRect.xMin + traitSpaceOverviewPos.x, lineRect.yMin + traitSpaceOverviewPos.y, traitSpaceOverviewSize.x, traitSpaceOverviewSize.y);
                GUI.Box(spaceRect, "", skin.GetStyle("Inventory"));

                // Draw simlyfied Traits
                for (int j = 0; j < traitDatabase.traitLines[i].lineTraits.Count; j++)
                {
                    Trait curTrait = traitDatabase.traitLines[i].lineTraits[j];

                    Rect traitRect = new Rect(spaceRect.xMin + (spaceRect.width - traitSizeOverview) * curTrait.posXPercent / 100, spaceRect.yMin + spaceRect.height * ((float)curTrait.pointsToLearn / 400), traitSizeOverview, traitSizeOverview);
                    GUI.Box(traitRect, "", skin.GetStyle("Inventory"));
                    if (curTrait.curLevel > 0)
                    {
                        // white shine
                        if (curTrait.icon)
                        {
                            // GUI.DrawTexture(traitRect, curTrait.icon);
                        }
                    }
                    else
                    {
                        // black
                        if (curTrait.icon)
                        {
                            // GUI.DrawTexture(traitRect, curTrait.icon);
                        }
                    }
                }
            }
        }
        /*
         * Wenn man ganz nach rechts gescrollt hat fängt es wieder mit dem ersten trait an
         */
    }
    void DrawLine()
    {
        Rect barRect = new Rect(windowPos.x + barPos.x, windowPos.y + barPos.y, barSize.x, barSize.y);
        Rect fillRect = new Rect(windowPos.x + barPos.x, windowPos.y + barPos.y, barSize.x, barSize.y * ((float)traitDatabase.traitLines[showLine].points / 400));
        GUI.Box(barRect, "", skin.GetStyle("Inventory"));
        if (fillRect.height > 0)
        {
            GUI.Box(fillRect, "", skin.GetStyle("Inventory"));
        }
        Rect spaceRect = new Rect(windowPos.x + traitSpaceLinePos.x, windowPos.y + traitSpaceLinePos.y, traitSpaceLineSize.x, traitSpaceLineSize.y);
        GUI.Box(spaceRect, "", GUIStyle.none);

        for (int i = 0; i < traitDatabase.traitLines[showLine].lineTraits.Count; i++)
        {
            Trait curTrait = traitDatabase.traitLines[showLine].lineTraits[i];

            Rect traitRect = new Rect(windowPos.x + traitSpaceLinePos.x + (traitSpaceLineSize.x - traitSizeLine) * curTrait.posXPercent / 100, windowPos.y + traitSpaceLinePos.y + traitSpaceLineSize.y * ((float)curTrait.pointsToLearn / 400), traitSizeLine, traitSizeLine);
            GUI.Box(traitRect, curTrait.name + curTrait.curLevel + "/" + curTrait.maxLevel, skin.GetStyle("Inventory"));
            if (curTrait.icon)
            {
                // GUI.DrawTexture(traitRect, curTrait.icon);
            }
            if (traitRect.Contains(Event.current.mousePosition))
            {
                ShowTraitInfo(curTrait);

                if (Event.current.button == 0 && Event.current.type == EventType.MouseDown)
                {
                    if (attribute.GetFreeSkillPoints > 0 && curTrait.curLevel < curTrait.maxLevel && traitDatabase.traitLines[showLine].points >= curTrait.pointsToLearn)
                    {
                        curTrait.curLevel++;
                        attribute.freeSkillPoints--;
                    }
                }
            }
        }
        if (Input.GetAxis("Mouse ScrollWheel") < -0.05)
        {
            showLine = -1;
        }
    }


    /*
     * Master   = 400
     * Expert   < 400
     * Geselle  < 300
     * Lehrling < 200
     * Beginner < 100
     */
    string GetTitel(int points)
    {
        if (points < 100)
        {
            return "Beginner";
        }
        else if (points < 200)
        {
            return "Lehrling";
        }
        else if (points < 300)
        {
            return "Geselle";
        }
        else if (points < 400)
        {
            return "Expert";
        }
        else
        {
            return "Master";
        }
    }
    /*
    struct Lane
    {
        //public int size;
        public List<Trait> extend;
        public List<Trait> nextend;
        public void Init()
        {
            //size = 0;
            extend = new List<Trait>();
            nextend = new List<Trait>();
        }
    }
    
    void CalculateTraitPositions(TraitLine line, ref float[] pos,float width)
    {
        Lane[] border = new Lane[5];
        List<Trait>[] sortTraits = new List<Trait>[5];
        for (int i = 0; i < border.Length; i++)
        {
            border[i].Init();
        }

        for (int i = 0; i < line.lineTraits.Count; i++)
        {
            Trait curTrait = line.lineTraits[i];
            //border[curTrait.pointsToLearn / 100].size++;
            if(curTrait.extends == null)
            {
                border[curTrait.pointsToLearn / 100].nextend.Add(curTrait);
            }
            else
            {
                border[curTrait.pointsToLearn / 100].extend.Add(curTrait);
            }
        }

        for(int i = 0; i < 5;i++)
        {
            sortTraits
        }

        int longest = 0;
        for (int i = 0; i < border.Length; i++)
        {
            if (border[i].size > longest)
            {
                longest = border[i].size;
            }
        }

        Trait[][] positions = new Trait[][] { new Trait[longest], new Trait[longest], new Trait[longest], new Trait[longest], new Trait[longest] };
        // go through lanes
        for (int i = 0; i < border.Length; i++)
        {
            // go through lane Traids
            for (int j = 0; j < border[i].size; j++)
            {
                // are there more traits which extend?
                if(j < border[i].extend.Count)
                {
                    // search for extended trait in positions
                    for (int k = 0; k < longest; k++)
                    {
                        int laneToSearch = border[i].extend[j].pointsToLearn / 100;
                        if (positions[laneToSearch][k] == border[i].extend[j].extends)
                        {
                            positions[i][k] = border[i].extend[j];
                        }
                    }
                }
                else
                {
                    // go through positions lane traids
                    for(int l = 0; l < longest;l++)
                    {
                        // position in lane is free? 
                        if (positions[i][l] == null)
                        {
                            // add non extendend trait to free slot in positions
                            positions[i][l] = border[i].nextend[j - border[i].extend.Count];
                            break;
                        }
                    }
                }
            }
        }
        string output = "";

        for(int i = 0; i < positions.Length;i++)
        {
            for (int j = 0; j < positions[0].Length; j++)
            {
                if (positions[i][j] != null)
                {
                    output += positions[i][j].name;
                }
                else
                {
                    output += "null";
                }
            }
            output += "\n"; 
        }
        Debug.Log(output);
        
        /*
        for(int i = 0; i < line.lineTraits.Count; i++)
        {
            int borderID = line.lineTraits[i].pointsToLearn / 100;
            pos[i] = width / border[borderID].x * border[borderID].y;
            border[borderID].y++;
        }
        
        
        string output = "";
        for (int j = 0; j < border.Length; j++)
        {
            output += border[j];
        }
        Debug.Log(output);
        
    }
    */

    void ShowTraitInfo(Trait cur)
    {

    }

}
