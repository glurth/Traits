using System.Collections.Generic;
using System.IO;
using EyE.Collections;
namespace EyE.Traits
{
    [System.Serializable]
    public class TraitValues : SerializableDictionary<TraitDefinition, TraitValue>, IBinarySaveLoad
    {
        public TraitValues() : base() { }
        public TraitValues(params TraitValue[] traits) : base()
        {
            foreach (TraitValue tv in traits)
                this.Add(tv.Trait, tv);
        }
        public TraitValues(params (TraitDefinition identifier, float baseValue)[] traits) : base()
        {
            foreach ((TraitDefinition identifier, float baseValue) tv in traits)
                this.Add(tv.identifier, (TraitValue)tv);
        }
        public TraitValues(TraitValues toCopy) : base(toCopy)
        {

        }
        public TraitValues(List<TraitValue> traits) : base()
        {
            foreach (TraitValue tv in traits)
                this.Add(tv.Trait, tv);
        }
        public void Deserialize(BinaryReader reader)
        {
            this.Clear();
            List<TraitValue> allValues = new List<TraitValue>();
            BinarySerializeExtension.DeserializeList(allValues, reader);
            foreach (TraitValue val in allValues)
                base.Add(val.Trait, val);
            //BinarySerializeExtension.DeserializeDictionary<TraitDefinition, TraitValue>(this,reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            List<TraitValue> allValues = new List<TraitValue>(this.Values);
            BinarySerializeExtension.SerializeList(allValues, writer);

        }
    }
}