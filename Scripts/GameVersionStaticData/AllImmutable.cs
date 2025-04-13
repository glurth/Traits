/*
// Ignore Spelling: Immutables

using System.Collections.Generic;
using UnityEngine;
using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;

public interface IUniqueID
{
    long ID { get; }
}
public interface IUniqueName
{
    string Name { get; }
}


/// <summary>
/// Base class for all immutable objects.  When serialized at rutime, as part of a saved game
/// </summary>
[Serializable]
public class NameBasedID: IUniqueID, IUniqueName, ISerializationCallbackReceiver, IBinarySaveLoad
{
    
   // [SerializeField]
    protected long id;
    public long ID
    {
        get => id;
        private set => id = value;
    }
 
    [SerializeField]
    private string name;
    public string Name
    {
        get => name;
        protected set
        {
            name = value;
            id = ReGenerateConsistentID();
        }
    }

    public void OnAfterDeserialize()
    {
        id = ReGenerateConsistentID();
    }

    public void OnBeforeSerialize()
    {
      // id = ReGenerateConsistentID();
    }

    virtual protected long ReGenerateConsistentID()
    {
        using (var md5 = MD5.Create())
        {
            byte[] hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(name));
            return BitConverter.ToInt64(hashBytes, 0);
        }
    }
    static public long GenerateConsistentID(string name)
    {
        using (var md5 = MD5.Create())
        {
            byte[] hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(name));
            return BitConverter.ToInt64(hashBytes, 0);
        }
    }

    public void Serialize(BinaryWriter writer)
    {
        writer.Write(ID);
    }

    public void Deserialize(BinaryReader reader)
    {
        long id = reader.ReadInt64();
    }
}



/// <summary>
/// A singleton ScriptableObject that holds and manages a collection of immutable objects of type T,
/// where T implements the IUniqueID interface. It allows for unity serialization, deserialization, and custom initialization.
/// S represents the concrete, non-generic class that inherits from this one. (S param exists because this class needs to call CreateInstance<S> in a static function, which does not support generic classes)
/// Default values for various allImmuntables, should be defined in code by override the populate function.  
/// The table generated can be added to in the editor, or otherwise modifying the scriptableObject file after distro.
/// </summary>
/// <typeparam name="T">Type of item this collection is made of</typeparam>
/// <typeparam name="S">Type of the class that inherits from this one  e.g. AllTraits:AllImmutable<Trait,AllTraits> </typeparam>
public class AllImmutable<T,S> : ScriptableObject, ISerializationCallbackReceiver where T : IUniqueID where S : AllImmutable<T, S>
{
    /// <summary>
    /// The singleton instance of AllImmuntable.
    /// </summary>
    public static AllImmutable<T,S> Instance
    {
        get
        {
            if (_instance == null)
            {
                LoadOrCreate();//populates _instance
                _instance.Initialize();
            }
            return _instance;
        }
    }
    private static AllImmutable<T,S> _instance=null;

    /// <summary>
    /// A read-only dictionary containing all the objects managed by this class, indexed by their ID.
    /// </summary>
    public static IReadOnlyDictionary<long, T> All => Instance?.allDictionary;

    /// <summary>
    /// The internal dictionary that stores the objects, keyed by their ID.
    /// </summary>
    private Dictionary<long, T> allDictionary = new Dictionary<long, T>();

    /// <summary>
    /// A list used for Unity serialization, representing all the objects managed by this class.
    /// </summary>
    [SerializeField]
    protected List<T> serializedList = new List<T>();

    /// <summary>
    /// Populates the serialized list with default values. This method is virtual and can be overridden by derived classes.
    /// </summary>
    /// <returns>A list of objects to populate the serialized list.</returns>
    virtual public List<T> Populate()
    {
        return new List<T>();
    }

    /// <summary>
    /// Resets the list with the result of 'Populate()' then initializes the dictionary using that list via `Initialize()`.
    /// Override and call base version to perform addition actions after the dictionary is setup.
    /// </summary>
    virtual public void Reset()
    {
        //base.name = "DefaultValue";
        serializedList = Populate();
        Initialize();
    }

    /// <summary>
    /// Initializes the dictionary from the serialized list, mapping each object by its ID.
    /// </summary>
    virtual public void Initialize()
    {
        //Debug.Log("Initializing AllImmutable<T>"+ typeof(T).Name+">");
        _instance = this;
        allDictionary.Clear();
        foreach (T obj in serializedList)
        {
            if (obj != null)
            {
                if(!allDictionary.ContainsKey(obj.ID))
                    allDictionary[obj.ID] = obj;
            }
        }
       //ApplyModdedAdditions();
    }

    //modded additions
    void DoRegisterModdedImmutables(List<T> additionalImmutables)
    {
        foreach (T item in additionalImmutables)
        {
            Register(item);
        }
    }
    public static void RegisterModdedImmutables(List<T> additionalImmutables)
    {
        Instance.DoRegisterModdedImmutables(additionalImmutables);
    }

    /// <summary>
    /// Registers a new object in the dictionary if it is not already registered.
    /// </summary>
    /// <param name="obj">The object to register.</param>
    public void Register(T obj)
    {
        if (!allDictionary.ContainsKey(obj.ID))
        {
            serializedList.Add(obj);
            allDictionary.Add(obj.ID,obj);
        }
    }

    /// <summary>
    /// Called during lazy initialization of Instance, ensuring that the singleton is loaded from Resources.
    /// </summary>
    //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]  attribute not applied because this class is generic<T,S> and so, wont run
    static void LoadOrCreate()
    {
        //Debug.Log("Loading AllImmuatable type " + typeof(S).Name);
        AllImmutable<T,S>[] foundInstances = Resources.LoadAll<S>("GameVersionData");
        if (foundInstances.Length > 1)
            Debug.LogWarning("Loading AllImmutable<" + typeof(S).Name + "> asset, found more that one in Resources/GameVersionData.  Using first found.");
        if (foundInstances.Length == 0)
        {
            Debug.LogError("Loading AllImmutable<"+ typeof(S).Name+"> asset, found NO data in Resources/GameVersionData.  Using default values.");
            _instance = CreateInstance<S>();
            _instance.Reset();
        }
        else
            _instance = Instantiate(foundInstances[0]);
    }

    /// <summary>
    /// Attempts to retrieve an object by its ID from the dictionary.
    /// </summary>
    /// <param name="id">The ID of the object to retrieve.</param>
    /// <param name="obj">The object found, or null if not found.</param>
    /// <returns>True if the object was found, otherwise false.</returns>
    public static bool TryGet(long id, out T obj) => Instance.allDictionary.TryGetValue(id, out obj);

    /// <summary>
    /// Finds specific object in the Instance dictionary, by ID.  the dictionary itself will throws a (not very explicit) exception if the object does not exist.
    /// Use TryGet for greater control.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>The object associated with the ID</returns>
    /// <exception cref="Exception">Dictionary exception Thrown if the object is not found. (Not very descriptive)</exception>
    public static T Get(long id) => Instance.allDictionary[id];

    /// <summary>
    /// Retrieves an object by its name, using a consistent ID generated from the name.
    /// Throws an exception if the object is not found.
    /// </summary>
    /// <param name="longName">The name of the object to retrieve.</param>
    /// <returns>The object associated with the name.</returns>
    /// <exception cref="Exception">Thrown if the object is not found.</exception>
    public static T GetByName(string longName)
    {
        long ID = NameBasedID.GenerateConsistentID(longName);
        if (!Instance.allDictionary.ContainsKey(ID)) throw new Exception("Expected "+typeof(S).Name+" [" + longName + "] not preset in "+ Instance.name);
        return Instance.allDictionary[ID];
    }
    /// <summary>
    /// Returns true and populates the value param if the name exists in the dictionary.
    /// Returns false and populates the value param with default(T), if the name does not exists in the dictionary.
    /// </summary>
    /// <param name="longName"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool TryGetByName(string longName, out T value)
    {
        long ID = NameBasedID.GenerateConsistentID(longName);
        if (!Instance.allDictionary.ContainsKey(ID))
        {
            value = default(T); return false;
        }
        value=Instance.allDictionary[ID];
        return true;
    }
    /// <summary>
    /// Called before serialization. Syncs the serialized list with the current dictionary values.
    /// </summary>
    public void OnBeforeSerialize()
    {}

    /// <summary>
    /// Called after deserialization. Reinitializes the dictionary with the deserialized data.
    /// </summary>
    public void OnAfterDeserialize()
    {
        allDictionary.Clear();
        Initialize();
    }
}

*/