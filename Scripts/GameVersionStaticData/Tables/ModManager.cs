using System.Collections.Generic;
using UnityEngine;
using EyE.Collections.UnityAssetTables;

public class ModManager
{
    const string tableModPath="/mods/tables";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void RegisterModImmunitables()
    {
        ImmutableTableBase[] allTableMods = Resources.LoadAll<ImmutableTableBase>(tableModPath);
        foreach (ImmutableTableBase tableBase in allTableMods)
        {
            tableBase.AddAsModTable();
        }
    }

}
