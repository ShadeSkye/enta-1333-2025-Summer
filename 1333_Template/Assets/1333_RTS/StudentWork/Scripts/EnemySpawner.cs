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

    [Header("Wave Settings")]
    [SerializeField] private float minTimeBetweenWaves = 5f; //seconds
    [SerializeField] private float maxTimeBetweenWaves = 10f;
    [SerializeField] private int minEnemiesPerWave = 3;
    [SerializeField] private int maxEnemiesPerWave = 6;

    [SerializeField] private float spawnHeightOffset = 0.5f;

    private int aliveEnemies = 0;

    public int AliveEnemies => aliveEnemies;

    private void Start()
    {
        StartCoroutine(WaveSpawnerRoutine());
    }

    private IEnumerator WaveSpawnerRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minTimeBetweenWaves, maxTimeBetweenWaves);
            yield return new WaitForSeconds(waitTime);

            int enemyCount = Random.Range(minEnemiesPerWave, maxEnemiesPerWave + 1);
            Vector2Int spawnDirection = GetRandomEdge(); // Which side of the grid
            List<GridNode> spawnNodes = GetSpawnNodesAlongEdge(spawnDirection, enemyCount);

            for (int i = 0; i < enemyCount && i < spawnNodes.Count; i++)
            {
                SpawnEnemyAtNode(spawnNodes[i]);
            }

            Debug.Log($"Wave spawned with {enemyCount} enemies from direction {spawnDirection}");
        }
    }

    private void SpawnEnemyAtNode(GridNode node)
    {
        if (!node.Walkable)
            return;

        if (enemyUnitTypes == null || enemyUnitTypes.Count == 0)
        {
            Debug.LogError("Enemy types list is empty.");
            return;
        }

        UnitType selectedEnemyType = enemyUnitTypes[Random.Range(0, enemyUnitTypes.Count)];
        Vector3 spawnPos = node.WorldPosition + Vector3.up * spawnHeightOffset;

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

        FindFirstObjectByType<UnitManager>()?.RegisterUnit(unit);

        if (aliveEnemies <= 0)
        {
            AudioManager.instance.ChangeMusic();
        }

        aliveEnemies++;

        unit.OnDeath += HandleEnemyDeath;
    }

    /// <summary>
    /// Randomly selects a side: left (0,-1), right (0,1), top (-1,0), bottom (1,0)
    /// </summary>
    private Vector2Int GetRandomEdge()
    {
        int rand = Random.Range(0, 4);
        return rand switch
        {
            0 => Vector2Int.left,   // Left
            1 => Vector2Int.right,  // Right
            2 => Vector2Int.up,     // Top
            _ => Vector2Int.down    // Bottom
        };
    }

    /// <summary>
    /// Gets walkable nodes along a specific edge (row or column).
    /// </summary>
    private List<GridNode> GetSpawnNodesAlongEdge(Vector2Int edge, int count)
    {
        List<GridNode> edgeNodes = new();

        int sizeX = gridManager.GridSettings.GridSizeX;
        int sizeY = gridManager.GridSettings.GridSizeY;

        if (edge == Vector2Int.left)
        {
            for (int y = 0; y < sizeY; y++)
                TryAddNode(0, y, edgeNodes);
        }
        else if (edge == Vector2Int.right)
        {
            for (int y = 0; y < sizeY; y++)
                TryAddNode(sizeX - 1, y, edgeNodes);
        }
        else if (edge == Vector2Int.up)
        {
            for (int x = 0; x < sizeX; x++)
                TryAddNode(x, sizeY - 1, edgeNodes);
        }
        else if (edge == Vector2Int.down)
        {
            for (int x = 0; x < sizeX; x++)
                TryAddNode(x, 0, edgeNodes);
        }

        // Shuffle list to get random positions along the edge
        for (int i = 0; i < edgeNodes.Count; i++)
        {
            GridNode temp = edgeNodes[i];
            int randomIndex = Random.Range(i, edgeNodes.Count);
            edgeNodes[i] = edgeNodes[randomIndex];
            edgeNodes[randomIndex] = temp;
        }

        return edgeNodes.GetRange(0, Mathf.Min(count, edgeNodes.Count));
    }

    private void TryAddNode(int x, int y, List<GridNode> list)
    {
        GridNode node = gridManager.GetNode(x, y);
        if (node.Walkable)
            list.Add(node);
    }

    private void HandleEnemyDeath(UnitInstance unit)
    {
        aliveEnemies--;

        // Unsubscribe just in case
        unit.OnDeath -= HandleEnemyDeath;

        if (aliveEnemies <= 0)
        {
            AudioManager.instance.ChangeMusic();
            Debug.Log("All enemies in the wave defeated.");
        }
    }
}
