using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace EyE.Collections.UnityAssetTables
{
    public interface IUniqueID
    {
        long ID { get; }
    }
    public interface IUniqueName
    {
        string Name { get; }
    }

    /// <summary>
    /// This class can be serialized by both unity and via IBinarySaveLoad- but there are differences.
    /// When serializing/deserializing with unity, all fields are serialized normally, and this is how they will be stored in permanent-at-runtime Tables.
    ///    This means,the tables, and their elements will be editable in the unity editor.
    /// When serializing with IBinarySaveLoad only the id number is serialized.
    /// When deserializing with IBinarySaveLoad- the element will be looked up in the appropriate table, and the reference to this object will be provided.
    ///     This differers from the usual IBinarySaveLoad functionality, where an already existing instance is populated with data.)
    /// The means that the element type may be used directly as members by other, IBinarySaveLoad classes.
    /// </summary>
    [Serializable]
    public class TableElement : IUniqueID, IUniqueName, ISerializationCallbackReceiver, IBinarySaveLoad
    {
        [SerializeField] //for display in editor- overrwitten by processed name->id when deserialized
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
        { }

        virtual protected long ReGenerateConsistentID()
        {
            return GenerateConsistentIDFromName(name);
        }
        static public long GenerateConsistentIDFromName(string name)
        {
            using (var md5 = MD5.Create())
            {
                byte[] hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(name));
                return BitConverter.ToInt64(hashBytes, 0);
            }
        }
        //only serialize the id- we will get the reference from the table on deserialize
        public void Serialize(BinaryWriter writer)
        {
            writer.Write(ID);
        }

        public void Deserialize(BinaryReader reader)
        {
            throw new NotImplementedException("It is not possible to deserialize a TableElement Reference directly.  Use [T TablesByElementType.DeserializeTableElement<T>(BinaryReader reader) where T: TableElement] to create a reference to the element stored in the table");

        }

    }



    /// <summary>
    /// Base class for tables of objects that will not change at runtime.
    /// They will be saved as assets containing a list of TableElement derived objects.  The table will be editable in unity, and may be Reset to default values.  
    /// </summary>
    public abstract class ImmutableTableBase : ScriptableObject, IEnumerable<TableElement>
    {
        public abstract Type TableElementType { get; }
        /// <summary>
        /// Clears and repopulates the table with default data, then initializes the table for runtime usage.
        /// Override and call base version to perform addition actions after the table is setup.
        /// </summary>
        virtual public void Reset()
        {
            PopulateWithDefault();
            Initialize();
        }

        /// <summary>
        /// Clears then populates the table with default elements.
        /// </summary>
        abstract public void PopulateWithDefault();
        abstract public void Initialize();
        abstract public void AddAsModTable();

        /// <summary>
        /// Attempts to retrieve an object by its ID from the dictionary.
        /// </summary>
        /// <param name="id">The ID of the object to retrieve.</param>
        /// <param name="obj">The object found, or null if not found.</param>
        /// <returns>True if the object was found, otherwise false.</returns>
        abstract public bool TryGetElement(long id, out TableElement obj);

        #region IEnumerable
        // Implement GetEnumerator for IEnumerable<T>
        public abstract IEnumerator<TableElement> GetEnumerator();// => idBasedDictionary.Values.GetEnumerator();

        // Explicit interface implementation for non-generic IEnumerable
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        #endregion
    }

    public class DrawElementDataAttribute : Attribute { }
    /// <summary>
    /// Base class for tables of objects that will not change at runtime.
    /// Serializable, and editable scriptable object in unity editor.  Becomes immutable in build.
    /// </summary>
    /// <typeparam name="T">The type of element the table contains.</typeparam>
    [Serializable]
    public abstract class ImmutableTable<T> : ImmutableTableBase, ISerializationCallbackReceiver where T : TableElement
    {
        #region Singleton
        /// <summary>
        /// The singleton instance of AllImmuntable.
        /// </summary>
        public static ImmutableTable<T> Instance
        {
            get
            {
                if (_instance == null)
                {
                    LoadOrCreate();//populates _instance
                    _instance.Initialize();
                    TablesByElementType.Register(_instance);
                }
                return _instance;
            }
        }
        protected static ImmutableTable<T> _instance = null;
        #endregion
        #region RegisterTable with TablesByElementType
        public override Type TableElementType { get { return typeof(T); } }

        #endregion

        #region RuntimeAndSerializedFields
        /// <summary>
        /// The internal runtime dictionary that stores the objects, keyed by their ID.
        /// </summary>
        private Dictionary<long, T> idBasedDictionary = new Dictionary<long, T>();

        /// <summary>
        /// A list used for Unity serialization, representing all the objects managed by this class.
        /// </summary>
        [SerializeField, DrawElementData]
        protected List<T> serializedList = new List<T>();
        #endregion

        #region GetElement functions, static and instance
        /// <summary>
        /// Attempts to retrieve an object by its ID from the dictionary.
        /// </summary>
        /// <param name="id">The ID of the object to retrieve.</param>
        /// <param name="obj">The object found, or null if not found.</param>
        /// <returns>True if the object was found, otherwise false.</returns>
        public static bool TryGet(long id, out T obj) => Instance.TryGetElement(id, out obj);

        /// <summary>
        /// Finds specific object in the Instance dictionary, by ID.  The dictionary itself will throws a (not very explicit) exception if the object does not exist.
        /// Use TryGet for greater control.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The object associated with the ID</returns>
        /// <exception cref="Exception">Dictionary exception Thrown if the object is not found. (Not very descriptive)</exception>
        public static T Get(long id) => Instance.GetElement(id);

        /// <summary>
        /// Attempts to retrieve an object by its ID from the dictionary.
        /// </summary>
        /// <param name="id">The ID of the object to retrieve.</param>
        /// <param name="obj">The object found, or null if not found.</param>
        /// <returns>True if the object was found, otherwise false.</returns>
        public bool TryGetElement(long id, out T obj) => idBasedDictionary.TryGetValue(id, out obj);

        override public bool TryGetElement(long id, out TableElement obj)
        {
            return TryGetElement(id, out obj);
        }

        /// <summary>
        /// Finds specific object in the Instance dictionary, by ID.  The dictionary itself will throws a (not very explicit) exception if the object does not exist.
        /// Use TryGet for greater control.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The object associated with the ID</returns>
        /// <exception cref="Exception">Dictionary exception Thrown if the object is not found. (Not very descriptive)</exception>
        public T GetElement(long id) => idBasedDictionary[id];

        /// <summary>
        /// Retrieves an object by its name, using a consistent ID generated from the name.
        /// Throws an exception if the object is not found.
        /// </summary>
        /// <param name="name">The name of the object to retrieve.</param>
        /// <returns>The object associated with the name.</returns>
        /// <exception cref="Exception">Thrown if the object is not found.</exception>
        public static T GetByName(string name)
        {
            long ID = TableElement.GenerateConsistentIDFromName(name);
            if (!Instance.idBasedDictionary.ContainsKey(ID)) throw new Exception("Expected " + typeof(T).Name + " [" + name + "] not preset in ImmutableTable " + Instance.name);
            return Instance.idBasedDictionary[ID];
        }

        #endregion
        #region Overridden and one new abstract Setup functions
        abstract public List<T> GetDefaultTableElements();

        /// <summary>
        /// Populates the serialized list with default values. This method is virtual and can/should be overridden by derived classes.
        /// </summary>
        /// <returns>A list of objects to populate the serialized list.</returns>
        override public void PopulateWithDefault()
        {
            serializedList = GetDefaultTableElements();
        }

        override public void Initialize()
        {
            idBasedDictionary.Clear();
            foreach (T obj in serializedList)
            {
                if (obj != null)
                {
                    if (!idBasedDictionary.ContainsKey(obj.ID))
                        idBasedDictionary[obj.ID] = obj;
                    else
                        throw new InvalidDataException("ID collision detected while initializing ImmutableTable<"+typeof(T).Name+">: "+name+". "
                            + "\n    Names generating collision: '" +obj.Name+ "' ,'" + idBasedDictionary[obj.ID].Name+"'");
                }
            }
        }
        #endregion
        #region Creation,Initialization and Serialization
        /// <summary>
        /// Called before serialization. Syncs the serialized list with the current dictionary values.
        /// </summary>
        public void OnBeforeSerialize()
        { }

        /// <summary>
        /// Called after deserialization, invokes the initialize function to setup the dictionary with the deserialized data.
        /// </summary>
        public void OnAfterDeserialize()
        {
            Initialize();
        }

        /// <summary>
        /// Called during lazy initialization of Instance, ensuring that the singleton is loaded from Resources.
        /// </summary>
        static void LoadOrCreate()
        {
            //Debug.Log("Loading AllImmuatable type " + typeof(S).Name);
            ImmutableTable<T>[] foundInstances = Resources.LoadAll<ImmutableTable<T>>("GameVersionData");
            if (foundInstances.Length > 1)
                Debug.LogWarning("Loading ImmutableTable<" + typeof(T).Name + "> asset, found more that one in Resources/GameVersionData.  Using first found.");
            if (foundInstances.Length == 0)
            {
                Debug.LogWarning("Loading ImmutableTable<" + typeof(T).Name + "> asset, found NO data in Resources/GameVersionData.  Using default values.");
                _instance = CreateInstance<ImmutableTable<T>>();
                _instance.Reset();
            }
            else
                _instance = Instantiate(foundInstances[0]);
        }
        #endregion
        #region Mods

        override public void AddAsModTable()
        {
            foreach (T item in this.idBasedDictionary.Values)
            {
                if (Instance.idBasedDictionary.ContainsKey(item.ID))
                    Debug.Log("Warning: Mod ID already exists in ImmutableTable<" + typeof(T).Name + ">.  Existing value:" + Instance.idBasedDictionary[item.ID].Name + "   mod element: " + item.Name);
                else
                    Instance.idBasedDictionary[item.ID] = item;
            }
        }
        #endregion
        #region IEnumerable
        // Implement GetEnumerator for IEnumerable<T>
      //  public IEnumerator<T> GetEnumerator() => idBasedDictionary.Values.GetEnumerator();
        public override IEnumerator<TableElement> GetEnumerator() => idBasedDictionary.Values.GetEnumerator();
        // Explicit interface implementation for non-generic IEnumerable
       // IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        #endregion
    }

    /// <summary>
    /// static registrar of all ImmutableTables.  Users may register or find a table by the type of elements it contains.
    /// ImmutableTableBase objects will automatically register with this class, when loaded.
    /// </summary>
    public static class TablesByElementType
    {
        static private Dictionary<Type, ImmutableTableBase> tablesByType = new Dictionary<Type, ImmutableTableBase>();

        static void Initialize()
        {
            ImmutableTableBase b = EyE.Traits.AllBuffs.Instance;
            ImmutableTableBase t = EyE.Traits.AllTraits.Instance;
            ImmutableTableBase m = AllMeasurementUnits.Instance;
            ImmutableTableBase e = EyE.Traits.AllEntityTypes.Instance;

        }

        /// <summary>
        /// Concrete ImmutableTable<T>'s will call this function on Awake function (which is usually called the first time the type's static Instance member is accessed via lazy loading).
        /// </summary>
        /// <param name="table"></param>
        public static void Register(ImmutableTableBase table)
        {
            if (tablesByType.ContainsKey(table.TableElementType))
                Debug.Log("TablesByElementType Error: attempt to register ImmutableTable [" + table.name + "] failed.. Table with this element type <"+ table.TableElementType .Name+ "> is already registered: ["+ tablesByType[table.TableElementType].name + "].  Not registering new table.");
            else
                tablesByType[table.TableElementType] = table;
        }

        /// <summary>
        /// Retrives the immutable table that uses the provided  element type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>Return the table that holds this element type.  If so such table has been registered, returns null</returns>
        public static ImmutableTable<T> GetTable<T>() where T : TableElement
        {
            Initialize();
            if (tablesByType.TryGetValue(typeof(T), out var table))
            {
                return table as ImmutableTable<T>;
            }

            return null;
        }

        /// <summary>
        /// Retrives the immutable table that uses the provided  element type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>Return the table that holds this element type.  If so such table has been registered, returns null</returns>
        public static ImmutableTableBase GetTable(System.Type typeOfElement)
        {
            Initialize();
            if (tablesByType.TryGetValue(typeOfElement, out ImmutableTableBase table))
            {
                return table;
            }

            return null;
        }
        /// <summary>
        /// Gets the table by type T, and the element by id
        /// </summary>
        /// <typeparam name="T">gets the table that has this type of element</typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        static public T GetTableElement<T>(long id) where T : TableElement
        {
            return GetTable<T>().GetElement(id);
        }


        public static T DeserializeTableElement<T>(this BinaryReader reader) where T : TableElement
        {
            long id = reader.ReadInt64();
            return TablesByElementType.GetTableElement<T>(id);
        }
    }
}