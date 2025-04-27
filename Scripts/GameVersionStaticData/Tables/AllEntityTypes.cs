using System.Collections.Generic;
using UnityEngine;
using EyE.Collections.UnityAssetTables;
namespace EyE.Traits
{
    [CreateAssetMenu(fileName = "AllEntityTypes", menuName = "GameVersionData/All Entity Types")]
    public class AllEntityTypes : ImmutableTable<EntityType>, ISerializationCallbackReceiver
    {
        /*
        //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static public void OnLoad()
        {
            AllImmutable<EntityType, AllEntityTypes>.LoadOrCreate();
            //test
            Debug.Log("AllEntityTypes loaded.  testing...CombatUnit: " + CombatUnit.Name);
        }*/

        private static EntityType _CombatUnit;
        private static EntityType _Construction;
        private static EntityType _Provence;
        private static EntityType _Nation;
        public static EntityType CombatUnit => _CombatUnit ??= GetByName("CombatUnit");
        public static EntityType Construction => _Construction ??= GetByName("Construction");
        public static EntityType Provence => _Provence ??= GetByName("Provence");
        public static EntityType Nation => _Nation ??= GetByName("Nation");

        override public List<EntityType> GetDefaultTableElements()
        {
            List<EntityType> list = new List<EntityType>(){
                new EntityType("CombatUnit", new TraitValues( ( AllTraits.Health, 10f ), ( AllTraits.Speed, 3f ) )),
                new EntityType("Construction", new TraitValues( ( AllTraits.Health, 10f ) )),
                new EntityType("Provence", new TraitValues(( AllTraits.Income, 0f ))),
                new EntityType("Nation", new TraitValues(( AllTraits.Wealth, 0f),( AllTraits.Income, 0f) )),
            };

            return list;
        }
        override public void Initialize()
        {
            base.Initialize();
        }
    }
}