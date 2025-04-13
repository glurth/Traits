using System.Collections.Generic;
using UnityEngine;
using EyE.Collections.UnityAssetTables;
namespace EyE.Traits
{
    [CreateAssetMenu(fileName = "AllTraits", menuName = "GameVersionData /All Traits")]
    public class AllTraits : ImmutableTable<TraitDefinition>, ISerializationCallbackReceiver
    {
        private static TraitDefinition _health;
        private static TraitDefinition _speed;
        private static TraitDefinition _wealth;
        private static TraitDefinition _income;

        public static TraitDefinition Health => _health ??= GetByName("Health");
        public static TraitDefinition Speed => _speed ??= GetByName("Speed");
        public static TraitDefinition Wealth => _wealth ??= GetByName("Wealth");
        public static TraitDefinition Income => _income ??= GetByName("Income");

        override public List<TraitDefinition> GetDefaultTableElements()
        {
            List<TraitDefinition> list = new List<TraitDefinition>(){
            new TraitDefinition("Health", ValueType.Number, PositiveValueImpact.Positive, AllMeasurementUnits.HitPoints, 1),
            new TraitDefinition("Speed", ValueType.Number, PositiveValueImpact.Positive, AllMeasurementUnits.MilesPerHour, 0),
            new TraitDefinition("Wealth", ValueType.Number, PositiveValueImpact.Positive, AllMeasurementUnits.Gold, 0),
            new TraitDefinition("Income", ValueType.Number, PositiveValueImpact.Positive, AllMeasurementUnits.Gold, 0)
        };

            return list;
        }

    }
}