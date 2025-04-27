using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EyE.Collections.UnityAssetTables;
using System;



[Serializable]
public class MeasurementUnit: TableElement//NameBasedID//IBinarySaveLoad<MeasurementUnit>, IHaveAnID
{


    [SerializeField] private string shortName;
    [SerializeField] private Sprite icon; // Optional icon reference

    public string ShortName
    {
        get => shortName;
        set => shortName = value;
    }

    public Sprite Icon
    {
        get => icon;
        set => icon = value;
    }

    public MeasurementUnit(string longName, string shortName, Sprite icon = null):base()
    {
        Name = longName;
        ShortName = shortName;
        Icon = icon;
    }

}
[System.Serializable]
public class MeasurementUnitRef : TableElementRef<MeasurementUnit>
{
    public MeasurementUnitRef(long id, MeasurementUnit reference = null) : base(id, reference)
    {
    }
}

