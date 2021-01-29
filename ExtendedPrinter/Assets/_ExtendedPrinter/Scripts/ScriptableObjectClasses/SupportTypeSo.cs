using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SupportType", menuName = "ScriptableObjects/SupportType", order = 1)]
public class SupportTypeSo : ScriptableObject
{
   public  SupportType SupportType;
}

[Serializable, Flags]
public enum SupportType : int
{
    None = 0,
    Buildeplate = 1,
    Everywhere = 2
}