using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DialogStatusManager : MonoBehaviour
{
    private GameObject avatar;
    private DialogList dialogList;
    private Dictionary<string, Line> dialogLines;

    private DialogTree rootNode;
    private DialogTree activeNode;
    private DialogTree dialogStartNode;

    /*
    public DialogTree GetStartPoint()
    {
        return dialogStartNode;
    }

    void Awake()
    {
        // create references
        avatar = transform.root.gameObject;
        dialogList = GameObject.FindWithTag("Database").GetComponent<DialogList>();

        // load dialogLines
        dialogLines = dialogList.GetDialogLines(avatar);

        // create dialogTree
        rootNode = CreateStartTree("S");
        dialogStartNode = rootNode;
    }

    public DialogTree CreateStartTree(string lineId)
    {
        Line line;
        dialogLines.TryGetValue(lineId, out line);

        DialogTree startTree = new DialogTree(lineId, line);

        CreateChildren(startTree);

        return startTree;
    }

    DialogTree CreateChildren(DialogTree parentNode)
    {
        string[] childIds = parentNode.lineData.lineOptions;

        for (int i = 0; i < childIds.Length; i++)
        {
            // get line
            Line childLine;
            dialogLines.TryGetValue(childIds[i], out childLine);

            // create childTree
            DialogTree childNode = new DialogTree(childIds[i], childLine);

            // get childs of child
            if(childLine.lineOptions != null)
            {
                CreateChildren(childNode);
            }

            // add to parent
            parentNode.children.Add(childNode);
        }

        return parentNode;
    }

    public DialogTree CreateNode(string lineId)
    {
        Line line;
        dialogLines.TryGetValue(lineId, out line);

        return new DialogTree(lineId, line);
    }
    
    public DialogTree CreateNode(string lineId, string[] children)
    {
        // search line
        Line line;
        dialogLines.TryGetValue(lineId, out line);

        // create children
        List<DialogTree> childrenNodes = new List<DialogTree>();
        for(int i = 0; i < children.Length;i++)
        {
            childrenNodes.Add(CreateNode(children[i]));
        }

        return new DialogTree(lineId, line, childrenNodes);
    }
    

    public void InsertNode(string insertPointId, DialogTree newNode)
    {
        // search insertPoint
        DialogTree parentNode = null;
        if (!SearchNode(rootNode, ref parentNode, insertPointId))
        {
            Debug.LogError("Insert point not existing");
            return;
        }

        // insert new node
        parentNode.children.Add(newNode);
    }

    public void RemoveNode(string nodeId)
    {
        // search node to remove
        DialogTree searchedNode = null;
        if (!SearchParentNode(rootNode, ref searchedNode, nodeId))
        {
            Debug.LogError("node not existing");
            return;
        }

        // remove node
        searchedNode.children.Remove(searchedNode);
    }

    public bool SearchNode(DialogTree startNode, ref DialogTree insertNode, string id)
    {
        if (startNode.id == id)
        {
            insertNode = startNode;
            return true;
        }

        for (int i = 0; i < startNode.children.Count; i++)
        {
            if (SearchNode(startNode.children[i], ref insertNode, id))
            {
                return true;
            }
        }

        return false;
    }

    public bool SearchParentNode(DialogTree startNode, ref DialogTree parentNode, string id)
    {
        for (int i = 0; i < startNode.children.Count; i++)
        {
            if (startNode.children[i].id == id)
            {
                parentNode = startNode;
                return true;
            }

            if (SearchParentNode(startNode.children[i], ref parentNode, id))
            {
                return true;
            }
        }

        return false;
    }*/
}
