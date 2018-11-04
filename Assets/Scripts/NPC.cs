using UnityEngine;
using System.Collections;

[System.Serializable]
public class NPC : MonoBehaviour
{
    // Person
    public static void Create(string name, string tag, string prefabName, int level, int maxHp, Vector3 position)
    {
        // create NPC
        GameObject npc = Instantiate(Resources.Load<GameObject>("NPCs/" + prefabName));

        // setup NPC
        npc.name = name;
        npc.tag = tag;
        npc.transform.position = position;
    }

}
