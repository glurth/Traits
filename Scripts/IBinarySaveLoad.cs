using System.IO;
using System;
using System.Collections.Generic;




/// <summary>
/// Defines a contract for binary serialization and deserialization of objects.
/// </summary>
public interface IBinarySaveLoad
{
    /// <summary>
    /// Serializes the current instance of the object into a BinaryWriter stream.
    /// </summary>
    /// <param name="writer">
    /// The BinaryWriter used to write the object's state.
    /// </param>
    /// <remarks>
    /// Implementers should write all necessary fields and properties to fully represent the object's state.
    /// </remarks>
    void Serialize(BinaryWriter writer);

    /// <summary>
    /// Deserializes data from a BinaryReader stream to populate and otherwise initialize the current object instance.
    /// In order to function with the BinarySerializeExtension functions defined below, classes implementing this interface will also need a parameterless constructor:  the instance upon which Deserialize will be called)
    /// </summary>
    /// <param name="reader">
    /// The BinaryReader used to read the object's state.
    /// </param>
    /// <remarks>
    /// This method should fully populate the current instance based on the data read from the stream, and perform any initialization actions.
    /// </remarks>
    void Deserialize(BinaryReader reader);
}


public static class BinarySerializeExtension
{
    // Serialize methods
    public static void Serialize(this IBinarySaveLoad i, BinaryWriter writer)
    {
        i.Serialize(writer);
        //writer.Write(i);
    }

    public static void Serialize(this int i, BinaryWriter writer)
    {
        writer.Write(i);
    }

    public static void Serialize(this float f, BinaryWriter writer)
    {
        writer.Write(f);
    }

    public static void Serialize(this string s, BinaryWriter writer)
    {
        writer.Write(s);
    }

    public static void Serialize(this double d, BinaryWriter writer)
    {
        writer.Write(d);
    }

    public static void Serialize(this bool b, BinaryWriter writer)
    {
        writer.Write(b);
    }

    public static void Serialize(this long l, BinaryWriter writer)
    {
        writer.Write(l);
    }
    public static void Serialize(this UnityEngine.ScriptableObject o, BinaryWriter writer)
    {
        writer.Write(ResourceManager.GetPathOfObject(o));
    }

    // Deserialize methods-
    // The type read from the reader needs to be specified when this function is called.
    // This is USUALY done just by the function name, but DeserializeBinary,DeserializeList and the collections, being an exception that takes a generic parameter
    public static T DeserializeBinary<T>(this BinaryReader reader) where T : IBinarySaveLoad, new()
    {
        T newObj = new T();
        newObj.Deserialize(reader);
        return newObj;
    }

    public static int DeserializeInt(this BinaryReader reader)
    {
        return reader.ReadInt32();
    }

    public static float DeserializeFloat(this BinaryReader reader)
    {
        return reader.ReadSingle();
    }

    public static string DeserializeString(this BinaryReader reader)
    {
        return reader.ReadString();
    }

    public static double DeserializeDouble(this BinaryReader reader)
    {
        return reader.ReadDouble();
    }

    public static bool DeserializeBool(this BinaryReader reader)
    {
        return reader.ReadBoolean();
    }

    public static long DeserializeLong(this BinaryReader reader)
    {
        return reader.ReadInt64();
    }
    public static EyE.Collections.UnityAssetTables.TableElementRef<T> DeserializeTableElementRef<T>(this BinaryReader reader) where T: EyE.Collections.UnityAssetTables.TableElement
    {
        long traitID = reader.DeserializeLong();
        return new EyE.Collections.UnityAssetTables.TableElementRef<T>(traitID);
    }
    /*public static T DeserializeTableElement<T>(this BinaryReader reader) where T: EyE.Collections.UnityAssetTables.TableElement
    {
        return EyE.Collections.UnityAssetTables.TablesByElementType.DeserializeTableElement<T>(reader);
    }*/

    public static UnityEngine.Object DeserializeSO(this BinaryReader reader)
    {
        string resourcePath = reader.ReadString();
        return UnityEngine.Resources.Load(resourcePath);
    }
    public static T DeserializeSO<T>(this BinaryReader reader) where T:UnityEngine.Object
    {
        string resourcePath = reader.ReadString();
        return UnityEngine.Resources.Load(resourcePath) as T;
    }

    /// <summary>
    /// Serializes a list of T objects
    /// </summary>
    public static void SerializeList<T>(this List<T> list, BinaryWriter writer)
    {
        writer.Write(list.Count); // Write dictionary size
        foreach (T element in list)
        {
            Serialize(element, writer); // Serialize key
        }
    }

    /// <summary>
    /// Deserializes a list of type T . T must implement new() in order for the new object to be created (and then populated with deserialized data).
    /// </summary>
    public static List<T> DeserializeList<T>(this BinaryReader reader) where T : new()
    {
        int count = reader.ReadInt32(); // Read dictionary size
        var list = new List<T>(count);
        DeserializeList<T>(list,count,reader);
        return list;
    }

    /// <summary>
    /// Deserializes into an existing list of type T . T must implement new() in order for the new object to be created (and then populated with deserialized data).
    /// </summary>
    public static void DeserializeList<T>(this List<T> list, BinaryReader reader) where T : new()
    {
        int count = reader.ReadInt32(); // Read dictionary size
        DeserializeList<T>(list, count, reader);
    }
    /// <summary>
    /// Deserializes into an existing list of type T . T must implement new() in order for the new object to be created (and then populated with deserialized data).
    /// </summary>
    private static void DeserializeList<T>(List<T> populateList, int count, BinaryReader reader) where T : new()
    {
        for (int i = 0; i < count; i++)
        {
            T element = Deserialize<T>(reader); // Deserialize key
            populateList.Add(element);
        }
    }
    /// <summary>
    /// Serializes a dictionary with flexible key-value support.
    /// </summary>
    public static void SerializeDictionary<K, V>(this IDictionary<K, V> dictionary, BinaryWriter writer)
    {
        writer.Write(dictionary.Count); // Write dictionary size
        foreach (var kvp in dictionary)
        {
            Serialize(kvp.Key, writer); // Serialize key
            Serialize(kvp.Value, writer); // Serialize value
        }
    }

    /// <summary>
    /// Deserializes a dictionary with flexible key-value support.
    /// both K and V must implement new() in order for the new KeyValuePair to be created (and then populated with deserialized data).
    /// </summary>
    public static Dictionary<K, V> DeserializeDictionary<K, V>(this BinaryReader reader) where K:new() where V : new()
    {
        int count = reader.ReadInt32(); // Read dictionary size
        var dictionary = new Dictionary<K, V>(count);
        dictionary.DeserializeDictionary<K, V>(reader);
        return dictionary;
    }

    /// <summary>
    /// Deserializes into an existing dictionary with flexible key-value support.
    /// both K and V must implement new() in order for the new KeyValuePair to be created (and then populated with deserialized data).
    /// </summary>
    public static void DeserializeDictionary<K, V>(this IDictionary<K, V> dictionary,BinaryReader reader) where K : new() where V : new()
    {
        int count = reader.ReadInt32(); // Read dictionary size

        for (int i = 0; i < count; i++)
        {
            K key = Deserialize<K>(reader); // Deserialize key
            V value = Deserialize<V>(reader); // Deserialize value

            dictionary[key] = value;
        }
    }


    // ============================================
    // Generalized Serialize/Deserialize
    // these are used when loading collections, where element and or key types are not known in advance
    // They are less efficient than type-specified Serialize/Deserialize functions above.
    // Also, they will throw exceptions if the specified type is not handled.
    // currently handles: IBinarySaveLoad, int, long, string, float, double, bool
    // Users may implement IBinarySaveLoad on a class to make it work here, and in the collection serialization methods above.
    // ============================================

    /// <summary>
    /// Serializes any supported type, including IBinarySaveLoad and atomic types.
    /// </summary>
    private static void Serialize<T>(T value, BinaryWriter writer)
    {
        if (value is IBinarySaveLoad serializable)
        {
            serializable.Serialize(writer);
            return;
        }
        
        if (value is int)
        {
            writer.Write((int)(object)value);
        }
        else if (value is long)
        {
            writer.Write((long)(object)value);
        }
        else if (value is string)
        {
            writer.Write((string)(object)value);
        }
        else if (value is float)
        {
            writer.Write((float)(object)value);
        }
        else if (value is double)
        {
            writer.Write((double)(object)value);
        }
        else if (value is bool)
        {
            writer.Write((bool)(object)value);
        }
        else
        {
            throw new InvalidOperationException($"Unsupported type: {typeof(T)}. Implement IBinarySaveLoad for custom types.");
        }
    }

    /// <summary>
    /// Deserializes any supported type, including IBinarySaveLoad and atomic types.
    /// </summary>
    private static T Deserialize<T>(BinaryReader reader) where T:new()
    {
        
        if (typeof(IBinarySaveLoad).IsAssignableFrom(typeof(T)))
        {
            T tvalue = new T();
            ((IBinarySaveLoad)tvalue).Deserialize(reader);
            return tvalue;
        }
        T value;
        if (typeof(T) == typeof(int))
        {
            value = (T)(object)reader.ReadInt32();
        }
        else if (typeof(T) == typeof(long))
        {
            value = (T)(object)reader.ReadInt64();
        }
        else if (typeof(T) == typeof(string))
        {
            value = (T)(object)reader.ReadString();
        }
        else if (typeof(T) == typeof(float))
        {
            value = (T)(object)reader.ReadSingle();
        }
        else if (typeof(T) == typeof(double))
        {
            value = (T)(object)reader.ReadDouble();
        }
        else if (typeof(T) == typeof(bool))
        {
            value = (T)(object)reader.ReadBoolean();
        }
        else
        {
            throw new InvalidOperationException($"Unsupported type: {typeof(T)}. Implement IBinarySaveLoad for custom types.");
        }

        return (T)value;
    }
}



public class ResourceManager
{
    // Singleton instance
    private static ResourceManager instance = null;

    // Dictionary to map objects to their paths
    private Dictionary<UnityEngine.Object, string> objectToPathDict = new Dictionary<UnityEngine.Object, string>();
    private bool resourcesLoaded = false;  // Flag to ensure lazy loading

    // Private constructor to prevent instantiation
    private ResourceManager() { LoadResources(); }

    // Public static property to access the singleton instance
    public static ResourceManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ResourceManager();
            }
            return instance;
        }
    }

    // Load resources lazily, including subfolders
    private void LoadResources()
    {
        if (resourcesLoaded) return;

        try
        {
            string resourcesPath = "Assets/Resources"; // Path to the Resources folder

            // Recursively get all paths to assets in the Resources folder
            string[] directories = Directory.GetDirectories(resourcesPath, "*", SearchOption.AllDirectories);

            // Traverse through each directory and load the assets
            foreach (string directory in directories)
            {
                string relativePath = directory.Replace(resourcesPath + Path.DirectorySeparatorChar, "").Replace("\\", "/");
                if (relativePath.Contains("Editor")) continue; // Skip editor-specific folders

                // Get all asset files in the current directory
                List<string> files = new List<string>(Directory.GetFiles(directory, "*.prefab"));
                files.AddRange(Directory.GetFiles(directory, "*.asset")); // Add more extensions as needed

                foreach (var file in files)
                {
                    string assetPath = file.Replace(resourcesPath + Path.DirectorySeparatorChar, "").Replace("\\", "/").Replace(".prefab", "");
                    var loadedObject = UnityEngine.Resources.Load<UnityEngine.Object>(assetPath);

                    if (loadedObject != null)
                    {
                        objectToPathDict[loadedObject] = assetPath; // Map object to path
                    }
                    else
                    {
                        UnityEngine.Debug.LogWarning($"Failed to load asset at path: {assetPath}");
                    }
                }
            }

            resourcesLoaded = true;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to load resources from the 'Resources' folder.", ex);
        }
    }

    // Get the path of an object
    public static string GetPathOfObject(UnityEngine.Object obj)
    {
        if (Instance.objectToPathDict.TryGetValue(obj, out string path))
        {
            return path;
        }
        else
        {
            return string.Empty;
        }
    }
    /*
    // Load a resource by its path
    public static T LoadResource<T>(string path) where T : UnityEngine.Object
    {
        // Ensure resources are loaded before accessing them
        Instance.LoadResources();

        T resource = Resources.Load<T>(path);

        if (resource == null)
        {
            throw new InvalidOperationException($"Failed to load resource of type '{typeof(T).Name}' at path '{path}'. Resource not found.");
        }

        return resource;
    }*/
}
