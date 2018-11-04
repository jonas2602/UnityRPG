using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DialogDatabaseList : ScriptableObject
{
    [SerializeField]
    private List<Line> dialogLines = new List<Line>();

    public Line CreateNewLine()
    {
        Line newLine = new Line();
        dialogLines.Add(newLine);

        return newLine;
    }

    public void DeleteLine(Line line)
    {
        dialogLines.Remove(line);
    }

    public Line[] GetAllLines()
    {
        return dialogLines.ToArray();
    }

    public void GetLinesForActor()
    {
        // search for all "S"
        // conditions proved

        // send back
    }
}
