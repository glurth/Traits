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

    static public class IDProvider
    {
        static long nid = 0;
        static object locker= new object();
        static public long GetNextID()
        {
            long i=nid;
            lock (locker)
            {
                nid++;
            }
            return i;
        }
        static public void IDLoaded(long id)
        {
            lock (locker)
            {
                if (nid < id)
                    nid = id + 1;
            }
        }
    }

    /// <summary>
    /// // the below has changed. now using seperate TableElementRef class only for references.  
    /// //nonono
    /// This class can be serialized by both unity and via IBinarySaveLoad- but there are differences.
    /// When serializing/deserializing with unity, all fields are serialized normally, and this is how they will be stored in permanent-at-runtime Tables.
    ///    This means,the tables, and their elements will be editable in the unity editor.
    /// When serializing with IBinarySaveLoad only the id number is serialized.
    /// When deserializing with IBinarySaveLoad- the element will be looked up in the appropriate table, and the reference to this object will be provided.
    ///     This differers from the usual IBinarySaveLoad functionality, where an already existing instance is populated with data.)
    /// The means that the element type may be used directly as members by other, IBinarySaveLoad classes.
    /// </summary>
    [Serializable]
    abstract public class TableElement : IUniqueID, ISerializationCallbackReceiver//, IBinarySaveLoad
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

        protected TableElement()
        {
            id = IDProvider.GetNextID();
        }

        public string Name
        {
            get => name;
            protected set
            {
                name = value;
            }
        }

        public void OnAfterDeserialize()
        {
            IDProvider.IDLoaded(id);
        }

        public void OnBeforeSerialize()
        { }
        /*
        //for runtime stuff- only serialize the id- we will get the reference from the table on deserialize
        public void Serialize(BinaryWriter writer)
        {
            writer.Write(ID);
        }
        //at runtime, TableElements are not loaded from the save file.  Only the refence ID is loaded, and used to get a reference to the already-loaded table.
        public void Deserialize(BinaryReader reader)
        {
            throw new NotImplementedException("It is not possible to deserialize a TableElement Reference directly.  Instead, deserialize the reference from the containing object using ``TablesByElementType.DeserializeTableElement<T>(BinaryReader reader)`` to get a reference to the element stored in the appropriate table");
            //throw new NotImplementedException("TableElement based class, "+GetType()+" does not override the Derserialize Method.");
        }
        */
    }
    
    [Serializable]
    public class TableElementRef<T> : ISerializationCallbackReceiver, IBinarySaveLoad where T:TableElement
    {
        [SerializeField] long ID;
        [NonSerialized] T reference=null;

        public TableElementRef(long id, T reference=null)
        {
            this.ID = id;
            if (reference != null)
                this.reference = reference;
            else
                PopulateReference();
        }

        public T Value
        {
            get
            {
                if (reference == null)
                    PopulateReference();
                return reference;
            }
            set
            {
                ID = value.ID;
                reference = value;
            }
        }
        public Type refToType => typeof(T);

        public static implicit operator T(TableElementRef<T> r) => r?.Value;
        public static implicit operator TableElementRef<T>(T r) => new TableElementRef<T>(r.ID, r );

        public void PopulateReference()
        {
            if (ID != -1)
                reference = TablesByElementType.GetTableElement<T>(ID);
            else
                Debug.LogWarning("TableElementReference<" + typeof(T) + "> ID number is -1, unable to get reference.");
        }
        //unity serialization stuff.. for references in immutabletables
        public void OnBeforeSerialize()
        {
            
            if (reference != null)
                ID = reference.ID;
            else
            {
                if(ID==-1)
                    Debug.LogWarning("Attempting to serialize TableElementReference<"+typeof(T)+"> to ID number, but the reference is null.  Serializing -1 ID.");
                //else
                    //Debug.LogWarning("Attempting to serialize TableElementReference<" + typeof(T) + "> to ID number: "+ID+", but the reference is null.  May not have been populated yet.  Storing ID anyway.");
            }
        }
        //Use the serialized id to get ref from the table
        public void OnAfterDeserialize()
        {
           // Debug.Log("OnDeserialize TableElementRef<"+typeof(T)+">");
           // PopulateReference(); // we cannot do this yet-  the referenced tables might not be loaded yet
        }
        //runtime for state storage
        public void Serialize(BinaryWriter writer)
        {
            writer.Write(ID);
        }
        //at runtime, TableElements are not loaded from the save file.  Only the reference ID is loaded, and used to get a reference to the already-loaded table asset.
        public void Deserialize(BinaryReader reader)
        {
            ID=reader.ReadInt64();
            PopulateReference();
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
        /// <summary>
        /// Initialziation Operations to be performed after the table has been loaded.
        /// </summary>
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
                    throw new Exception("Instance is not yet initialized");
                    //LoadOrNull();//populates _instance
                   // if(_instance==null) return null;
                   // _instance.Initialize();
                   // TablesByElementType.Register(_instance);
                }
                return _instance;
            }
        }
        protected static ImmutableTable<T> _instance = null;
        #endregion
        
        public override Type TableElementType { get { return typeof(T); } }

        
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
            bool found = idBasedDictionary.TryGetValue(id, out T typedObj);
            obj = typedObj as TableElement;
            return found;
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
        /// Retrieves an object by its name, by iterating through the table.  Slow, use for init only.
        /// Throws an exception if the object is not found.
        /// </summary>
        /// <param name="name">The name of the object to retrieve.</param>
        /// <returns>The object associated with the name.</returns>
        /// <exception cref="Exception">Thrown if the object is not found.</exception>
        public static T GetByName(string name)
        {
            foreach (var kvp in Instance.idBasedDictionary)
                if (kvp.Value.Name == name)
                    return kvp.Value;

            throw new Exception("Expected " + typeof(T).Name + " [" + name + "] not preset in ImmutableTable " + Instance.name);
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
            Debug.Log("ImmutableTable<" + typeof(T).Name + ">: rebuilding Internal dictionary from serialized data");
            idBasedDictionary.Clear();
            foreach (T obj in serializedList)
            {
                if (obj != null)
                {
                    if (!idBasedDictionary.ContainsKey(obj.ID))
                        idBasedDictionary[obj.ID] = obj;
                    else
                        throw new InvalidDataException("ID collision detected while initializing ImmutableTable<" + typeof(T).Name + ">: ");// +name+". "
                            //+ "\n    Names generating collision: '" +obj.Name+ "' ,'" + idBasedDictionary[obj.ID].Name+"'");
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
           // Debug.Log("After loaded table ImmutableTable<" + typeof(T).Name + ">");
           Initialize();
        }

        /// <summary>
        /// Called during lazy initialization of Instance, ensuring that the singleton is loaded from Resources.
        /// </summary>
        static void LoadOrNull<TTableType>() where TTableType : ImmutableTable<T>
        {
            Debug.Log("Loading "+typeof(TTableType).Name+ ":ImmutableTable<" + typeof(T).Name+">");
            TTableType[] foundInstances = Resources.LoadAll<TTableType>("GameVersionData");
            
            Debug.Log("Loading ImmutableTable<" + typeof(T).Name + "> List complete, confirming results.");
            if (foundInstances==null)
            {
                Debug.LogWarning("Loading ImmutableTable<" + typeof(T).Name + "> asset, found NO data in Resources/GameVersionData.");
                return;
            }
            if (foundInstances.Length > 1)
                Debug.LogWarning("Loading ImmutableTable<" + typeof(T).Name + "> asset, found more that one in Resources/GameVersionData.  Using first found.");
            if (foundInstances.Length == 0)
            {
                Debug.LogWarning("Loading ImmutableTable<" + typeof(T).Name + "> asset, found NO data in Resources/GameVersionData.");
                return;
            }
            else
                _instance = //Instantiate
                    (ImmutableTable<T>)(foundInstances[0]);

        //    _instance.Initialize();
            TablesByElementType.Register(_instance);
        }

        /// <summary>
        /// Called during lazy initialization of Instance, ensuring that the singleton is loaded from Resources.
        /// </summary>
        static public void LoadOrCreate<TTableType>() where TTableType : ImmutableTable<T>
        {
            LoadOrNull<TTableType>();
            if (_instance == null)
            {
                #if UNITY_EDITOR

                                Debug.LogWarning("Loading ImmutableTable " + typeof(TTableType).Name + " asset, found NO data in Resources/GameVersionData.  Using default values.");
                                _instance = CreateInstance<TTableType>();
                                //_instance.Reset(); // reset called automatically by creatinstance
                                UnityEditor.AssetDatabase.CreateAsset(_instance, "Assets/Resources/GameVersionData/" + typeof(TTableType).Name + ".asset");
                                
                                //_instance.Initialize();
                                TablesByElementType.Register(_instance);
                #else
                                throw new  IOException("Loading ImmutableTable<" + typeof(T).Name + "> asset, found NO data in Resources/GameVersionData.");
                #endif
            }
            
            /*
            //Debug.Log("Loading AllImmuatable type " + typeof(S).Name);
            ImmutableTable<T>[] foundInstances = Resources.LoadAll<ImmutableTable<T>>("GameVersionData");
            if (foundInstances.Length > 1)
                Debug.LogWarning("Loading ImmutableTable: " + typeof(TTableType).Name + " asset, found more that one in Resources/GameVersionData.  Using first found.");
            if (foundInstances.Length == 0)
            {
#if UNITY_EDITOR

                Debug.LogWarning("Loading ImmutableTable " + typeof(TTableType).Name + " asset, found NO data in Resources/GameVersionData.  Using default values.");
                _instance = CreateInstance<TTableType>();
                _instance.Reset();
                UnityEditor.AssetDatabase.CreateAsset(_instance, "Assets/Resources/GameVersionData/" + typeof(TTableType).Name + ".asset");

#else
                throw new  IOException("Loading ImmutableTable<" + typeof(T).Name + "> asset, found NO data in Resources/GameVersionData.");
#endif
            }
            else
                _instance = Instantiate(foundInstances[0]);
            _instance.Initialize();
            TablesByElementType.Register(_instance);*/
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

        public override IEnumerator<TableElement> GetEnumerator() => idBasedDictionary.Values.GetEnumerator();

        #endregion
    }

    /// <summary>
    /// static registrar of all ImmutableTables.  Users may register or find a table by the type of elements it contains.
    /// ImmutableTableBase objects will automatically register with this class, when loaded.
    /// </summary>
    public static class TablesByElementType
    {
        static private Dictionary<Type, ImmutableTableBase> tablesByType = null;

        static bool init = false;
        [RuntimeInitializeOnLoadMethod]
        [UnityEditor.InitializeOnLoadMethod]
        static void Initialize()
        {
            if (init) return;
            init = true;
            tablesByType = new Dictionary<Type, ImmutableTableBase>();
            Debug.Log("AllMeasurementUnits: Initializing.");
            EyE.Traits.AllMeasurementUnits.LoadOrCreate<EyE.Traits.AllMeasurementUnits>();
            ImmutableTableBase m = EyE.Traits.AllMeasurementUnits.Instance;

            Debug.Log("AllTraits: Initializing.");
            EyE.Traits.AllTraits.LoadOrCreate<EyE.Traits.AllTraits>();
            ImmutableTableBase t = EyE.Traits.AllTraits.Instance;

            Debug.Log("AllEntityTypes: Initializing.");
            EyE.Traits.AllEntityTypes.LoadOrCreate<EyE.Traits.AllEntityTypes>();
            ImmutableTableBase e = EyE.Traits.AllEntityTypes.Instance;

            Debug.Log("AllBuffs: Initializing.");
            EyE.Traits.AllBuffs.LoadOrCreate<EyE.Traits.AllBuffs>();
            ImmutableTableBase b = EyE.Traits.AllBuffs.Instance;

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
            return (ImmutableTable<T>)GetTable(typeof(T));
        }

        /// <summary>
        /// Retrives the immutable table that uses the provided  element type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>Return the table that holds this element type.  If so such table has been registered, returns null</returns>
        public static ImmutableTableBase GetTable(System.Type typeOfElement)
        {
            if (tablesByType == null) throw new Exception("TablesByElementType has not been initialized yet, no tables registered including:" + typeOfElement);
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
            ImmutableTable<T> table = GetTable<T>();
            if (table == null)
            {
                throw new Exception("TablesByElementType: unable to find ImmutableTable of type : " + typeof(T) + " in registry: "+ string.Join(",", tablesByType.Keys) );
            } 
            return table.GetElement(id);
        }


        public static T DeserializeTableElement<T>(this BinaryReader reader) where T : TableElement
        {
            long id = reader.ReadInt64();
            return TablesByElementType.GetTableElement<T>(id);
        }
    }
}