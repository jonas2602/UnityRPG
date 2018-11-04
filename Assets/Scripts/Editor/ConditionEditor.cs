using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;

public class ConditionEditor : EditorWindow
{
    private static ConditionEditor conditionEditor;

    private Condition condition;
    private List<QuestAlias> aliasList;
    private int selectedAliasIndex = 0;
    private int selectedComparisonIndex = 0;
    private int selectedFunctionIndex = 0;
    private static string[] typeList = new string[0];
    private ConstructorInfo curConstructor;
    private UnityEngine.Object[] parameters;

    private int result;

    [MenuItem("Dialog/ConditionEditor")]
    public void Init(Condition condition, List<QuestAlias> aliasList)
    {
        typeList = new string[Condition.authorizedFunctions.Length];
        for(int i = 0; i < typeList.Length;i++)
        {
            typeList[i] = Condition.authorizedFunctions[i].ToString();
        }

        this.condition = condition;
        this.aliasList = aliasList;

        conditionEditor = GetWindow<ConditionEditor>(typeof(ConditionEditor));
        conditionEditor.Show();
    }

    void OnGUI()
    {
        if(condition == null)
        {
            condition = new Condition();
        }

        ShowCondition();
    }

    void ShowCondition()
    {
        EditorGUILayout.BeginHorizontal();
        
        // show condition function
        EditorGUILayout.BeginVertical();
        GUILayout.Label("Condition Function", EditorStyles.boldLabel);
        int newSelectedFunction = EditorGUILayout.Popup(selectedFunctionIndex,typeList);
        if(newSelectedFunction != selectedFunctionIndex || curConstructor == null)
        {
            selectedFunctionIndex = newSelectedFunction;
            curConstructor = Condition.authorizedFunctions[selectedFunctionIndex].GetConstructors()[0];
            parameters = new UnityEngine.Object[curConstructor.GetParameters().Length];
        }
        EditorGUILayout.EndVertical();

        // show parameters
        EditorGUILayout.BeginVertical();
        GUILayout.Label("Parameters", EditorStyles.boldLabel);
        if (curConstructor != null)
        {
            ParameterInfo[] paraInfo = curConstructor.GetParameters();
            for (int i = 0; i < paraInfo.Length; i++)
            {
                parameters[i] = EditorGUILayout.ObjectField(/*paraInfo[i].Name, */parameters[i], paraInfo[i].ParameterType, true);
            }
        }
        EditorGUILayout.EndVertical();

        // show comparison
        EditorGUILayout.BeginVertical();
        GUILayout.Label("Comparison", EditorStyles.boldLabel);
        selectedComparisonIndex = EditorGUILayout.Popup(selectedComparisonIndex, Condition.authorizedComparisons);
        EditorGUILayout.EndVertical();

        // show result
        EditorGUILayout.BeginVertical();
        GUILayout.Label("Value", EditorStyles.boldLabel);
        result = EditorGUILayout.IntField(result);
        EditorGUILayout.EndVertical();

        // show target
        EditorGUILayout.BeginVertical();
        GUILayout.Label("Target", EditorStyles.boldLabel);
        string[] aliasNames = new string[aliasList.Count];
        for (int i = 0; i < aliasNames.Length; i++)
        {
            aliasNames[i] = aliasList[i].aliasName;
        }
        selectedAliasIndex = EditorGUILayout.Popup(selectedAliasIndex, aliasNames);
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();

        // save changes
        if (GUILayout.Button("Ok"))
        {
            if (curConstructor.GetParameters().Length > 0)
            {
                condition.function = (ConditionFunction)curConstructor.Invoke(parameters);
            }
            else
            {
                condition.function = (ConditionFunction)curConstructor.Invoke(null);
            }
            condition.comparison = Condition.authorizedComparisons[selectedComparisonIndex];
            condition.result = result;
            condition.alias = aliasList[selectedAliasIndex];

            conditionEditor.Close();
        }

        // revert changes
        if (GUILayout.Button("Cancel"))
        {
            conditionEditor.Close();
        }
    }

    public static void EditorBuildIn(List<Condition> conditions, List<QuestAlias> aliasList)
    {
        GUILayout.Label("Conditions:", EditorStyles.boldLabel);
        for (int i = 0; i < conditions.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();

            // show function
            if (conditions[i].function != null)
                EditorGUILayout.TextField(conditions[i].function.ToString());
            else
                EditorGUILayout.TextField("");
            // show comparison
            if (conditions[i].comparison != null)
                EditorGUILayout.TextField(conditions[i].comparison.ToString());
            else
                EditorGUILayout.TextField("");
            // show result
            EditorGUILayout.TextField(conditions[i].result.ToString());

            // show target
            if(conditions[i].alias != null)
                EditorGUILayout.TextField(conditions[i].alias.aliasName.ToString());
            else
                EditorGUILayout.TextField("");
            // show edit button
            if (GUILayout.Button("Edit"))
            {
                if (conditionEditor)
                {
                    conditionEditor.Close();
                }
                conditionEditor = GetWindow<ConditionEditor>();
                conditionEditor.Init(conditions[i], aliasList);
            }
            // show delete button
            if (GUILayout.Button("Delete"))
            {
                conditions.RemoveAt(i);
            }
            EditorGUILayout.EndHorizontal();
        }
        // create new text
        if (GUILayout.Button("Add Condition"))
        {
            conditions.Add(new Condition());
        }
    }
}

