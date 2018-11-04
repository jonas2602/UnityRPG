using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class ActorDatabaseList : ScriptableObject
{
    [SerializeField]
    private List<Actor> actorList = new List<Actor>();

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public Actor CreateNewActor()
    {
        Actor newActor = new Actor();
        actorList.Add(newActor);

        return newActor;
    }

    public void DeleteActor(Actor actor)
    {
        actorList.Remove(actor);
    } 

    public List<Actor> GetActorList()
    {
        return actorList;
    }

    /*public Actor GetActorByName(string name)
    {
        return actorList[0];
    }

    public string[] GetActorNames()
    {
        return actorList.ToArray();
    }*/
}
