
using UnityEngine;
using EyE.Collections;
using EyE.Traits;

[System.Serializable]
public class StringByInt : SerializableDictionary<int, string>
{ }
public class monotest : MonoBehaviour
{
    //public SerializableDictionary<int, string> dic = new SerializableDictionary<int, string>();
    public StringByInt dic2 = new StringByInt();
    private void OnEnable()
    {
        Debug.Log("looking for base trait, found:" + AllTraits.Wealth.Name);
       // Debug.Log("looking for modded trait, found Buoyancy: " + AllTraits.TryGetByName("Buoyancy", out TraitDefinition ignored) );
    }

}
