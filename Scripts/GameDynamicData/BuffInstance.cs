using System;
using System.Collections.Generic;
using System.IO;
namespace EyE.Traits
{
    /// <summary>
    /// Represents an instance of a specific buff applied to a game entity. 
    /// It tracks the buff's start time and manages the modification of trait values.
    /// </summary>
    public class BuffInstance : IBinarySaveLoad
    {
        /// <summary>
        /// The type of buff applied in this instance, including its effects and duration.
        /// </summary>
        public BuffDefinition TypeOfBuff { get => _TypeOfBuff; }
        private BuffDefinitionRef _TypeOfBuff;

        /// <summary>
        /// The time when the buff was applied to the entity.
        /// </summary>
        public SerializableDateTime StartTime { get; private set; }

        public BuffInstance() : base() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="BuffInstance"/> class.
        /// </summary>
        /// <param name="typeOfBuff">The type of buff being applied.</param>
        /// <param name="startTime">The time when the buff was applied.</param>
        public BuffInstance(BuffDefinition typeOfBuff, DateTime startTime)
        {
            _TypeOfBuff = (BuffDefinitionRef)typeOfBuff;
            StartTime = startTime;
        }

        /// <summary>
        /// Determines if the buff is still active at the given current time.
        /// </summary>
        /// <param name="currentTime">The current time to check against the buff's duration.</param>
        /// <returns>True if the buff is active, false if it has expired.</returns>
        public bool IsActive(DateTime currentTime)
        {
            // Buff is permanent if Duration is null
            if (TypeOfBuff.Duration == null)
                return true;

            // Return true if the buff is still within its active duration
            return (StartTime + TypeOfBuff.Duration) >= currentTime;
        }

        /// <summary>
        /// Calculates the modified <see cref="TraitValue"/> for a specific trait based on the buff's effects.
        /// </summary>
        /// <param name="trait">The trait to calculate the modified value for.</param>
        /// <param name="baseValue">The base trait value before applying the buff.</param>
        /// <param name="currentTime">The current time to check whether the buff is active.</param>
        /// <returns>A new <see cref="TraitValue"/> representing the modified value of the trait.</returns>
        public TraitValue CalculateModifiedValue(TraitDefinition trait, TraitValue baseValue, DateTime currentTime)
        {
            return new TraitValue(trait, CalculateModifiedNumericValue(trait, baseValue.NumericValue, currentTime));
        }

        /// <summary>
        /// Helper method that calculates the modified numeric value for a trait based on the buff's effects.
        /// </summary>
        /// <param name="trait">The trait to calculate the modified numeric value for.</param>
        /// <param name="baseNumericValue">The base numeric value of the trait.</param>
        /// <param name="currentTime">The current time to check whether the buff is active.</param>
        /// <returns>The modified numeric value of the trait after applying the buff.</returns>
        private float CalculateModifiedNumericValue(TraitDefinition trait, float baseNumericValue, DateTime currentTime)
        {
            // Return the base value if the buff is no longer active
            if (!IsActive(currentTime))
                return baseNumericValue;

            // Check if the buff affects the given trait
            if (TypeOfBuff.Effects.TryGetValue(trait, out TraitEffectList modifiers))
            {
                return BuffArithmetic.AggregateAllEffects(baseNumericValue, modifiers);
            }

            // If no modifier exists for the trait, return the base value
            return baseNumericValue;
        }

        public void Serialize(BinaryWriter writer)
        {
            _TypeOfBuff.Serialize(writer);
            StartTime.Serialize(writer);
        }

        public void Deserialize(BinaryReader reader)
        {
            _TypeOfBuff = (BuffDefinitionRef)reader.DeserializeTableElementRef<BuffDefinition>();
            StartTime = reader.DeserializeBinary<SerializableDateTime>();
        }
    }
}