using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(RoutineList), true)]
public class ScheduleEditor : Editor // Window
{
    private static RoutineList database;
    private GameObject curNPC;
    private Routine curSchedule;

    private Activity curActivity;
    
    // [MenuItem("Databases/ScheduleEditor")]
    void OnEnable() // static void Init()
    {
        database = target as RoutineList;
        /*
        // open new Window
        EditorWindow.GetWindow(typeof(ScheduleEditor));

        // get/create database
        Object scheduleDatabase = Resources.Load<RoutineList>("Databases/RoutineDatabase");
        if (scheduleDatabase == null)
        {
            database = CreateDatabases.CreateRoutineDatabase();
        }
        else
        {
            database = Resources.Load<RoutineList>("Databases/RoutineDatabase");
        }*/
    }

    public override void OnInspectorGUI() // void OnGui()
    {
        // no npc setup
        if (curNPC == null)
        {
            SetupNPC();
        }
        // npc setup
        else
        {
            EditSchedule();
        }

        base.OnInspectorGUI();
        /*
        int count = routineList.GetRoutineCount();

        // show in inspector
        GUILayout.Label("Routines:", EditorStyles.boldLabel);
        GUILayout.Label("Count: " + count);*/
    }

    void SetupNPC()
    {
        GameObject newNPC;

        // Setup Enemy to Inspect
        GUILayout.Label("Chose NPC", EditorStyles.boldLabel);
        newNPC = (GameObject)EditorGUILayout.ObjectField(curNPC, typeof(GameObject), true);

        // object dropped in
        if (newNPC)
        {
            // object is npc
            if (newNPC.GetComponentInChildren<Schedule>(true))
            {
                // update npc
                curNPC = newNPC;
                curActivity = null;

                // database contains no routine for npc
                if (!database.GetDailyRoutine(curNPC, ref curSchedule))
                {
                    // create new routine
                    // Debug.Log("has no routine");
                    curSchedule = new Routine(curNPC, new List<Activity>());
                }

            }
        }
    }

    void EditSchedule()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("current npc: " + curNPC.name, EditorStyles.boldLabel);

        // reset button
        if (GUILayout.Button("Reset NPC"))
        {
            curNPC = null;
            curActivity = null;
        }

        EditorGUILayout.EndHorizontal();

        if (curActivity == null)
        { 
            // show routines
            ShowRoutine();
        }
        else
        {
            // show activitys
            ShowActivity();
        }
        // buttons:

        // save schedule
        if (GUILayout.Button("Save"))
        {
            database.SetDailyRoutine(curSchedule);
        }
    }

    void ShowActivity()
    {
        // Name
        curActivity.name = EditorGUILayout.TextField("Name", curActivity.name);

        // Type
        curActivity.type = (Activity.Type)EditorGUILayout.EnumPopup("Type: ", curActivity.type);

        // TaskObjects
        List<GameObject> taskObjects = curActivity.taskObjects;
        for (int i = 0; i < taskObjects.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();

            taskObjects[i] = (GameObject)EditorGUILayout.ObjectField(taskObjects[i], typeof(GameObject), true);

            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();

            if (GUILayout.Button("Remove Object"))
            {
                taskObjects.RemoveAt(i);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }

        // add taskObject
        if (GUILayout.Button("Add Object"))
        {
            taskObjects.Add(null);
        }

        // back to schedule
        if (GUILayout.Button("Back"))
        {
            curActivity = null;
        }
    }
    

    /*
    public void CalculateRect(ref float posX, ref float width, int startHoure, int endHoure)
    {
        posX = timeLineXOffset + ((float)startHoure / 24) * (position.width - 2 * timeLineXOffset);
        width = position.width * ((float)(endHoure - startHoure) / 24);
    }
    */

    public void ShowRoutine()
    {
        List<Activity> activities = curSchedule.activities;

        // show routines
        for (int i = 0; i < activities.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();

            // name
            activities[i].name = EditorGUILayout.TextField("Name", activities[i].name);

            // period
            // startTime
            float oldStartTime = activities[i].period.startHoure + (float)activities[i].period.startMinute / 60;
            float newStartTime = EditorGUILayout.Slider("StartTime: ", oldStartTime, 0, 24);
            if (oldStartTime != newStartTime)
            {
                // Debug.Log("Old: " + curSchedule[i].period.startHoure + ":" + curSchedule[i].period.startMinute + ", New: " + (int)newStartTime  + ":" + (int)((newStartTime - (int)newStartTime) * 60));
                activities[i].period.startHoure = (int)newStartTime;
                activities[i].period.startMinute = (int)((newStartTime - (int)newStartTime) * 60);

                // update previous activity endTime
                Period prevPeriod = activities[(int)Mathf.Repeat(i - 1, activities.Count)].period;

                prevPeriod.endHoure = (int)newStartTime;
                prevPeriod.endMinute = (int)((newStartTime - (int)newStartTime) * 60);
            }

            // endTime
            float oldEndTime = activities[i].period.endHoure + (float)activities[i].period.endMinute / 60;
            float newEndTime = EditorGUILayout.Slider("EndTime: ", oldEndTime, 0, 24);
            if (oldEndTime != newEndTime)
            {
                activities[i].period.endHoure = (int)newEndTime;
                activities[i].period.endMinute = (int)((newEndTime - (int)newEndTime) * 60);

                // update next activity startTime
                Period nextPeriod = activities[(int)Mathf.Repeat(i + 1, activities.Count)].period;

                nextPeriod.startHoure = (int)newEndTime;
                nextPeriod.startMinute = (int)((newEndTime - (int)newEndTime) * 60);
            }
            
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            if (GUILayout.Button("Edit Activity"))
            {
                curActivity = activities[i];
            }

            if (GUILayout.Button("Remove Activity"))
            {
                activities.RemoveAt(i);
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }

        // add routine
        if (GUILayout.Button("Add Activity"))
        {
            activities.Add(new Activity("New Activity", new Period(0, 0, 0, 1), new List<GameObject>(),Activity.Type.InteractingRoutine));
        }


        /*activeRoutine.period.startHoure = Mathf.RoundToInt(EditorGUILayout.Slider("startHoure", activeRoutine.period.startHoure, 0, 24));

        // set pervious endTime
        DatabaseRoutine prev;
        if (routineIndex > 0)
        {
            prev = schedule.database[routineIndex - 1];
        }
        else
        {
            prev = schedule.database[schedule.database.Length - 1];
        }
        prev.period.endHoure = activeRoutine.period.startHoure;

        activeRoutine.period.endHoure = Mathf.RoundToInt(EditorGUILayout.Slider("endHoure", activeRoutine.period.endHoure, 0, 24));

        // set next endTime
        DatabaseRoutine next;
        if (routineIndex == schedule.database.Length - 1)
        {
            next = schedule.database[0];
        }
        else
        {
            next = schedule.database[routineIndex + 1];
        }
        next.period.startHoure = activeRoutine.period.endHoure;

        // show activitys
        GUILayout.Label("Activitys");
        for (int i = 0; i < activeRoutine.activitys.Length; i++)
        {
            ShowActivity(activeRoutine.activitys[i]);
            bool delete = GUILayout.Button("-", GUILayout.ExpandWidth(false));
            if (delete)
            {
                DeleteActivity(ref activeRoutine.activitys, i);
            }
        }

        // show new activity
        ShowActivity(newMultiActivity);
        bool add = GUILayout.Button("Add Activity", GUILayout.ExpandWidth(false));
        if (add && newMultiActivity.usedObjects.Length > 0)
        {
            AddActivity(activeRoutine);
        }*/

    }

    /*
    public void DragRoutine(int index)
    {
        // Routine active = schedule.database[index];

    }


    public void SwitchRoutine(ref DatabaseRoutine[] routineList, int moveIndex, int switchIndex)
    {
        // store moveRoutine
        DatabaseRoutine routineCache = routineList[moveIndex];

        // store period of upper routine
        Period periodcache = routineList[switchIndex].period;

        // switch move, upper routines
        routineList[moveIndex] = routineList[switchIndex];
        routineList[switchIndex] = routineCache;

        // restore periods
        routineList[moveIndex].period = routineCache.period;
        routineList[switchIndex].period = periodcache;
    }


    public void AddRoutine()
    {
        // create new array without new element
        DatabaseRoutine[] newArray = new DatabaseRoutine[schedule.database.Length + 1];
        for (int i = 0; i < schedule.database.Length; i++)
        {
            newArray[i] = schedule.database[i];
        }

        // create new element
        int startHoure = 0;
        int endHoure = 0;
        if (newArray.Length > 1)
        {
            startHoure = newArray[newArray.Length - 2].period.endHoure;
            endHoure = startHoure + 1;
            newArray[0].period.startHoure = endHoure;
        }

        // add new element
        newArray[newArray.Length - 1] = new DatabaseRoutine("new Routine", new Period(startHoure, 0, endHoure, 0), new MultiActivity[0]);

        // replace the old array against newArray
        schedule.database = newArray;
    }


    public void DeleteRoutine(int deleteIndex)
    {
        // create new array without deleting element
        DatabaseRoutine[] newArray = new DatabaseRoutine[schedule.database.Length - 1];
        for (int i = 0; i < schedule.database.Length; i++)
        {
            if (i < deleteIndex)
            {
                newArray[i] = schedule.database[i];
            }
            else if (i > deleteIndex)
            {
                newArray[i - 1] = schedule.database[i];
            }
        }

        // scale bordering routines



        // replace the old array against newArray
        schedule.database = newArray;
    }


    public void ShowActivity(Activity activity)
    {

        EditorGUILayout.BeginHorizontal();
        activity.type = (Activity.Type)EditorGUILayout.EnumPopup(activity.type);
        EditorGUILayout.BeginVertical();
        if (activity.multi)
        {
            MultiActivity mActivity = (MultiActivity)activity;
            for (int i = 0; i < mActivity.usedObjects.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();
                mActivity.usedObjects[i] = (GameObject)EditorGUILayout.ObjectField(mActivity.usedObjects[i], typeof(GameObject), true);
                bool delete = GUILayout.Button("-", GUILayout.ExpandWidth(false));
                if (delete)
                {
                    DeleteObject(ref mActivity.usedObjects, i);
                }
                EditorGUILayout.EndHorizontal();
            }

            GameObject newObject = (GameObject)EditorGUILayout.ObjectField(newObject = null, typeof(GameObject), true);
            if (newObject != null)
            {
                AddObject(ref mActivity.usedObjects, newObject);
            }
        }
        else
        {
            SingleActivity sActivity = (SingleActivity)activity;
            sActivity.usedObject = (GameObject)EditorGUILayout.ObjectField(sActivity.usedObject, typeof(GameObject), true);
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
    }


    public void AddActivity(DatabaseRoutine activeRoutine)
    {
        // transfer old gameobjects to new list
        MultiActivity[] activityList = activeRoutine.activitys;
        MultiActivity[] newArray = new MultiActivity[activityList.Length + 1];
        for (int i = 0; i < activityList.Length; i++)
        {
            newArray[i] = activityList[i];
        }

        // add new activity to array
        newArray[newArray.Length - 1] = newMultiActivity;
        

        // reset global variables
        newMultiActivity = new MultiActivity(new GameObject[0], Activity.Type.Interact);

        // replace the old array against newArray
        activeRoutine.activitys = newArray;

    }


    public void DeleteActivity(ref MultiActivity[] activityList, int activityIndex)
    {
        // create new array without deleting element
        MultiActivity[] newArray = new MultiActivity[activityList.Length - 1];
        for (int i = 0; i < activityList.Length; i++)
        {
            if (i < activityIndex)
            {
                newArray[i] = activityList[i];
            }
            else if (i > activityIndex)
            {
                newArray[i - 1] = activityList[i];
            }
        }

        // replace the old array against newArray
        activityList = newArray;
    }


    public void AddObject(ref GameObject[] objectList, GameObject newObject)
    {

        GameObject[] newArray = new GameObject[objectList.Length + 1];
        for (int i = 0; i < objectList.Length; i++)
        {
            newArray[i] = objectList[i];
        }

        newArray[newArray.Length - 1] = newObject;

        objectList = newArray;
    }


    public void DeleteObject(ref GameObject[] objectList, int deleteIndex)
    {
        // create new array without deleting element
        GameObject[] newArray = new GameObject[objectList.Length - 1];
        for (int i = 0; i < objectList.Length; i++)
        {
            if (i < deleteIndex)
            {
                newArray[i] = objectList[i];
            }
            else if (i > deleteIndex)
            {
                newArray[i - 1] = objectList[i];
            }
        }

        // replace the old array against newArray
        objectList = newArray;
    }*/

    /*
    List<Routine> LoadSchedule()
    {
        Routine[] scheduleArray = database.GetDailyRoutine(curNPC);
        List<Routine> outSchedule = new List<Routine>();

        for(int i = 0;i < scheduleArray.Length;i++)
        {
            outSchedule.Add(scheduleArray[i]);
        }

        return outSchedule;
    }

    void StoreSchedule()
    {
        Routine[] scheduleArray = new Routine[curSchedule.Count];

        for (int i = 0; i < scheduleArray.Length; i++)
        {
            scheduleArray[i] = curSchedule[i];
        }

        database.SetDailyRoutine(curNPC, scheduleArray);
    }*/
}
