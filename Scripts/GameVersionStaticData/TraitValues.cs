using System.Collections.Generic;
using System.IO;
using EyE.Collections;
namespace EyE.Traits
{
    [System.Serializable]
    public class TraitValues : SerializableDictionary<TraitDefinitionRef, float>, IBinarySaveLoad
    {
        public TraitValues() : base() { }
        public TraitValues(params TraitValue[] traits) : base()
        {
            foreach (TraitValue tv in traits)
                this.Add(tv.Trait, tv.NumericValue);
            OnBeforeSerialize();
        }
        public List<TraitValue> ToTraitValueList()
        {
            List<TraitValue> allValues = new List<TraitValue>();
            foreach (var val in this)
                allValues.Add(new TraitValue(val.Key, val.Value));
            return allValues;
        }
        public TraitValues(params (TraitDefinition identifier, float baseValue)[] traits) : base()
        {
            foreach ((TraitDefinition identifier, float baseValue) tv in traits)
                this.Add((TraitDefinitionRef)tv.identifier, tv.baseValue);
            OnBeforeSerialize();
        }
        public TraitValues(TraitValues toCopy) : base(toCopy)
        { }
        public TraitValues(List<TraitValue> traits) : base()
        {
            foreach (TraitValue tv in traits)
                this.Add(tv.Trait, tv.NumericValue);
            OnBeforeSerialize();
        }
        public void Deserialize(BinaryReader reader)
        {
            this.Clear();
            List<TraitValue> allValues = new List<TraitValue>();
            BinarySerializeExtension.DeserializeList(allValues, reader);
            foreach (TraitValue val in allValues)
                base.Add(val.Trait, val.NumericValue);
            //BinarySerializeExtension.DeserializeDictionary<TraitDefinition, TraitValue>(this,reader);
        }


        public void Serialize(BinaryWriter writer)
        {
            List<TraitValue> allValues = ToTraitValueList();
            BinarySerializeExtension.SerializeList(allValues, writer);
        }
    }
}