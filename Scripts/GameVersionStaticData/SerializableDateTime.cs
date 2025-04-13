using System;
using System.IO;
using UnityEngine;
/// <summary>
/// saved both in scriptable objects, and runtime state data and so implements both serialization interfaces
/// </summary>
[Serializable]
public class SerializableDateTime : ISerializationCallbackReceiver, IBinarySaveLoad
{
    [SerializeField] private long ticks;

    private DateTime dateTime;
    public SerializableDateTime() : base() { }
    public SerializableDateTime(DateTime dateTime)
    {
        this.dateTime = dateTime;
        ticks = dateTime.Ticks;
    }

    public DateTime DateTime => dateTime;

    // Unity Serialization Callbacks
    public void OnBeforeSerialize()
    {
        ticks = dateTime.Ticks;
    }

    public void OnAfterDeserialize()
    {
        dateTime = new DateTime(ticks);
    }

    // Implicit Conversion
    public static implicit operator DateTime(SerializableDateTime sdt) => sdt.dateTime;
    public static implicit operator SerializableDateTime(DateTime dt) => new SerializableDateTime(dt);

    // Operators
    public static SerializableDateTime operator +(SerializableDateTime dt, SerializableTimeSpan ts)
    {
        return new SerializableDateTime(dt.dateTime + ts.TimeSpan);
    }

    public static SerializableDateTime operator -(SerializableDateTime dt, SerializableTimeSpan ts)
    {
        return new SerializableDateTime(dt.dateTime - ts.TimeSpan);
    }

    public static SerializableTimeSpan operator -(SerializableDateTime a, SerializableDateTime b)
    {
        return new SerializableTimeSpan(a.dateTime - b.dateTime);
    }

    public override string ToString() => dateTime.ToString();

    public void Serialize(BinaryWriter writer)
    {
        ticks = dateTime.Ticks;
        ticks.Serialize(writer);

    }

    public void Deserialize(BinaryReader reader)
    {
        ticks = reader.DeserializeLong();
        dateTime = new DateTime(ticks);
    }
}
