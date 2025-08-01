using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarracksSpawner : MonoBehaviour
{
    public BuildingData buildingData;
    public GridManager gridManager;
    public Pathfinder pathfinder;
    public ArmyManager armyManager;

    private float spawnTimer = 0f;

    private void Update()
    {
        if (buildingData == null || !buildingData.CanSpawnUnits || buildingData.SpawnableUnits.Count == 0)
            return;
    }

    public void SpawnUnit(UnitTypePrefab unitTypePrefab)
    {
        if (buildingData == null || !buildingData.CanSpawnUnits)
            return;

        if (!PopulationManager.Instance.AddUnits(1))
        {
            Debug.LogWarning("Not enough population capacity to spawn unit!");
            return;
        }

        Vector3 spawnPos = GetClosestFreeNode(transform.position);
        if (spawnPos == Vector3.zero)
        {
            Debug.LogWarning("No free node to spawn unit!");
            return;
        }

        Quaternion spawnRotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
        GameObject go = Instantiate(unitTypePrefab.unitPrefab, spawnPos, spawnRotation);

        UnitInstance unit = go.GetComponent<UnitInstance>();
        if (unit == null)
        {
            Debug.LogError("Spawned unit prefab missing UnitInstance component!");
            Destroy(go);
            return;
        }

        unit.Initialize(pathfinder, unitTypePrefab.unitType);
        unit.PlayerTeam = true;
        //unit.SetTeamMaterial(armyManager.TeamMaterial);
        armyManager.Units.Add(unit);

        FindFirstObjectByType<UnitManager>()?.RegisterUnit(unit);

        Debug.Log($"Spawned unit {unitTypePrefab.unitType.name} at {spawnPos}");
    }

    private Vector3 GetClosestFreeNode(Vector3 origin)
    {
        float bestDist = float.MaxValue;
        GridNode bestNode = default;
        bool found = false;

        foreach (var node in gridManager.GetAllNodes())
        {
            if (node.Walkable && node.BuildingData == null && Vector3.Distance(origin, node.WorldPosition) < bestDist)
            {
                bestDist = Vector3.Distance(origin, node.WorldPosition);
                bestNode = node;
                found = true;
            }
        }

        return found ? bestNode.WorldPosition : Vector3.zero;
    }

    public void SpawnUnitByType(UnitType type)
    {
        var prefab = buildingData.SpawnableUnits.Find(u => u.unitType == type);
        if (prefab != null)
        {
            SpawnUnit(prefab);
        }
        else
        {
            Debug.LogWarning($"Could not find prefab for unit type: {type.name}");
        }
    }
}
