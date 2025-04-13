using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SOTest", menuName = "GameVersionData/SOTest")]
public class SOTest : ScriptableObject
{

    public int anInt;
    //[ContextMenu("Reset")]    
    public void Reset()
    { anInt = 69;  Debug.Log("Reset called"); }
}
