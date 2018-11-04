using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Schedule : MonoBehaviour
{
    public List<Activity> dailyRoutine;
    public List<Activity> currentActivities;

    public Activity optActivity;

    public Activity activeActivity;
    public int taskIndex;


    public void SetupAlerts(GameTime gameTime)
    {
        SetupDailyRoutine();

        if (dailyRoutine.Count > 1)
        {
            // Debug.Log(dailyRoutine.Length);
            for (int i = 0; i < dailyRoutine.Count; i++)
            {
                int startHoure = dailyRoutine[i].period.startHoure;
                gameTime.dailys[startHoure].Add(new GameTime.Action(this.gameObject, "AddActivity", i));

                int endHoure = dailyRoutine[i].period.endHoure;
                gameTime.dailys[endHoure].Add(new GameTime.Action(this.gameObject, "RemoveActivity", i));
            }
        }
    }

    void SetupDailyRoutine()
    {
        // add slots to dailyRoutine
        RoutineList database = GameObject.FindWithTag("Database").GetComponent<RoutineList>();

        Routine newRoutine = null;

        if (database.GetDailyRoutine(this.transform.root.gameObject, ref newRoutine))
        {
            dailyRoutine = newRoutine.activities;
        }

        // setup current activities
        if(dailyRoutine.Count > 0)
        {
            // only one activity?
            if (dailyRoutine.Count == 1)
            {
                // activate only activity
                AddActivity(0);
            }    
            else
            {
                // search current activities
                int curHoure = GameObject.FindWithTag("GameTime").GetComponent<GameTime>().gameTime.houre;
                for (int i = 0; i < dailyRoutine.Count; i++)
                {
                    Period curPeriod = dailyRoutine[i].period;

                    // period until next day
                    if (curPeriod.startHoure > curPeriod.endHoure)
                    {
                        // current houre is between start and 24h or 0h and end
                        if (curPeriod.startHoure < curHoure || curPeriod.endHoure > curHoure)
                        {
                            // add to active
                            AddActivity(i);
                        }
                    }
                    else
                    {
                        // current houre is between start and endtime
                        if (curPeriod.startHoure < curHoure && curPeriod.endHoure > curHoure)
                        {
                            // add to active
                            AddActivity(i);
                        }
                    }
                }
            }
        }
    }

    void AddActivity(int i)
    {
        currentActivities.Add(dailyRoutine[i]);
    }

    void RemoveActivity(int i)
    {
        currentActivities.Remove(dailyRoutine[i]);
    }
    
    // Use this for initialization
    void Start()
    {
        StartCoroutine(ChoseOptActivity());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator ChoseOptActivity()
    {
        for(;;)
        {
            // wait for routines
            if (currentActivities.Count == 0)
            {
                // Debug.LogWarning("no routines available");
                yield return new WaitUntil(() => currentActivities.Count > 0);
                // Debug.LogWarning("continue dailys");
            }

            // Debug.Log("routines available");

            // calculate activity to perform
            int activityIndex = Mathf.RoundToInt(Random.value * (currentActivities.Count - 1));
            optActivity = currentActivities[activityIndex];
            
            yield return new WaitForSeconds(10f);
        }
    }

    public void ChangeActivity()
    {
        activeActivity = optActivity;
        taskIndex = 0;
    }
}
