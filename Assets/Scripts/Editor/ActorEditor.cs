using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class ActorEditor : EditorWindow
{
    static private ActorDatabaseList actorDatabase;

    // toolBar
    private int toolbarIndex = 0;
    private string[] toolbarStrings = new string[] { "ActorData", "Inventory", "Relationships", "Factions" };

    // active actor
    private Actor activeActor;
    private Faction newFaction;

    [MenuItem("Actor/ActorEditor")]
    static void Init()
    {
        actorDatabase = Resources.Load("Databases/ActorDatabase") as ActorDatabaseList;
        if (actorDatabase == null)
        {
            actorDatabase = DatabaseFactory.CreateActorDatabase();
        }

        ActorEditor window = GetWindow<ActorEditor>();
        window.Show();
    }

    void OnGUI()
    {
        if(actorDatabase == null)
        {
            return;
        }

        if(activeActor == null)
        {
            ChoseActor();
        }
        else
        {
            EditActor();
        }
    }

    void ChoseActor()
    {
        List<Actor> actorList = actorDatabase.GetActorList();
         
        for(int i = 0; i < actorList.Count;i++)
        {
            EditorGUILayout.BeginHorizontal();
            // id
            EditorGUILayout.LabelField("Id:" + actorList[i].id);
            // name
            EditorGUILayout.LabelField("Name:" + actorList[i].name);
            // race
            EditorGUILayout.LabelField("Race:" + actorList[i].race.ToString());
            // uses
            // EditorGUILayout.LabelField("Uses:" + actorList[i].sceenReferences.Count);

            if (GUILayout.Button("Edit"))
            {
                LoadActor(actorList[i]);
            }

            if (GUILayout.Button("Delete"))
            {
               actorDatabase.DeleteActor(actorList[i]);
            }

            EditorGUILayout.EndHorizontal();
        }

        // create new Actor
        if (GUILayout.Button("Add Actor"))
        {
            LoadActor(actorDatabase.CreateNewActor());
        }
    }

    void EditActor()
    {
        // Edit Quest
        toolbarIndex = GUILayout.Toolbar(toolbarIndex, toolbarStrings);

        switch (toolbarIndex)
        {
            case 0:
                {
                    EditAvatarData();
                    break;
                }
            case 1:
                {
                    EditActorInventory();
                    break;
                }
            case 2:
                {
                    EditActorRelationships();
                    break;
                }
            case 3:
                {
                    EditActorFactions();
                    break;
                }
        }

        if (GUILayout.Button("Save"))
        {
            SaveActor();
        }
    }

    void EditAvatarData()
    {
        // id
        activeActor.id = EditorGUILayout.TextField("Id:", activeActor.id);
        // name
        activeActor.name = EditorGUILayout.TextField("Name:", activeActor.name);
        // race
        activeActor.race = (Race)EditorGUILayout.EnumPopup("Race:", activeActor.race);
    }

    void EditActorInventory()
    {

    }

    void EditActorRelationships()
    {

    }

    void EditActorFactions()
    {
        // show existing factions
        for(int i = 0; i < activeActor.factions.Count;i++)
        {
            EditorGUILayout.LabelField(activeActor.factions[i].ToString());

            if (GUILayout.Button("Delete"))
            {
                activeActor.factions.RemoveAt(i);
            }
        }

        // Add new Faction
        EditorGUILayout.BeginHorizontal();
        newFaction = (Faction)EditorGUILayout.EnumPopup("Faction:", newFaction);
        if (activeActor.factions.Contains(newFaction))
        {
            EditorGUILayout.LabelField("Actor is already in " + newFaction);
        }
        else if (GUILayout.Button("Add"))
        {
            activeActor.factions.Add(newFaction);
        }
        EditorGUILayout.EndHorizontal();
    }

    void LoadActor(Actor newActor)
    {
        activeActor = newActor;
    }

    void SaveActor()
    {
        EditorUtility.SetDirty(actorDatabase);
        activeActor = null;
    }
}
