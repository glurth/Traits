using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(SerializableNestedList<>),true)]
public class SerializableNestedListPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var itemsProp = property.FindPropertyRelative("internalList");
        EditorGUI.PropertyField(position, itemsProp, label, true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var itemsProp = property.FindPropertyRelative("internalList");
        return EditorGUI.GetPropertyHeight(itemsProp, true);
    }
}
