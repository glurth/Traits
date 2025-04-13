using UnityEditor;
using UnityEngine;
using EyE.Collections.UnityAssetTables;
using System.Collections;
/*
//[CustomEditor(typeof(ImmutableTable<>), true)]
public class ImmutableTableEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Debug.Log("Immutable table editor");

        // Get the serialized property
        SerializedProperty listProperty = serializedObject.FindProperty("serializedList");

        // Start observing property changes
        serializedObject.Update();

        if (listProperty != null && listProperty.isArray)
        {
            // Draw the list in a foldable UI
            listProperty.isExpanded = EditorGUILayout.Foldout(listProperty.isExpanded, listProperty.displayName);

            if (listProperty.isExpanded)
            {
                // Increase indentation for better hierarchy visualization
                EditorGUI.indentLevel++;
                for (int i = 0; i < listProperty.arraySize; i++)
                {
                    SerializedProperty element = listProperty.GetArrayElementAtIndex(i);

                    // Use the default Unity property drawer for each element
                    DrawDefaultTableElementSerializedProperty(element);//, new GUIContent($"Element {i}"), true);

                }

                // Buttons to add/remove elements from the list
                EditorGUILayout.Space();
                if (GUILayout.Button("Add Element"))
                {
                    listProperty.arraySize++;
                }

                if (GUILayout.Button("Remove Last Element") && listProperty.arraySize > 0)
                {
                    listProperty.arraySize--;
                }

                EditorGUI.indentLevel--;
            }
        }
        else
        {
            EditorGUILayout.LabelField("List property 'serializedList' not found or not an array.");
        }

        // Apply property changes
        serializedObject.ApplyModifiedProperties();
    }

    private void DrawDefaultTableElementSerializedProperty(SerializedProperty property)
    {
        Debug.Log("DefDrawer");
        // Draw each field of the serialized object manually to avoid custom drawers
        SerializedProperty currentProperty = property.Copy();
        SerializedProperty endProperty = property.GetEndProperty();

        while (currentProperty.NextVisible(true) && !SerializedProperty.EqualContents(currentProperty, endProperty))
        {
            if(currentProperty.name != "id")
                EditorGUILayout.PropertyField(currentProperty, true);
        }
    }
}*/