using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ReferenceManager : MonoBehaviour
{
    private List<ActorReference> sceneReferences; 
    // private List<ItemReference> itemReferences

    public Actor GetActorFromAvatar(GameObject avatar)
    {
        for (int i = 0; i < sceneReferences.Count; i++)
        {
            for (int j = 0; j < sceneReferences[i].avatars.Count; j++)
            {
                if (sceneReferences[i].avatars[j] == avatar)
                {
                    return sceneReferences[i].actor;
                }
            }
        }
        return null;
    }

    public List<GameObject> GetReferencesFromActor(Actor actor)
    {
        for (int i = 0; i < sceneReferences.Count; i++)
        {
            if(sceneReferences[i].actor == actor)
            {
                return sceneReferences[i].avatars;
            }
        }

        return null;
    }

    public void AddAvatarToScene(Actor actor)
    {
        // create new GameObject
        GameObject avatar = null; //Instantiate(actor.prefab);

        ActorReference actorReference = null;
        // Search Actor in Dictionary
        for(int i = 0; i < sceneReferences.Count;i++)
        {
            if(sceneReferences[i].actor == actor)
            {
                actorReference = sceneReferences[i];
                break;
            }
        }

        // Actor exists in Dictionary?
        if(actorReference == null)
        {
            // Create new entry
            sceneReferences.Add(new ActorReference(actor, new List<GameObject>() { avatar }));
        }
        else
        {
            // Add Avatar to existing entry in Dictionary
            actorReference.avatars.Add(avatar);
        }

    }

    public void RemoveAvatarFromScene(GameObject avatar)
    {
        // Destroy GameObject

        // Remove Reference from Dictionary
    }
}

[System.Serializable]
public class ActorReference
{
    public Actor actor;
    public List<GameObject> avatars;

    public ActorReference(Actor actor)
    {
        this.actor = actor;
        avatars = new List<GameObject>();
    }

    public ActorReference(Actor actor, List<GameObject> avatars)
    {
        this.actor = actor;
        this.avatars = avatars;
    }
}
