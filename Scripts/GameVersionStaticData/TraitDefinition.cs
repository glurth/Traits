using System.Runtime.Serialization;
using EyE.Collections.UnityAssetTables;

namespace EyE.Traits
{
    /// <summary>
    /// Specifies the type of value that a trait can hold.
    /// </summary>
    [DataContract]
    public enum ValueType
    {
        /// <summary>
        /// The value is a number (e.g., integer, float, etc.).
        /// </summary>
        Number,

        /// <summary>
        /// The value is a percentage.
        /// </summary>
        Percentage,

        /// <summary>
        /// The value represents a counter or countable quantity.
        /// </summary>
        Counter
    }

    /// <summary>
    /// Defines the impact a trait can have, which could be positive, negative, or neutral.
    /// </summary>
    [DataContract]
    public enum PositiveValueImpact
    {
        /// <summary>
        /// The trait has a positive impact.
        /// </summary>
        Positive,

        /// <summary>
        /// The trait has a negative impact.
        /// </summary>
        Negative,

        /// <summary>
        /// The trait has a neutral impact.
        /// </summary>
        Neutral
    }

    /// <summary>
    /// Represents a definition of a trait that includes its identifier, name, value type, impact, unit, 
    /// and a default numeric value. This class ensures that trait IDs are unique across instances.
    /// </summary>
    [System.Serializable]
    public class TraitDefinition : TableElement//NameBasedID
    {

        /// <summary>
        /// Gets the UI sprite of this trait.
        /// </summary>
        public UnityEngine.Sprite Sprite;

        /// <summary>
        /// Gets the type of value that this trait holds (e.g., Number, Percentage, or Counter).
        /// </summary>
        public ValueType Type;

        /// <summary>
        /// Gets the impact of this trait, which can be Positive, Negative, or Neutral.
        /// </summary>
        public PositiveValueImpact Impact;

        /// <summary>
        /// Gets the unit of measurement for this trait (e.g., "kg", "%", etc.).
        /// </summary>
        public MeasurementUnit Unit;

        /// <summary>
        /// Gets the default numeric value for this trait when it is initially defined.
        /// </summary>
        public double SuperDefaultNumericValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="TraitDefinition"/> class with the specified parameters.
        /// </summary>
        /// <param name="identifier">The unique identifier for this trait.</param>
        /// <param name="name">The name of the trait.</param>
        /// <param name="type">The type of value the trait holds (e.g., Number, Percentage, Counter).</param>
        /// <param name="impact">The impact of the trait (Positive, Negative, or Neutral).</param>
        /// <param name="unit">The unit of measurement for the trait.</param>
        /// <param name="superDefaultNumericValue">The default numeric value of the trait.</param>
        /// <exception cref="System.Exception">
        /// Thrown when attempting to create a trait with a duplicate identifier.
        /// </exception>
        public TraitDefinition(string name, ValueType type, PositiveValueImpact impact, MeasurementUnit unit, double superDefaultNumericValue)
        {
            //_ID = name.GenerateConsistentID();
            Name = name;
            Type = type;
            Impact = impact;
            Unit = unit;
            SuperDefaultNumericValue = superDefaultNumericValue;
        }
    }
}