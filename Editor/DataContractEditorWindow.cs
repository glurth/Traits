/*using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections;
using System.Runtime.Serialization;
using System.Collections.Generic;

public class DataContractEditorWindow : EditorWindow
{
    private GameVersionData gameVersionData;
    private Dictionary<Type, Func<object>> defaultObjectCreationDictionary;

    [MenuItem("Window/DataContract Editor")]
    public static void ShowWindow()
    {
        GetWindow<DataContractEditorWindow>("DataContract Editor");
    }

    private void OnEnable()
    {
        // Initialize default factory methods for specific types
        defaultObjectCreationDictionary = new Dictionary<Type, Func<object>>
        {
            { typeof(TraitDefinition), () => new TraitDefinition(0, "New Trait", ValueType.Number,PositiveValueImpact.Positive,null,1) },
            { typeof(EntityType), () => new EntityType("New Entity") },
            { typeof(BuffType), () => new BuffType("New Buff") }
        };
    }

    private void OnGUI()
    {
        if (gameVersionData == null)
        {
            if (GUILayout.Button("Create New GameVersionData"))
            {
                gameVersionData = new GameVersionData();
            }
            return;
        }

        // Draw GameVersionData with the default factory dictionary
        DrawDataContract(gameVersionData, "GameVersionData", defaultObjectCreationDictionary);
    }
// Common method to draw both fields and properties
private void DrawFieldOrProperty(string name, object value, Type type, object obj, MemberInfo member, Dictionary<Type, Func<object>> objectCreationDictionary)
    {

        if (typeof(IList).IsAssignableFrom(type))
        {
            DrawList(name, value as IList, obj, member, objectCreationDictionary);
        }
        else if (type == typeof(int))
        {
            int newValue = EditorGUILayout.IntField(name, (int)value);
            SetValue(obj, member, newValue);
        }
        else if (type == typeof(float))
        {
            float newValue = EditorGUILayout.FloatField(name, (float)value);
            SetValue(obj, member, newValue);
        }
        else if (type == typeof(string))
        {
            string newValue = EditorGUILayout.TextField(name, (string)value);
            SetValue(obj, member, newValue);
        }
        else if (typeof(UnityEngine.Object).IsAssignableFrom(type))
        {
            UnityEngine.Object newObject = EditorGUILayout.ObjectField(name, (UnityEngine.Object)value, type, false);
            SetValue(obj, member, newObject);
        }
        // Handle other types as needed...
        // Try to draw using Unity's built-in property drawers
        else if (TryDrawWithPropertyDrawer(name, obj, member))
        {
            return; // Property was drawn using Unity's system
        }
    }

    // Helper method to set the value of a field or property using reflection
    private void SetValue(object obj, MemberInfo member, object value)
    {
        if (member is FieldInfo field)
        {
            field.SetValue(obj, value);
        }
        else if (member is PropertyInfo property)
        {
            property.SetValue(obj, value);
        }
    }

    // Recursively display all DataContract members with an optional dictionary of factory functions
    private void DrawDataContract(object obj, string label, Dictionary<Type, Func<object>> objectCreationDictionary)
    {
        if (obj == null) return;

        EditorGUILayout.LabelField(label, EditorStyles.boldLabel);

        // Get fields (public and private)
        FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        foreach (FieldInfo field in fields)
        {
            if (field.GetCustomAttribute<DataMemberAttribute>() != null)
            {
                object fieldValue = field.GetValue(obj);
                DrawFieldOrProperty(field.Name, fieldValue, field.FieldType, obj, field, objectCreationDictionary);
            }
        }

        // Get properties (public and private)
        PropertyInfo[] properties = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        foreach (PropertyInfo property in properties)
        {
            if (property.GetCustomAttribute<DataMemberAttribute>() != null && property.CanRead)
            {
                object propertyValue = property.GetValue(obj);
                DrawFieldOrProperty(property.Name, propertyValue, property.PropertyType, obj, property, objectCreationDictionary);
            }
        }
    }

    // Method to handle drawing and manipulating lists
    private void DrawList(string name, IList list, object obj, MemberInfo member, Dictionary<Type, Func<object>> objectCreationDictionary)
    {
        if (list == null) return;

        EditorGUILayout.LabelField($"{name} (List)", EditorStyles.boldLabel);

        Type elementType = list.GetType().GetGenericArguments()[0];

        for (int i = 0; i < list.Count; i++)
        {
            object element = list[i];

            EditorGUILayout.BeginHorizontal();

            // Draw individual list elements
            DrawDataContract(element, $"{name} Element {i}", objectCreationDictionary);

            if (GUILayout.Button("Remove", GUILayout.Width(60)))
            {
                list.RemoveAt(i);
                break;
            }

            EditorGUILayout.EndHorizontal();
        }

        // Check if the type can be created with a factory or a parameterless constructor
        bool canCreateElement = CanCreateElement(elementType, objectCreationDictionary);
        string addButtonTooltip = canCreateElement
            ? "Add new element to the list"
            : "No factory method or parameterless constructor available for this type.";

        using (new EditorGUI.DisabledScope(!canCreateElement))
        {
            if (GUILayout.Button(new GUIContent("Add Element", addButtonTooltip)))
            {
                object newItem = CreateNewElement(elementType, objectCreationDictionary);
                if (newItem != null)
                {
                    list.Add(newItem);
                }
            }
        }

        // After adding/removing, we set the modified list back
        SetValue(obj, member, list);
    }

    // Check if an element can be created either via a factory method or parameterless constructor
    private bool CanCreateElement(Type elementType, Dictionary<Type, Func<object>> objectCreationDictionary)
    {
        if (objectCreationDictionary.ContainsKey(elementType))
        {
            return true;
        }

        // Check if the type has a parameterless constructor
        return elementType.GetConstructor(Type.EmptyTypes) != null;
    }

    // Create a new element either via the factory method or using Activator
    private object CreateNewElement(Type elementType, Dictionary<Type, Func<object>> objectCreationDictionary)
    {
        if (objectCreationDictionary.ContainsKey(elementType))
        {
            // Use the factory method to create the new instance
            return objectCreationDictionary[elementType]();
        }

        // Use Activator to create a new instance if it has a parameterless constructor
        if (elementType.GetConstructor(Type.EmptyTypes) != null)
        {
            return Activator.CreateInstance(elementType);
        }

        // Return null if the element cannot be created
        return null;
    }

    // Method to try drawing with Unity's built-in property drawers (e.g., enums, vectors, etc.)
    private bool TryDrawWithPropertyDrawer(string name, object obj, MemberInfo member)
    {
        SerializedObject serializedObject = new SerializedObject(obj as UnityEngine.Object);

        // Find the serialized property associated with the field or property name
        SerializedProperty property = serializedObject.FindProperty(name);
        if (property != null)
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(property, new GUIContent(name), true);
            serializedObject.ApplyModifiedProperties();
            return true; // Successfully drawn using Unity's property drawer
        }

        return false; // No property drawer available
    }
}
*/