using System.Collections.Generic;
using UnityEngine;
using EyE.Collections.UnityAssetTables;
namespace EyE.Traits
{
    [CreateAssetMenu(fileName = "AllBuffs", menuName = "GameVersionData/All Buffs")]
    public class AllBuffs : ImmutableTable<BuffDefinition>, ISerializationCallbackReceiver
    {
        override public List<BuffDefinition> GetDefaultTableElements()
        {
            //AllTraits ens = (AllTraits)AllTraits.Instance;
            List<BuffDefinition> list = new List<BuffDefinition>(){
        new BuffDefinition(
                "Haste",
                new System.TimeSpan(0, 0, 10),
                new TraitEffects()
                {
                    new KeyValuePair<TraitDefinitionRef, TraitEffectList>(
                        //keyvalue pair
                        AllTraits.Speed, //key
                        new List<TraitEffect>{ new TraitEffect(1.2f,  Multiplicative.Instance ) }// BuffArithmetic.Multiplicative) }
                    )
                }
            ),
        new BuffDefinition(
                "Slow",
                new System.TimeSpan(0, 0, 10),
                new TraitEffects()
                {
                    new KeyValuePair<TraitDefinitionRef, TraitEffectList>(
                        //keyvalue pair
                            AllTraits.Speed,
                            new List<TraitEffect>{new TraitEffect(0.7f, Multiplicative.Instance) }
                    )
                }
            ),
        new BuffDefinition(
                "Small Heal",
                null,
                new TraitEffects()
                {
                    new KeyValuePair<TraitDefinitionRef, TraitEffectList>(
                        //keyvalue pair
                        AllTraits.Health,
                        new List<TraitEffect>{new TraitEffect(2f, Additive.Instance) }
                    )
                }
            ),
        new BuffDefinition(
                "Vitality",
                new System.TimeSpan(0, 0, 10),
                new TraitEffects()
                {
                        new KeyValuePair<TraitDefinitionRef, TraitEffectList>(
                        //keyvalue pair
                            AllTraits.Health,
                            new List<TraitEffect>{new TraitEffect(1.2f, new Multiplicative()) }
                        ),
                        new KeyValuePair<TraitDefinitionRef, TraitEffectList>(
                        //keyvalue pair
                            AllTraits.Speed,
                            new List<TraitEffect>{new TraitEffect(3f, new Additive()), new TraitEffect(1.5f, new Multiplicative()) }
                        )

                }
            )
        };
            return list;
        }

        //convenient access to specific buffs
        private static BuffDefinition _haste;
        private static BuffDefinition _slow;
        private static BuffDefinition _smallHeal;
        private static BuffDefinition _vitality;

        public static BuffDefinition Haste => _haste ??= GetByName("Haste");
        public static BuffDefinition Slow => _slow ??= GetByName("Slow");
        public static BuffDefinition SmallHeal => _smallHeal ??= GetByName("SmallHeal");
        public static BuffDefinition Vitality => _vitality ??= GetByName("Vitality");

    }
}