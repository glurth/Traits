using System.Collections.Generic;
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
    /// <summary>
    /// Represents a game entity, such as a character or object, that has traits and can receive buffs.
    /// </summary>
    public class GameEntity : IBinarySaveLoad
    {
        /// <summary>
        /// Unique identifier for the game entity.
        /// </summary>
        public long ID { get; set; }

        /// <summary>
        /// The type of the entity, which defines its default traits.
        /// </summary>
        public EntityType TypeOfEntity { get; private set; }

        /// <summary>
        /// Dictionary of trait definitions to their corresponding values for this entity.
        /// Initial values come from the EntityType Type
        /// </summary>
        public TraitValues Traits { get; private set; }

        /// <summary>
        /// List of active buffs affecting the entity.
        /// </summary>
        public List<BuffInstance> Buffs { get; private set; }

        public GameEntity()
        {
            Traits = new TraitValues();
            Buffs = new List<BuffInstance>();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="GameEntity"/> class with the specified ID and type.
        /// </summary>
        /// <param name="id">The unique identifier for the entity.</param>
        /// <param name="type">The type of the entity, which defines its traits.</param>
        public GameEntity(long id, EntityType type)
        {
            ID = id;
            TypeOfEntity = type;
            Traits = new TraitValues();
            Buffs = new List<BuffInstance>();
            InitializeTraits(); // Initialize traits based on the entity type
        }

        /// <summary>
        /// Initializes the traits of the entity based on the default trait values from its type.
        /// </summary>
        private void InitializeTraits()
        {
            Traits = new TraitValues(TypeOfEntity.DefaultTraitValues);
        }

        /// <summary>
        /// Gets the buffed value of a trait, applying any active buffs to the base trait value.
        /// </summary>
        /// <param name="whichTrait">The trait to retrieve the buffed value for.</param>
        /// <returns>The buffed <see cref="TraitValue"/> after applying all active buffs.</returns>
        public TraitValue GetTraitBuffedValue(TraitDefinition whichTrait, DateTime? asOf =null)
        {
            // Retrieve the base trait value
            TraitValue baseValue = GetTraitBaseValue(whichTrait);
            TraitValue buffedValue = baseValue;
            if(asOf==null)
                asOf = GameStateDateTime.Now;
            // Apply each active buff to the base value
            foreach (var buff in Buffs)
            {
                buffedValue = buff.CalculateModifiedValue(whichTrait, buffedValue, asOf.Value);
            }

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
            if (!Traits.TryGetValue(whichTrait, out TraitValue traitValue))
                throw new ArgumentException("Trait [" + whichTrait.Name + "] does not exist in Game Entity [" + ID + "]");
            return traitValue;
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
                Traits[traitIdentifier] = value;
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
            /*  the following makes it difficult to manually remove permanent bufss, so not doing it- we'll just have to compute 'em each time.
            // If the buff has a duration, add it to the active buff list
            if (typeOfBuff.Duration != null)
            {
                Buffs.Add(buff);
            }
            else
            {
                // For permanent buffs, immediately apply the buff's effects to the relevant traits
                foreach (TraitDefinition traitId in typeOfBuff.Effects.Keys)
                {
                    SetTraitValue(traitId, buff.CalculateModifiedValue(traitId, GetTraitBaseValue(traitId), DateTime.Now));
                }
            }*/
        }

        /// <summary>
        /// Logs when a buff is applied to the entity. Could be useful for debugging or UI.
        /// </summary>
        /// <param name="entity">The entity receiving the buff.</param>
        /// <param name="buff">The buff being applied.</param>
        private void BuffApplied(GameEntity entity, BuffInstance buff)
        {
            Console.WriteLine($"Buff {buff.TypeOfBuff.Name} has been applied to {entity.ID}.");
        }

        /// <summary>
        /// Logs when a buff is removed from the entity. Could be useful for debugging or UI.
        /// </summary>
        /// <param name="entity">The entity losing the buff.</param>
        /// <param name="buff">The buff being removed.</param>
        private void BuffRemoved(GameEntity entity, BuffInstance buff)
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
            TypeOfEntity.Serialize(writer);
            Traits.SerializeDictionary(writer);
            Buffs.SerializeList(writer);
        }

        public void Deserialize(BinaryReader reader)
        {
            ID = reader.DeserializeLong();
            this.TypeOfEntity = EyE.Collections.UnityAssetTables.TablesByElementType.DeserializeTableElement<EntityType>(reader);
            Traits.Deserialize(reader);
            Buffs.DeserializeList(reader);
        }
    }
}