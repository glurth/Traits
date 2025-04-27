// Path: Editor/TraitEffectDrawer.cs
using UnityEngine;
using UnityEditor;
using EyE.Traits;
[CustomPropertyDrawer(typeof(TraitEffect))]
public class TraitEffectPropertyDrawer : PropertyDrawer
{
    private static readonly int[] Ids;
    private static readonly string[] TypeNames;

    static TraitEffectPropertyDrawer()
    {
        var idList = new System.Collections.Generic.List<int>();
        var nameList = new System.Collections.Generic.List<string>();

        foreach (var kvp in BuffArithmetic.IdRegistry)
        {
            idList.Add(kvp.Key);
            nameList.Add(kvp.Value.GetType().Name);
        }

        Ids = idList.ToArray();
        TypeNames = nameList.ToArray();
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty effectAmountProp = property.FindPropertyRelative("effectAmount");
        SerializedProperty encodedBuffArithmeticProp = property.FindPropertyRelative("encodedBuffArithmetic");

        Rect amountRect = new Rect(position.x, position.y, position.width * 0.5f, position.height);
        Rect typeRect = new Rect(position.x + position.width * 0.5f + 5, position.y, position.width * 0.5f - 5, position.height);

        EditorGUI.PropertyField(amountRect, effectAmountProp, GUIContent.none);

        int currentIndex = System.Array.IndexOf(Ids, encodedBuffArithmeticProp.intValue);
        if (currentIndex < 0) currentIndex = 0;

        int newIndex = EditorGUI.Popup(typeRect, currentIndex, TypeNames);
        encodedBuffArithmeticProp.intValue = Ids[newIndex];

        EditorGUI.EndProperty();
    }
}
