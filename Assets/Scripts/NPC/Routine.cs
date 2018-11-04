using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Routine
{
    public GameObject npc;
    public List<Activity> activities;

    public Routine(GameObject npc, List<Activity> activities)
    {
        this.npc = npc;
        this.activities = activities;
    }
}

[System.Serializable]
public class Activity
{
    public string name;
    public Period period;
    public List<GameObject> taskObjects;
    public Type type;

    public enum Type
    {
        Patrol,
        InteractingRoutine,
        Stay,
        GuardingRoutine,
        TrainingRoutine
    }

    public Activity(string name, Period period, List<GameObject> taskObjects, Type type)
    {
        this.name = name;
        this.period = period;
        this.taskObjects = taskObjects;
        this.type = type;
    }
}

[System.Serializable]
public class Period
{
    public int startHoure;
    public int startMinute;
    public int endHoure;
    public int endMinute;

    public Period(int startHoure, int startMinute, int endHoure, int endMinute)
    {
        this.startHoure = startHoure;
        this.startMinute = startMinute;

        this.endHoure = endHoure;
        this.endMinute = endMinute;
    }
}

