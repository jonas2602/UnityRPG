using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class DialogTree
{
    public string id;
    public Line lineData;
    public List<DialogTree> children;

    public DialogTree(string id, Line lineData)
    {
        this.id = id;
        this.lineData = lineData;
        this.children = null;
    }

    public DialogTree(string id, Line lineData, List<DialogTree> children)
    {
        this.id = id;
        this.lineData = lineData;
        this.children = children;
    }
}

