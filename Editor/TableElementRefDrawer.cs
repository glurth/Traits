using UnityEditor;
using UnityEngine;
using System;
using EyE.Collections.UnityAssetTables;



[CustomPropertyDrawer(typeof(TableElementRef<>), true)]
public class TableElementRefDrawer : PropertyDrawer
{
    Type[] cachedGenericArgs = null;


    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var idProperty = property.FindPropertyRelative("ID");
        if (cachedGenericArgs == null)
        {
            cachedGenericArgs = GetGenericArgumentsIfDerivedFrom(fieldInfo.FieldType, typeof(TableElementRef<>));
            if (cachedGenericArgs.Length == 0)
            {
                throw new InvalidOperationException($"TableElementRefDrawer error:  Type {fieldInfo.FieldType} is not derived from TableElementRef<>.");
            }
        }
        Type elementType;
        if (cachedGenericArgs.Length > 0)
            elementType = cachedGenericArgs[0];
        else
            throw new InvalidOperationException($"TableElementRefDrawer error:  Type {fieldInfo.FieldType} is not derived from TableElementRef<>.");

      //  Debug.Log("element type" + elementType);
        ImmutableTableBase table = TablesByElementType.GetTable(elementType);

        if (table != null)
        {
            string[] elementNames;
            long[] elementIDs;
            int selectedIndex = -1;

            var list = new System.Collections.Generic.List<TableElement>(table);
            elementNames = new string[list.Count];
            elementIDs = new long[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                elementNames[i] = list[i].Name;
                elementIDs[i] = list[i].ID;
                if (list[i].ID == idProperty.longValue)
                    selectedIndex = i;
            }
            if (selectedIndex == -1)
                Debug.Log("Didn't find id " + idProperty.longValue + " in table: "+ table.name);
            int newIndex = EditorGUI.Popup(position, label.text, selectedIndex, elementNames);
            if (newIndex != selectedIndex && newIndex >= 0 && newIndex < elementIDs.Length)
            {
                idProperty.longValue = elementIDs[newIndex];
            }
        }
        else
        {
            EditorGUI.LabelField(position, label.text, "[Missing Table for " + elementType.Name + "]");
        }

        EditorGUI.EndProperty();
    }



    /// <summary>
    /// Checks whether a given derived type inherits from a specified generic base type definition.
    /// If so, returns the concrete generic type arguments used in the inheritance.
    /// </summary>
    /// <param name="derived">The type to check for inheritance.</param>
    /// <param name="baseGeneric">The generic base type definition (e.g., typeof(Base&lt;&gt;)).</param>
    /// <returns>
    /// An array of concrete types used in the generic base, or null if not derived from the base.
    /// </returns>
    public static Type[] GetGenericArgumentsIfDerivedFrom(Type derived, Type baseGeneric)
    {
        if (!baseGeneric.IsGenericTypeDefinition)
            throw new ArgumentException("baseGeneric must be a generic type definition", nameof(baseGeneric));

        Type current = derived;
        if (baseGeneric != typeof(System.Collections.Generic.List<>))
        {
            Type[] listTypes = GetGenericArgumentsIfDerivedFrom(current, typeof(System.Collections.Generic.List<>));
            if (listTypes != null && listTypes.Length>0)
                current = listTypes[0];
        }
        while (current != null && current != typeof(object))
        {
            if (current.IsGenericType && current.GetGenericTypeDefinition() == baseGeneric)
                return current.GetGenericArguments();

            current = current.BaseType;
        }

        return new Type[0];
    }
}
