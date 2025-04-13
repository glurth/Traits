using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
/*
public class GameDataEditor : EditorWindow
{
    private GameVersionData gameData;

    [MenuItem("Window/Game Data Editor")]
    public static void ShowWindow()
    {
        GetWindow<GameDataEditor>("Game Data Editor");
    }

    private void OnEnable()
    {
        // Load data when the editor window is opened
        gameData = GameVersionData.LoadData();
    }

    private void OnGUI()
    {
        GUILayout.Label("Game Data Editor", EditorStyles.boldLabel);

        // Add Trait Definitions Section
        if (GUILayout.Button("Add Trait Definition"))
        {
            gameData.Traits.Add(new TraitDefinition(0, "New Trait", ValueType.Number, PositiveValueImpact.Positive, "Units", 0));
        }

        for (int i = 0; i < gameData.Traits.Count; i++)
        {
            GUILayout.Label($"Trait {i + 1}");
            TraitDefinition trait = gameData.Traits[i];
            trait.Name = EditorGUILayout.TextField("Name", trait.Name);
            trait.SuperDefaultNumericValue = EditorGUILayout.DoubleField("Default Value", trait.SuperDefaultNumericValue);
            trait.Units = EditorGUILayout.TextField("Units", trait.Units);
            GUILayout.Space(10);

            if (GUILayout.Button("Remove Trait"))
            {
                gameData.Traits.RemoveAt(i);
            }
        }

        GUILayout.Space(20);

        // Add Entity Types Section
        if (GUILayout.Button("Add Entity Type"))
        {
            gameData.EntityTypes.Add(new EntityType("New Entity Type"));
        }

        for (int i = 0; i < gameData.EntityTypes.Count; i++)
        {
            GUILayout.Label($"Entity {i + 1}");
            var entity = gameData.EntityTypes[i];
            entity.Name = EditorGUILayout.TextField("Name", entity.Name);

            GUILayout.Label("Default Trait Values");
            foreach (var trait in gameData.Traits)
            {
                if (entity.DefaultTraitValues.ContainsKey(trait))
                {
                    entity.DefaultTraitValues[trait] = EditorGUILayout.DoubleField($"{trait.Name} Default Value", entity.DefaultTraitValues[trait]);
                }
                else
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label($"Add {trait.Name}?");
                    if (GUILayout.Button("Add"))
                    {
                        entity.DefaultTraitValues[trait] = trait.SuperDefaultNumericValue;
                    }
                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Remove Entity"))
            {
                gameData.EntityTypes.RemoveAt(i);
            }
        }

        GUILayout.Space(20);

        // Add Buff Types Section
        if (GUILayout.Button("Add Buff Type"))
        {
            gameData.BuffTypes.Add(new BuffType("New Buff", null));
        }

        for (int i = 0; i < gameData.BuffTypes.Count; i++)
        {
            GUILayout.Label($"Buff {i + 1}");
            var buff = gameData.BuffTypes[i];
            buff.Name = EditorGUILayout.TextField("Name", buff.Name);
            buff.Duration = EditorGUILayout.FloatField("Duration (0 or negative for permanent)", buff.Duration ?? -1f);

            GUILayout.Space(10);

            if (GUILayout.Button("Remove Buff"))
            {
                gameData.BuffTypes.RemoveAt(i);
            }
        }

        GUILayout.Space(20);

        // Save Button
        if (GUILayout.Button("Save Data"))
        {
            gameData.SaveData();
        }
    }
}
*/