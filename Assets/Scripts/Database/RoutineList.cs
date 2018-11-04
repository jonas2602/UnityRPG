using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class RoutineList : MonoBehaviour
{
    [SerializeField]
    private List<Routine> scheduleList = new List<Routine>();


    public bool GetDailyRoutine(GameObject npc, ref Routine npcSchedule)
    {
        for (int i = 0; i < scheduleList.Count; i++)
        {
            // is there a schedule for this npc?
            if (scheduleList[i].npc == npc)
            {
                // get daily routine
                npcSchedule = scheduleList[i];
                return true;
            }
        }

        return false;
    }

    public void SetDailyRoutine(Routine routine)
    {
        // remove old
        scheduleList.Remove(routine);
        
        // set new 
        scheduleList.Add(routine);
    }

    public void ClearList()
    {
        scheduleList.Clear();
    }

    public int GetRoutineCount()
    {
        return scheduleList.Count;
    }
}
