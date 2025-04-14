using System.Collections.Generic;
using System;
using EyE.Collections;
using EyE.Collections.UnityAssetTables;
namespace EyE.Traits
{
    /// <summary>
    /// Abstract base class that defines the contract for arithmetic operations applied to trait buffs.
    /// Each arithmetic type (e.g., additive, multiplicative) implements how to aggregate multiple effects
    /// and how to compute the final modified value from the base trait value.
    /// 
    /// Responsibilities:
    /// - Defines the operation order for sequencing multiple arithmetic types.
    /// - Aggregates multiple buff effects of the same arithmetic type.
    /// - Computes the final affected trait value using the aggregated buff.
    /// 
    /// Usage:
    /// Concrete implementations (e.g., Additive, Multiplicative) specify exact behavior.
    /// During buff calculation, effects are grouped by their arithmetic type, aggregated,
    /// and then applied in operation order to the base value.
    /// 
    /// Equality:
    /// Two BuffArithmetic instances are considered equal if they are of the same type. HashCodes similarly use the type.
    /// This implies, and requires, that this class and it's variants does NOT contain state data.
    /// 
    /// Note:
    /// Instances are used as dictionary keys; ensure consistent instance handling to avoid key mismatches.
    /// Providing simple instance members for concrete descendants is recommended: e.g. ``public static Additive Instance { get; } = new Additive();``
    /// </summary>
    abstract public class BuffArithmetic
    {
        /// <summary>
        /// Aggregates and applies all trait effects to a base value using the defined BuffArithmetic strategies.
        /// 
        /// Process:
        /// 1. Groups all trait effects by their BuffArithmetic type.
        /// 2. Aggregates the values within each group using the type-specific aggregation logic.
        /// 3. Applies each aggregated result to the base value, following the BuffArithmetic operation order.
        /// 
        /// Notes:
        /// - BuffArithmetic instances define their operation order via the OperationOrder property.
        /// - Proper ordering ensures that multiplicative effects are applied before additive, or as configured.
        /// - Grouping effects by arithmetic type optimizes aggregation and reduces redundant calculations.
        /// </summary>
        /// <param name="baseValue">The original value of the trait before any effects are applied.</param>
        /// <param name="buffValues">The list of trait effects to apply, each specifying a value and arithmetic type.</param>
        /// <returns>The final modified value of the trait after all effects are applied.</returns>
        static public float AggregateAllEffects(float baseValue, List<TraitEffect> buffValues)
        {
            //first we take the input list of effects, and break it up by the BuffArithmetic they use
            Dictionary<BuffArithmetic, List<TraitEffect>> effectsByArithType = new Dictionary<BuffArithmetic, List<TraitEffect>>();
            foreach (TraitEffect buff in buffValues)
            {
                if (!effectsByArithType.ContainsKey(buff.typeOfArithmetic))  //if this BuffArithmetic is not already a key in the dictionary, add it, with a new list.
                    effectsByArithType.Add(buff.typeOfArithmetic, new List<TraitEffect>());

                effectsByArithType[buff.typeOfArithmetic].Add(buff);
            }

            //Compute the Aggregate of all the modifier values, for each arithmatic type
            Dictionary<BuffArithmetic, float> aggregateBuffValueByArithType = new Dictionary<BuffArithmetic, float>();
            foreach (BuffArithmetic t in effectsByArithType.Keys)
            {
                aggregateBuffValueByArithType[t] = t.Aggregate(effectsByArithType[t]);
            }

            // now we need to apply each computation in the appropriate order, using the result of each step as the input for the next
            List<BuffArithmetic> opOrder = new List<BuffArithmetic>(effectsByArithType.Keys);
            opOrder.Sort((a, b) => { return a.OperationOrder - b.OperationOrder; });
            float value = baseValue;
            foreach (BuffArithmetic t in opOrder)
            {
                value = t.ComputeAffectedValue(value, aggregateBuffValueByArithType[t]);
            }
            return value;
        }

        /// <summary>
        /// descendant classes must define an int value that specifies their order when processing operations
        /// for example- 
        /// if we want it to be ( base * mulModifier) + addModifier :  we would ensure that mulModifier has a lower OperationOrder than addModifier does.
        /// if we want it to be ( base + addModifier) * mulModifier :  we would ensure that addModifier has a lower OperationOrder than mulModifier does.
        /// It is recommend you separate these by about 100, to make room if you later (or mods) want to fit any in between existing ones.
        /// </summary>
        public abstract int OperationOrder { get; }
        /// <summary>
        /// Overridden by descendants to do the math to apply a buff to a value 
        /// </summary>
        /// <param name="baseValue"></param>
        /// <param name="buffValue"></param>
        /// <returns></returns>
        public abstract float ComputeAffectedValue(float baseValue, float buffValue);
        /// <summary>
        /// Overridden by descendants to specify how a set of TraitEffects should be aggregated together into a single value.  
        /// Usually this is a simple summation, but not necessarily.
        /// </summary>
        /// <param name="buffValues">the set of TraitEffects to be aggregated together</param>
        /// <returns>the computed, aggregated value</returns>
        public abstract float Aggregate(List<TraitEffect> buffValues);

        /// <summary>
        /// If the type of the passed object is equal to this type, we declare them as equal.  
        /// This works because instances contain no state data, only Method members.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            return obj.GetType() == GetType();
        }
        /// <summary>
        /// Provides the HashCode of the CurrentType. 
        /// This works because instances contain no state data, only Method members.
        /// </summary>
        /// <returns>HashCode of instance's class type</returns>
        public override int GetHashCode()
        {
            return GetType().GetHashCode();
        }
    }

    /// <summary>
    /// Additive BuffArithmetic adds the buffValue to the baseTraitValue.
    /// </summary>
    [Serializable]
    public class Additive : BuffArithmetic
    {
        public static Additive Instance { get; } = new Additive();
        public override int OperationOrder { get => 200; }
        public override float ComputeAffectedValue(float baseValue, float buffValue)
        {
            return baseValue + buffValue;
        }
        public override float Aggregate(List<TraitEffect> buffValues)
        {
            float sum = 0;
            foreach (TraitEffect buff in buffValues)
                sum += buff.effectAmount;
            return sum;
        }
    }

    /// <summary>
    /// Multiplicative BuffArithmetic multiplies (buffValue+1.0) by the baseTraitValue.
    /// e.g.
    /// -100% malus is specified  as -1, this should yield a buffed value of 0
    /// 0% bonus is specified  as 0, this should yield a buffed value of baseTraitValue
    /// +50% bonus is specified  as 0.5, this should yield a buffed value of (baseTraitValue*1.5)
    /// +100% bonus is specified  as 0.5, this should yield a buffed value of (baseTraitValue*2)
    /// for consistency:
    /// -200% malus is specified  as -2, this should yield a buffed value of (baseTraitValue* -1)-  the buff is NOT responsible for clipping the output.
    /// </summary>
    [Serializable]
    public class Multiplicative : BuffArithmetic
    {
        public static Multiplicative Instance { get; } = new Multiplicative();

        public override int OperationOrder { get => 100; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseValue"></param>
        /// <param name="buffValue"></param>
        /// <returns></returns>
        public override float ComputeAffectedValue(float baseValue, float buffValue)
        {
            return baseValue * (buffValue + 1f);
        }
        /// <summary>
        /// simple summation
        /// </summary>
        /// <param name="buffValues"></param>
        /// <returns></returns>
        public override float Aggregate(List<TraitEffect> buffValues)
        {
            float sum = 0;
            foreach (TraitEffect buff in buffValues)
                sum += buff.effectAmount;
            return sum;
        }
    }


    /// <summary>
    /// Represents an effect that the buff has on a specific trait, consisting of a value and an arithmetic type.
    /// </summary>
    [Serializable]
    public struct TraitEffect
    {
        /// <summary>
        /// The value of the buff effect to apply to the trait.
        /// </summary>
        public float effectAmount;
        
        /// <summary>
        /// The arithmetic method to apply the buff (additive, multiplicative, or percentage).
        /// </summary>
        public BuffArithmetic typeOfArithmetic;

        /// <summary>
        /// Initializes a new instance of the <see cref="TraitEffect"/> struct.
        /// </summary>
        /// <param name="value">The numeric value of the effect.</param>
        /// <param name="type">The arithmetic type defining how the value modifies the trait.</param>
        public TraitEffect(float value, BuffArithmetic type)
        {
            effectAmount = value;
            typeOfArithmetic = type;
        }

        /// <summary>
        /// Implicit conversion from a tuple to a <see cref="TraitEffect"/>. 
        /// This allows creating a <see cref="TraitEffect"/> from a tuple (value, type).
        /// </summary>
        /// <param name="tuple">A tuple containing the value and type of the trait effect.</param>
        public static implicit operator TraitEffect((float value, BuffArithmetic type) tuple)
        {
            return new TraitEffect(tuple.value, tuple.type);
        }
    }

    /// <summary>
    /// For each trait affected, a set of modifiers
    /// </summary>
    [System.Serializable]
    public class TraitEffects : SerializableDictionary<TraitDefinition, List<TraitEffect>>
    {
        public TraitEffects() { }
        public TraitEffects(SerializableDictionary<TraitDefinition, List<TraitEffect>> toCopy) : base(toCopy) { }
        public TraitEffects(int len = 0) : base(len) { }
    }

    /// <summary>
    /// Represents a type of buff or debuff that can modify specific traits of an entity.
    /// Buffs can either be temporary (with a duration) or permanent.
    /// </summary>
    [Serializable]
    public class BuffDefinition : TableElement
    {

        /// <summary>
        /// Gets the UI sprite of this trait.
        /// </summary>
        public UnityEngine.Sprite Sprite;

        /// <summary>
        /// A dictionary mapping each affected <see cref="TraitDefinition"/> to its respective <see cref="TraitEffect"/>.
        /// This defines how the buff modifies various traits. It may have only one modifier per trait.
        /// </summary>
        public TraitEffects Effects;


        /// <summary>
        /// The duration for which the buff is active. A null value indicates a permanent buff.
        /// </summary>
        public SerializableTimeSpan Duration;


        /// <summary>
        /// Initializes a new instance of the <see cref="BuffDefinition"/> class.
        /// </summary>
        /// <param name="name">The name of the buff type.</param>
        /// <param name="duration">The duration of the buff. If null, the buff is considered permanent.</param>
        /// <param name="modifiers">An optional dictionary of trait modifiers to initialize the buff with.</param>
        public BuffDefinition(string name, TimeSpan? duration = null, TraitEffects modifiers = null)
        {
            Name = name;
            //  id = name.GenerateConsistentID();
            Effects = modifiers ?? new TraitEffects();
            Duration = duration;
        }

    }
}