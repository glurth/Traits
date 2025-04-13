
// File: Editor/IRefDrawer.cs

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using EyE.Collections.UnityAssetTables;
using System;
using System.Collections;
using System.Reflection;



// Base Drawer for IRef Implementations
[CustomPropertyDrawer(typeof(TableElement),true)]
public class TableElementRefDrawer : PropertyDrawer
{
    //protected abstract Dictionary<long, string> GetAllItems();
    protected Dictionary<long, string> GetAllItems(System.Type tableElementType)
    {
        var result = new Dictionary<long, string>();
        ImmutableTableBase table = TablesByElementType.GetTable(tableElementType);
        //ImmutableTable<MeasurementUnit> table = TablesByElementType.GetTable<MeasurementUnit>();
        if (table != null)
        {
            foreach (TableElement element in table)
            {
                result[element.ID] = element.Name;
            }
        }
        return result;
    }
    TableElement GetTableElement(long id, System.Type tableElementType)
    {
        ImmutableTableBase table = TablesByElementType.GetTable(tableElementType);
        if (table.TryGetElement(id, out TableElement element))
            return element;
        return null;
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        
        // EyE.EditorUnity.Extensions.SerializedPropertyExtensions.SetValue(property, null);
        //Debug.Log("running TableElement property drawer.  PropertyPath: "+ property.propertyPath);
        // Get the FieldInfo or PropertyInfo from the SerializedProperty
        //string[] propertyPath = property.propertyPath.Split('.');
        Type targetObjectType = property.serializedObject.targetObject.GetType();
        FieldInfo fieldInfo = EyE.EditorUnity.Extensions.SerializedPropertyExtensions.GetFieldViaPath(targetObjectType,property.propertyPath); // GetField(property);//  targetObjectType.GetField(propertyPath[0], BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        // Check if the field has the DrawFullAttribute
        bool attributeOnPropertyMember = fieldInfo != null && fieldInfo.GetCustomAttribute(typeof(DrawElementDataAttribute)) != null;
        if (attributeOnPropertyMember)
        {
           // Debug.Log("Property: " + property.propertyPath + " has DrawElementDataAttribute set.");
            DrawDefaultSerializedProperty(property);
            return;
        }

        EditorGUI.BeginProperty(position, label, property);
        
        SerializedProperty idProperty = property.FindPropertyRelative("id");
        if (idProperty == null)
        {
            EditorGUI.LabelField(position, "Error: Missing ID property");
            EditorGUI.EndProperty();
            return;
        }

        long currentID = idProperty.longValue;
        Debug.Log("Reference Dropdown start - currentID: " + idProperty.longValue);
        // Fetch all available items
        Dictionary<long, string> allItems = GetAllItems(fieldInfo.FieldType);//  property.serializedObject.targetObject.GetType());

        // Prepare dropdown options
        List<long> ids = new List<long>(allItems.Keys);
        List<string> displayNames = new List<string>();
        int selectedIndex = -1;

        for (int i = 0; i < ids.Count; i++)
        {
            displayNames.Add(allItems[ids[i]]);
            if (ids[i] == currentID)
                selectedIndex = i;
        }

        // Dropdown selection
        int newIndex = EditorGUI.Popup(position, label.text, selectedIndex, displayNames.ToArray());
        
        if (newIndex >= 0 && newIndex < ids.Count && newIndex!=selectedIndex)
        {
            Debug.Log("New index selected: "+newIndex+ "  nw id: "+ ids[newIndex]);
            //property.managedReferenceValue = GetTableElement(ids[newIndex], fieldInfo.FieldType);
            //EyE.EditorUnity.Extensions.SerializedPropertyExtensions.SetValue(property, GetTableElement(ids[newIndex], fieldInfo.FieldType));
            
            //idProperty.longValue = ids[newIndex];
        }
        Debug.Log("Reference Dropdown ["+idProperty.propertyPath+"] end - currentID: " + idProperty.longValue);
        EditorGUI.EndProperty();
    }

    private void DrawDefaultSerializedProperty(SerializedProperty property)
    {
        //Debug.Log("DefDrawer START Property: " + property.propertyPath);
        // Draw each field of the serialized object manually to avoid custom drawers
        SerializedProperty currentProperty = property.Copy();
        SerializedProperty endProperty = property.GetEndProperty();
        int initialDepth = currentProperty.depth;
        bool descend = true;
        while (currentProperty.NextVisible(descend) && !SerializedProperty.EqualContents(currentProperty, endProperty))
        {
            if(descend)
                initialDepth = currentProperty.depth;
            descend = false;
            //Debug.Log("DefDrawer Property: " + currentProperty.propertyPath);
            if (currentProperty.depth==initialDepth && currentProperty.name != "id")
                EditorGUILayout.PropertyField(currentProperty, true);
        }
    }
    private static FieldInfo GetField(SerializedProperty property)
    {
        string[] propertyPath = property.propertyPath.Split('.');
        Type targetObjectType = property.serializedObject.targetObject.GetType();
        Debug.Log(property.propertyPath + "- target Object type: " + targetObjectType);
        // Start from the last element of the propertyPath and check if it's an array or list
        string fieldName;
        if (propertyPath.Length > 2 && propertyPath[propertyPath.Length - 2] == "Array")
            fieldName = propertyPath[propertyPath.Length - 3];
        else
            fieldName = propertyPath[propertyPath.Length - 1];
        return targetObjectType.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        
    }
}
/*
// Derived Drawer for MeasurementUnitRef
[CustomPropertyDrawer(typeof(MeasurementUnit))]
public class MeasurementUnitRefDrawer : TableElementRefDrawer
{
    protected override Dictionary<long, string> GetAllItems()
    {
        var result = new Dictionary<long, string>();
        ImmutableTable<MeasurementUnit> table = TablesByElementType.GetTable<MeasurementUnit>();
        if (table != null)
        {
            foreach (MeasurementUnit element in table)
            {
                result[element.ID] = element.Name;
            }
        }
        return result;
    }
}

// Derived Drawer for TraitRef
[CustomPropertyDrawer(typeof(TraitRef))]
public class TraitRefDrawer : TableElementRefDrawer
{
    protected override Dictionary<long, string> GetAllItems()
    {
        var result = new Dictionary<long, string>();
        if (AllTraits.All != null)
        {
            foreach (var kvp in AllTraits.All)
            {
                result[kvp.Key] = kvp.Value.Name;
            }
        }
        return result;
    }
}

// Derived Drawer for BuffRef
[CustomPropertyDrawer(typeof(BuffRef))]
public class BuffRefDrawer : TableElementRefDrawer
{
    protected override Dictionary<long, string> GetAllItems()
    {
        var result = new Dictionary<long, string>();
        if (AllBuffs.All != null)
        {
            foreach (var kvp in AllBuffs.All)
            {
                result[kvp.Key] = kvp.Value.Name;
            }
        }
        return result;
    }
}

*/