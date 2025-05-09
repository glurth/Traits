﻿using System.Collections.Generic;
using System;
using System.IO;
namespace EyE.Traits
{
    static public class GameStateDateTime
    {
        static DateTime _now= DateTime.Now;
        static public DateTime Now { get { return _now; } }
        static public void Tick()
        {
            _now += new TimeSpan(1, 0, 0, 0); ;
        }
    }
    /* look at interfce option- dont like need to cleanup cache in extension class- we leave as base class
    public interface IHaveBuffableTraits
    {
        /// <summary>
        /// Dictionary of trait definitions to their corresponding values for this entity.
        /// Initial values come from the EntityType Type
        /// </summary>
        public TraitValues baseTraits { get; }

        /// <summary>
        /// List of active buffs affecting the entity.
        /// </summary>
        public List<BuffInstance> appliedBuffs { get; }

        /// <summary>
        /// Gets the base value of a specific trait without applying buffs.
        /// </summary>
        /// <param name="whichTrait">The trait to retrieve the base value for.</param>
        /// <returns>The base <see cref="TraitValue"/> of the trait.</returns>
        public TraitValue GetTraitBaseValue(TraitDefinition whichTrait);

        /// <summary>
        /// Gets the buffed value of a trait, applying any active buffs to the base trait value.
        /// Removes expired/inactive buffs from the TraitsSet.
        /// </summary>
        /// <param name="whichTrait">The trait to retrieve the buffed value for.</param>
        /// <returns>The buffed <see cref="TraitValue"/> after applying all active buffs.</returns>
        public TraitValue GetBuffedTraitValue(TraitDefinition whichTrait, DateTime? asOf = null);

        /// <summary>
        /// Adds a new buff to the entity, applying its effects to the entity's traits.
        /// </summary>
        /// <param name="typeOfBuff">The type of buff being applied.</param>
        /// <param name="startTime">The time when the buff was applied.</param>
        public void AddBuff(BuffDefinition typeOfBuff, DateTime startTime);
    }
    public static class InterfceExtensions
    {
        struct LastComputedBuffValue
        {
            public DateTime lastComputed;
            public TraitValue value;
        }
        static Dictionary<IHaveBuffableTraits, Dictionary<TraitDefinition, LastComputedBuffValue>> lastComputedBuffedTraitValuesByEntity = new Dictionary<IHaveBuffableTraits, Dictionary<TraitDefinition, LastComputedBuffValue>>();
        static TimeSpan recomputeTick = new TimeSpan(0, 0, 0, 0, 20);//20 ms

        static public TraitValue GetBuffedTraitValue(this IHaveBuffableTraits entity, TraitDefinition whichTrait, DateTime? asOf = null)
        {
            // Retrieve the base trait value
            TraitValue baseValue = entity.GetTraitBaseValue(whichTrait);
            TraitValue buffedValue = baseValue;
            if (asOf == null)
                asOf = GameStateDateTime.Now;
            //see if we have this trait's value cached, and if so, how recently that was done.
            if (lastComputedBuffedTraitValuesByEntity[entity].TryGetValue(whichTrait, out LastComputedBuffValue cachedValue))
            {
                if (asOf - cachedValue.lastComputed < recomputeTick)
                    return cachedValue.value;
                //expired do recompute anyway
            }
            // Apply each active buff to the base value, or remove it if expired
            for (int i = 0; i < entity.appliedBuffs.Count; i++)
            {
                BuffInstance buff = entity.appliedBuffs[i];
                if (!buff.IsActive(asOf.Value))
                {
                    entity.appliedBuffs.RemoveAt(i);
                }
                else
                    buffedValue = buff.CalculateModifiedValue(whichTrait, buffedValue, asOf.Value);
            }
            //add or overwrite
            lastComputedBuffedTraitValuesByEntity[entity][whichTrait] = new LastComputedBuffValue() { lastComputed = asOf.Value, value = buffedValue };
            return buffedValue;
        }

        /// <summary>
        /// Adds a new buff to the entity, applying its effects to the entity's traits.
        /// </summary>
        /// <param name="typeOfBuff">The type of buff being applied.</param>
        /// <param name="startTime">The time when the buff was applied.</param>
        static public void AddBuff(this IHaveBuffableTraits entity, BuffDefinition typeOfBuff, DateTime startTime)
        {
            // Create a new BuffInstance
            BuffInstance buff = new BuffInstance(typeOfBuff, startTime);
            if(buff.IsActive(startTime))//sanity check
                entity.appliedBuffs.Add(buff);
        }
    }
    */
    /// <summary>
    /// Represents an object that has traits and can receive buffs/debuffs upon those traits.
    /// </summary>
    public class TraitsSet : IBinarySaveLoad
    {
        /// <summary>
        /// Unique identifier for the game entity.
        /// </summary>
        public long ID { get; set; }

        /// <summary>
        /// The type of the entity, which defines its default traits.
        /// </summary>
        public EntityTypeRef TypeOfEntityRef { get; private set; }

        public EntityType TypeOfEntity
        {
            get { return TypeOfEntityRef; }
        }
        /// <summary>
        /// Dictionary of trait definitions to their corresponding values for this entity.
        /// Initial values come from the EntityType Type
        /// </summary>
        public TraitValues Traits { get; private set; }

        /// <summary>
        /// List of active buffs affecting the entity.
        /// </summary>
        public List<BuffInstance> Buffs { get; private set; }

        public TraitsSet()
        {
            Traits = new TraitValues();
            Buffs = new List<BuffInstance>();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="GameEntity"/> class with the specified ID and type.
        /// </summary>
        /// <param name="id">The unique identifier for the entity.</param>
        /// <param name="type">The type of the entity, which defines its traits.</param>
        public TraitsSet(long id, EntityType type)
        {
            ID = id;
            TypeOfEntityRef = (EntityTypeRef)type;
            Traits = new TraitValues();
            Buffs = new List<BuffInstance>();
            InitializeTraits(); // Initialize traits based on the entity type
        }

        public TraitValue this[TraitDefinition i]
        {
            get
            {
                return new TraitValue(i,Traits[i]);
            }
        }

        /// <summary>
        /// Initializes the traits of the entity based on the default trait values from its type.
        /// </summary>
        private void InitializeTraits()
        {
            Traits = new TraitValues(((EntityType)TypeOfEntity).DefaultTraitValues);
        }


        struct LastComputedBuffValue
        {
            public DateTime lastComputed;
            public TraitValue value;
        }
        Dictionary<TraitDefinition, LastComputedBuffValue> lastComputedBuffedTraitValues= new Dictionary<TraitDefinition, LastComputedBuffValue>();
        TimeSpan recomputeTick = new TimeSpan(0,0,0,0,20);//20 ms
        /// <summary>
        /// Gets the buffed value of a trait, applying any active buffs to the base trait value.
        /// Removes expired/inactive buffs from the TraitsSet.
        /// </summary>
        /// <param name="whichTrait">The trait to retrieve the buffed value for.</param>
        /// <returns>The buffed <see cref="TraitValue"/> after applying all active buffs.</returns>
        public TraitValue GetBuffedTraitValue(TraitDefinition whichTrait, DateTime? asOf =null, bool removeExpired=true)
        {
            // Retrieve the base trait value
            TraitValue baseValue = GetTraitBaseValue(whichTrait);
            TraitValue buffedValue = baseValue;
            if(asOf==null)
                asOf = GameStateDateTime.Now;
            //see if we have this trait's value cached, and if so, how recently that was done.
            if (lastComputedBuffedTraitValues.TryGetValue(whichTrait, out LastComputedBuffValue cachedValue))
            {
                if (asOf - cachedValue.lastComputed < recomputeTick)
                    return cachedValue.value;
                //expired do recompute anyway
            }
            // Apply each active buff to the base value, or remove it if expired
            for(int i=Buffs.Count-1;i>=0 ;i--)
            {
                BuffInstance buff = Buffs[i];
                if (!buff.IsActive(asOf.Value))
                {
                    if(removeExpired)
                        Buffs.RemoveAt(i);
                }
                else
                    buffedValue = buff.CalculateModifiedValue(whichTrait, buffedValue, asOf.Value);
            }
            //add or overwrite
            lastComputedBuffedTraitValues[whichTrait]= new LastComputedBuffValue() { lastComputed = asOf.Value, value = buffedValue };
            return buffedValue;
        }

        /// <summary>
        /// Gets the base value of a specific trait without applying buffs.
        /// </summary>
        /// <param name="whichTrait">The trait to retrieve the base value for.</param>
        /// <returns>The base <see cref="TraitValue"/> of the trait.</returns>
        public TraitValue GetTraitBaseValue(TraitDefinition whichTrait)
        {

            // Retrieve the base trait value from the Traits dictionary
            if (!Traits.TryGetValue(whichTrait, out float traitValue))
                throw new ArgumentException("Trait [" + whichTrait.Name + "] does not exist in Game Entity [" + ID + "]");
            return new TraitValue(whichTrait,traitValue);
        }

        /// <summary>
        /// Sets the value of a trait, replacing the existing value if the trait already exists.
        /// </summary>
        /// <param name="traitIdentifier">The trait to set the value for.</param>
        /// <param name="value">The new value to assign to the trait.</param>
        public void SetTraitValue(TraitDefinition traitIdentifier, TraitValue value)
        {
            if (Traits.ContainsKey(traitIdentifier))
            {
                Traits[traitIdentifier] = value.NumericValue;
            }
        }

        /// <summary>
        /// Adds a new buff to the entity, applying its effects to the entity's traits.
        /// </summary>
        /// <param name="typeOfBuff">The type of buff being applied.</param>
        /// <param name="startTime">The time when the buff was applied.</param>
        public void AddBuff(BuffDefinition typeOfBuff, DateTime startTime)
        {
            // Create a new BuffInstance
            BuffInstance buff = new BuffInstance(typeOfBuff, startTime);
            Buffs.Add(buff);
        }

        /// <summary>
        /// Logs when a buff is applied to the entity. Could be useful for debugging or UI.
        /// </summary>
        /// <param name="entity">The entity receiving the buff.</param>
        /// <param name="buff">The buff being applied.</param>
        private void BuffApplied(TraitsSet entity, BuffInstance buff)
        {
            Console.WriteLine($"Buff {buff.TypeOfBuff.Name} has been applied to {entity.ID}.");
        }

        /// <summary>
        /// Logs when a buff is removed from the entity. Could be useful for debugging or UI.
        /// </summary>
        /// <param name="entity">The entity losing the buff.</param>
        /// <param name="buff">The buff being removed.</param>
        private void BuffRemoved(TraitsSet entity, BuffInstance buff)
        {
            Console.WriteLine($"Buff {buff.TypeOfBuff.Name} has been removed from {entity.ID}.");
        }
        
        /// <summary>
        /// Removes expired buffs from the entity based on the current time.
        /// </summary>
        /// <param name="asOf">The current time to check for expired buffs.</param>
        public void RemoveExpiredBuffs(DateTime asOf)
        {
            // Remove all buffs that are no longer active
            Buffs.RemoveAll(buff => !buff.IsActive(asOf));
        }
        

        //runtime serialization

        public void Serialize(BinaryWriter writer)
        {
            ID.Serialize(writer);
            TypeOfEntityRef.Serialize(writer);
            Traits.SerializeDictionary(writer);
            Buffs.SerializeList(writer);
        }

        public void Deserialize(BinaryReader reader)
        {
            ID = reader.DeserializeLong();
            TypeOfEntityRef = (EntityTypeRef)reader.DeserializeTableElementRef<EntityType>();//  EyE.Collections.UnityAssetTables.TablesByElementType.DeserializeTableElement<EntityType>(reader);
            Traits.Deserialize(reader);
            Buffs.DeserializeList(reader);
        }
    }
}