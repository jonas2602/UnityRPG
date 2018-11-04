using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(ItemStorage), true)]
public class ItemStorageEditor : Editor
{
    // ItemStorage itemStorage;
    
    void OnEnable()
    {
        // Setup the SerializedProperties.
        // itemStorage = target as ItemStorage; 
    }

    public override void OnInspectorGUI()
    {
        // show default ui
        base.OnInspectorGUI();
        /*
        // Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
        serializedObject.Update();

        GUILayout.Label("This is a Label in a Custom Editor");

        // show inventoryList
        List<Item> inventoryItems = itemStorage.GetInventoryItemList;

        EditorGUILayout.BeginVertical();

        EditorGUILayout.LabelField("Inventory:");
        for (int i = 0; i < inventoryItems.Count;i++)
        {
            EditorGUILayout.LabelField(inventoryItems[i].itemName);
        }

        EditorGUILayout.EndVertical();

        // show money
        // EditorGUILayout.IntSlider(money.itemAmount,0, 1000);

        

        // EditorGUILayout.BeginHorizontal


        // Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
        serializedObject.ApplyModifiedProperties();*/
    }
}
