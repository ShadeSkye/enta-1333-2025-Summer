using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingTypes", menuName = "Game/BuildingTypes")]
public class BuildingTypes : ScriptableObject
{
    public List<BuildingData> Buildings = new();
}

[System.Serializable]
public class BuildingData
{
    public string BuildingName;
    public Sprite Icon;
    public int Cost;
    public int Width;
    public int Length;
    public int Height;
    public int Health;
    public int Capacity;
    public GameObject BuildingPrefab;
    public GameObject BuildingPrefabGhost;
    public bool CanSpawnUnits;
    public List<UnitTypePrefab> SpawnableUnits;
    public float SpawnCooldown = 2f;
}
