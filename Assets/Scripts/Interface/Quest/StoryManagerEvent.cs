using UnityEngine;
using System.Collections;

[System.Serializable]
public abstract class StoryManagerEvent
{
    public abstract GameObject[] GetEventData();

    // public static abstract string[] GetActorInfos();

    // public static abstract string[] GetLocationInfos();
}

[System.Serializable]
public class ActorDialogEvent : StoryManagerEvent
{
    private GameObject addressor;
    private GameObject receiver;
    // Location location;

    public ActorDialogEvent(GameObject addressor, GameObject receiver)
    {
        this.addressor = addressor;
        this.receiver = receiver;
    }

    public override GameObject[] GetEventData()
    {
        return new GameObject[] { addressor, receiver };
    }
}

[System.Serializable]
public class ActorDeathEvent : StoryManagerEvent
{
    private GameObject dead;
    private GameObject murder;

    public ActorDeathEvent(GameObject dead, GameObject murder)
    {
        this.dead = dead;
        this.murder = murder;
    }

    public override GameObject[] GetEventData()
    {
        return new GameObject[] { dead, murder };
    }
}