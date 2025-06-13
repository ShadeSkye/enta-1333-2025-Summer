using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Serializable class oairs the UnitType SO with a prefab
/// </summary>
[System.Serializable]
public class UnitTypePrefab
{
    /// <summary>
    /// references the unit data
    /// </summary>
    public UnitType unitType;
    /// <summary>
    /// references the unit prefab
    /// </summary>
    public GameObject unitPrefab;

}
