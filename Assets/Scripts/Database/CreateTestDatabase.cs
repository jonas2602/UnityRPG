using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CreateTestDatabase
{
    public static TestDatabaseList asset;                                                  //The List of all Items

#if UNITY_EDITOR
    public static TestDatabaseList createItemDatabase()                                    //creates a new ItemDatabase(new instance)
    {
        asset = ScriptableObject.CreateInstance<TestDatabaseList>();                       //of the ScriptableObject InventoryItemList

        AssetDatabase.CreateAsset(asset, "Assets/Resources/Databases/TestDatabase.asset");            //in the Folder Assets/Resources/ItemDatabase.asset
        AssetDatabase.SaveAssets();                                                         //and than saves it there
        return asset;
    }
#endif
}
