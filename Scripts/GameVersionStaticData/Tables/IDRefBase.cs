/*using System.IO;
using UnityEngine;

/// <summary>
/// Base class for all the various reference-object-by-ID classes.
/// When serialized this class will store only the id number, but at runtime it can implicitly lookup and provide a reference to the object (of type T) that has that id.
/// Descendant of this class will define and override the abstract function that does this lookup: GetReferencedObjectByID
/// </summary>
/// <typeparam name="T">Must implement IUniqueID</typeparam>
[System.Serializable]
public abstract class IDRefBase<T> : IBinarySaveLoad where T : IUniqueID
{
    [SerializeField]
    long _ID;
    virtual public long ID { get => _ID; protected set { _ID = value; } }
   // public string Name { get => GetReferencedObjectByID(ID).Name; }
    abstract protected T GetReferencedObjectByID(long id);

    public IDRefBase() { }
    public IDRefBase(long ID) { this.ID = ID; }
    public IDRefBase(T objToRef) { this.ID = objToRef.ID; }
    public void Deserialize(BinaryReader reader)
    {
        ID = reader.ReadInt64();
    }

    public void Serialize(BinaryWriter writer)
    {
        writer.Write(ID);
    }

    public static implicit operator T(IDRefBase<T> sdt) => sdt.GetReferencedObjectByID(sdt.ID);
    //this function must be defined by decendants as this class is abstract-(so you wouldn't have a concrete object to cast to this class)
    //public static implicit operator IDRefBase<T, S>(T dt) => new S(dt.ID);
}
[System.Serializable]
/// <summary>
/// Variant of IDRefBase for all the various reference-object-by-UniqueNameGaneratedID classes.
/// Can provide the Name of the looked up object directly.
/// </summary>
/// <typeparam name="T">Must implement IUniqueID and IUniqueName</typeparam>
public abstract class NamedIDRefBase<T> : IDRefBase<T> where T : IUniqueID,IUniqueName
{
    public NamedIDRefBase() : base() { }
    public NamedIDRefBase(long ID) : base(ID) { }
    public NamedIDRefBase(T objToRef) : base(objToRef) { }

    public string Name { get => GetReferencedObjectByID(ID).Name; }
}


// *****************
// various NamedIDRefBase types that implement IHaveAnID: they use the appropriate AllImmutable<> global tables to perform the lookups.
// *****************

[System.Serializable]
public class MeasurementUnitRef : NamedIDRefBase<MeasurementUnit>
{
    public MeasurementUnitRef() : base() { }
    override protected MeasurementUnit GetReferencedObjectByID(long id)
    {
        return AllMeasurementUnits.Get(id);
    }
    public MeasurementUnitRef(long measurementUnitID) : base(measurementUnitID) { }

    // Implicit Conversion
   // public static implicit operator MeasurementUnit(MeasurementUnitRef sdt) => AllMeasurementUnits.Get(sdt.ID);
    public static implicit operator MeasurementUnitRef(MeasurementUnit dt) => new MeasurementUnitRef(dt.ID);


}

[System.Serializable]
public class TraitRef : NamedIDRefBase<TraitDefinition>
{

    public TraitRef() : base() { }
    public TraitRef(long measurementUnitID)
    {
        ID = measurementUnitID;
    }
    override protected TraitDefinition GetReferencedObjectByID(long id)
    {
        return AllTraits.Get(id);
    }
    // Implicit Conversion
    //public static implicit operator TraitDefinition(TraitRef sdt) => AllTraits.Get(sdt.ID);
    public static implicit operator TraitRef(TraitDefinition dt) => new TraitRef(dt.ID);

}
[System.Serializable]
public class BuffRef : NamedIDRefBase<BuffType>
{
    public BuffRef() : base() { }
    public BuffRef(long buffID)
    {
        this.ID = buffID;
    }
    override protected BuffType GetReferencedObjectByID(long id)
    {
        return AllBuffs.Get(id);
    }
    // Implicit Conversion
   // public static implicit operator BuffType(BuffRef sdt) => AllBuffs.Get(sdt.ID);
    public static implicit operator BuffRef(BuffType dt) => new BuffRef(dt.ID);

}

[System.Serializable]
public class EntityTypeRef : NamedIDRefBase<EntityType>
{
    public EntityTypeRef() : base() { }
    public EntityTypeRef(long ID)
    {
        this.ID = ID;
    }
    override protected EntityType GetReferencedObjectByID(long id)
    {
        return AllEntityTypes.Get(id);
    }
    // Implicit Conversion
  //  public static implicit operator EntityType(EntityTypeRef sdt) => AllEntityTypes.Get(sdt.ID);
    public static implicit operator EntityTypeRef(EntityType dt) => new EntityTypeRef(dt.ID);

}
*/