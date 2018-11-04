using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DatabaseFactory
{
#if UNITY_EDITOR
    public static QuestDatabaseList CreateQuestDatabase()                                         // creates a new ItemDatabase(new instance)
    {
        QuestDatabaseList asset = ScriptableObject.CreateInstance<QuestDatabaseList>();                            // of the ScriptableObject InventoryItemList

        AssetDatabase.CreateAsset(asset, "Assets/Resources/Databases/QuestDatabase.asset");    // in the Folder Assets/Resources/ItemDatabase.asset
        AssetDatabase.SaveAssets();                                                             // and than saves it there
        return asset;
    }

    public static DialogDatabaseList CreateDialogDatabase()                                        
    {
        DialogDatabaseList asset = ScriptableObject.CreateInstance<DialogDatabaseList>();                           

        AssetDatabase.CreateAsset(asset, "Assets/Resources/Databases/DialogDatabase.asset");   
        AssetDatabase.SaveAssets();                                                             
        return asset;
    }

    public static ActorDatabaseList CreateActorDatabase()
    {
        ActorDatabaseList asset = ScriptableObject.CreateInstance<ActorDatabaseList>();

        AssetDatabase.CreateAsset(asset, "Assets/Resources/Databases/ActorDatabase.asset");
        AssetDatabase.SaveAssets();
        return asset;
    }
#endif
}
