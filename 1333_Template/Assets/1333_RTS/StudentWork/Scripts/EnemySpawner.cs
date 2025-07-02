using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GridManager gridManager;
    [SerializeField] private Pathfinder pathfinder;
    [SerializeField] private List<UnitType> enemyUnitTypes;
    [SerializeField] private Material enemyTeamMaterial;
    [SerializeField] private Transform enemyParent;

    [Header("Spawn Settings")]
    [SerializeField] private Vector2Int spawnAreaStart = new Vector2Int(0, 0); // top-left corner
    [SerializeField] private int spawnAreaWidth = 3;
    [SerializeField] private int spawnAreaHeight = 3;
    [SerializeField] private float spawnHeightOffset = 0.5f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        if (!gridManager.IsInitialized)
        {
            Debug.LogError("Grid not initialized yet.");
            return;
        }

        GridNode? node = GetRandomWalkableNodeInArea();
        if (!node.HasValue)
        {
            Debug.LogWarning("Couldn't find a valid walkable node in spawn area.");
            return;
        }

        if (enemyUnitTypes == null || enemyUnitTypes.Count == 0)
        {
            Debug.LogError("Enemy types list is empty.");
            return;
        }

        UnitType selectedEnemyType = enemyUnitTypes[Random.Range(0, enemyUnitTypes.Count)];
        Vector3 spawnPos = node.Value.WorldPosition + Vector3.up * spawnHeightOffset;

        GameObject enemyGO = Instantiate(selectedEnemyType.UnitPrefab, spawnPos, Quaternion.identity, enemyParent);
        UnitInstance unit = enemyGO.GetComponent<UnitInstance>();

        if (unit == null)
        {
            Debug.LogError("Spawned prefab does not have a UnitInstance component!");
            return;
        }

        unit.Initialize(pathfinder, selectedEnemyType);
        unit.SetTeamMaterial(enemyTeamMaterial);
        unit.PlayerTeam = false;

        FindObjectOfType<UnitManager>()?.RegisterUnit(unit);

        Debug.Log($"Spawned {selectedEnemyType.name} at node {node.Value.WorldPosition}");
    }

    private GridNode? GetRandomWalkableNodeInArea()
    {
        List<GridNode> possibleNodes = new();

        for (int x = spawnAreaStart.x; x < spawnAreaStart.x + spawnAreaWidth; x++)
        {
            for (int y = spawnAreaStart.y; y < spawnAreaStart.y + spawnAreaHeight; y++)
            {
                if (x >= 0 && y >= 0 && x < gridManager.GridSettings.GridSizeX && y < gridManager.GridSettings.GridSizeY)
                {
                    GridNode node = gridManager.GetNode(x, y);
                    if (node.Walkable)
                        possibleNodes.Add(node);
                }
            }
        }

        if (possibleNodes.Count == 0)
            return null;

        return possibleNodes[Random.Range(0, possibleNodes.Count)];
    }
}
