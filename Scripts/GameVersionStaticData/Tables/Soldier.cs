//USAGE EXAMPLES
/*
public static class SoldierTraits
{
    // Static instance of EntityType for all Soldier instances
    public static readonly EntityType SoldierType;

    static SoldierTraits()
    {
        // Initialize the EntityType for soldiers with default trait values
        SoldierType = new EntityType("Soldier");
        SoldierType.AddTrait(AllTraits.Health, 100); // Default health
        SoldierType.AddTrait(AllTraits.Speed, 5);   // Default speed
    }
}*/
using EyE.Traits;
public class Soldier : GameEntity
{

    public Soldier(long id) : base(id, AllEntityTypes.CombatUnit)
    {
    }

    public void TakeDamage(float damage)
    {
        // Assuming 'Health' is a trait of Soldier (it should be)
        TraitValue healthTrait = Traits[AllTraits.Health];
        if (healthTrait != null)
        {
            // Reduce health by the amount of damage taken, not allowing it to drop below zero
            float currentHP = base.GetBuffedTraitValue(AllTraits.Health).NumericValue;
            float newHP = currentHP - damage;
            if (newHP < 0) newHP = 0;
            float ratio = newHP / currentHP;
            healthTrait.NumericValue *= ratio;
            //healthTrait.BaseValue = healthTrait.BaseValue - damage;  //does not account for buffs
            //Console.WriteLine($"Soldier {Id} took {damage} damage, new health is {healthTrait.BaseValue}.");
        }
        else
        {
            //Console.WriteLine("Health trait is not defined for this soldier.");
        }
    }
}
