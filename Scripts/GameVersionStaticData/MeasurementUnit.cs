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

    public MeasurementUnit(string longName, string shortName, Sprite icon = null)
    {
        Name = longName;
        ShortName = shortName;
        Icon = icon;
      //  ID = IDFromName.GenerateConsistentID(LongName);
    }

    /*
    public override void Serialize(System.IO.BinaryWriter writer)
    {
        writer.Write(ID);
    }
    public override MeasurementUnit Deserialize(System.IO.BinaryReader reader)
    {
        long id=reader.ReadInt64();
        if (AllMeasurementUnits.TryGet(id, out MeasurementUnit val))
            return val;
        throw new System.IO.InvalidDataException("Unable to find MeasurementUnit ID["+id.ToString("X") + "] in system."); 
    }*/
}


