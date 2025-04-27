using EyE.Collections.UnityAssetTables;
namespace EyE.Traits
{
    /// <summary>
    /// Represents a type of entity that possesses a set of traits. 
    /// Examples include characters (with traits like strength, dexterity, intelligence) 
    /// or weapons (with traits like cost, damage, speed).
    /// </summary>
    [System.Serializable]
    public class EntityType : TableElement
    {
        /// <summary>
        /// A dictionary that associates each trait (defined by <see cref="TraitDefinition"/>) with a default value.
        /// This is used to store the default value of each trait for this entity type.
        /// </summary>
        [UnityEngine.SerializeField]
        private TraitValues _defaultTraitValues;

        public TraitValues DefaultTraitValues
        {
            get => _defaultTraitValues;
            private set
            {
                _defaultTraitValues = value;
            }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityType"/> class with the given name.
        /// </summary>
        /// <param name="name">The name of the entity type.</param>
        public EntityType(string name, TraitValues defaultTraitValues) : base()
        {
            Name = name;//base class will set ID based on this
            _defaultTraitValues = defaultTraitValues;

        }

        /// <summary>
        /// Adds a trait to this entity type along with its default value.
        /// </summary>
        /// <param name="trait">The trait to add, defined by <see cref="TraitDefinition"/>.</param>
        /// <param name="defaultValue">The default value for the trait.</param>
        public void AddTrait(TraitDefinition trait, float defaultValue)
        {
            DefaultTraitValues.Add(trait, defaultValue);
        }
    }
    [System.Serializable]
    public class EntityTypeRef : TableElementRef<EntityType>
    {
        public EntityTypeRef(long id, EntityType reference = null) : base(id, reference)
        {
        }
    }
}