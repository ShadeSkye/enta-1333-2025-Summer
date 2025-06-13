using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ArmyComp", menuName = "Game/ArmyComp")]
public class ArmyComposition : ScriptableObject
{
    [System.Serializable]
    public class UnitEntry
    {
        // The type+prefab pairing for this entry.
        public UnitTypePrefab unitTypePrefab;
        // How many of this type in the army.
        public int count = 1;
    }

    // List of all unit entries in this army.
    public List<UnitEntry> units = new();
}
