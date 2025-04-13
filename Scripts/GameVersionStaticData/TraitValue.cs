using System.IO;
using System.Runtime.Serialization;
namespace EyE.Traits
{
    /// <summary>
    /// Represents an instance of a trait with a specific numeric value. 
    /// Each <see cref="TraitValue"/> is associated with a <see cref="TraitDefinition"/>, which defines the trait's properties.
    /// </summary>
    [System.Serializable]
    public class TraitValue : IBinarySaveLoad
    {

        private TraitDefinition _trait;
        /// <summary>
        /// The definition of the trait this instance is based on.
        /// This provides context such as the trait's name, type, and impact.
        /// </summary>
        public TraitDefinition Trait { get => _trait; private set => _trait = value; }

        /// <summary>
        /// The numeric value associated with this instance of the trait.
        /// This can be updated to reflect changes in the value of the trait.
        /// </summary>
        public float NumericValue { get; set; }


        public TraitValue() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="TraitValue"/> class with a specified trait definition and numeric value.
        /// </summary>
        /// <param name="identifier">The <see cref="TraitDefinition"/> that defines the properties of this trait.</param>
        /// <param name="baseValue">The initial numeric value for this trait instance.</param>
        public TraitValue(TraitDefinition identifier, float baseValue)
        {
            Trait = identifier;
            NumericValue = baseValue;
        }
        public static implicit operator TraitValue((TraitDefinition identifier, float baseValue) tuple)
        {
            return new TraitValue(tuple.identifier, tuple.baseValue);
        }
        public void Serialize(BinaryWriter writer)
        {
            Trait.Serialize(writer);
            NumericValue.Serialize(writer);
        }

        public void Deserialize(BinaryReader reader)
        {
            Trait = EyE.Collections.UnityAssetTables.TablesByElementType.DeserializeTableElement<TraitDefinition>(reader);
            //reader.DeserializeBinary<TraitDefinition>();
            NumericValue = reader.DeserializeFloat();
        }
    }

}