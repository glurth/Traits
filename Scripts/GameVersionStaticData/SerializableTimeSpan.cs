using System;
using System.IO;
using UnityEngine;

[Serializable]
public class SerializableTimeSpan : ISerializationCallbackReceiver,IBinarySaveLoad
{
    [SerializeField] private long ticks;

    private TimeSpan timeSpan;

    public SerializableTimeSpan(TimeSpan timeSpan)
    {
        this.timeSpan = timeSpan;
        ticks = timeSpan.Ticks;
    }

    public TimeSpan TimeSpan => timeSpan;

    // Unity Serialization Callbacks
    public void OnBeforeSerialize()
    {
        ticks = timeSpan.Ticks;
    }

    public void OnAfterDeserialize()
    {
        timeSpan = new TimeSpan(ticks);
    }

    // Implicit Conversion
    public static implicit operator TimeSpan(SerializableTimeSpan sts) => sts.timeSpan;
    public static implicit operator SerializableTimeSpan(TimeSpan ts) => new SerializableTimeSpan(ts);

    // Operators
    public static SerializableTimeSpan operator +(SerializableTimeSpan a, SerializableTimeSpan b)
    {
        return new SerializableTimeSpan(a.timeSpan + b.timeSpan);
    }

    public static SerializableTimeSpan operator -(SerializableTimeSpan a, SerializableTimeSpan b)
    {
        return new SerializableTimeSpan(a.timeSpan - b.timeSpan);
    }

    public static SerializableTimeSpan operator *(SerializableTimeSpan a, double multiplier)
    {
        return new SerializableTimeSpan(TimeSpan.FromTicks((long)(a.timeSpan.Ticks * multiplier)));
    }

    public static SerializableTimeSpan operator /(SerializableTimeSpan a, double divisor)
    {
        return new SerializableTimeSpan(TimeSpan.FromTicks((long)(a.timeSpan.Ticks / divisor)));
    }

    public override string ToString() => timeSpan.ToString();

    public void Serialize(BinaryWriter writer)
    {
        ticks = timeSpan.Ticks;
        ticks.Serialize(writer);
    }

    public void Deserialize(BinaryReader reader)
    {
        ticks = reader.DeserializeLong();
        timeSpan = new TimeSpan(ticks);
    }
}
