using System;
using UnityEngine;

[CreateAssetMenu(fileName = "AdhesionType", menuName = "ScriptableObjects/AdhesionType", order = 1)]
public class AdhesionTypeSo : ScriptableObject
{
    public AdhesionType AdhesionType;
}

[Serializable]
public enum AdhesionType : int
{
    None,
    Raft,
    Brim
}