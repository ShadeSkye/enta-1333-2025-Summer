using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private UnitManager unitManager;
    [SerializeField] private Pathfinder pathfinder;
    [SerializeField] private Transform startMarker;
    [SerializeField] private Transform endMarker;
    [SerializeField] private LineRenderer pathLine;
    [SerializeField] private float markerHeight = 0.5f;
    [SerializeField] private BuildingData castleBuilding;

    private void Awake()
    {
        if (!ValidateReferences())
        {
            Debug.LogError("Missing required references");
            enabled = false;
            return;
        }

        gridManager.InitializeGrid();
        PlaceStartingCastle();

        AudioManager.instance.PlayCalmMusic();

    }

    private bool ValidateReferences()
    {
        if (!gridManager || !pathfinder || !pathLine)
        {
            return false; 
        }

        return true;
    }

    private void Update()
    {

    }

    private void RandomizeAndPathfind()
    {
        RandomizeAll();

        var path = pathfinder.FindPath(gridManager.GetNodeFromWorldPosition(startMarker.position), gridManager.GetNodeFromWorldPosition(endMarker.position));
        string message = $"Path found: {path.Count} steps. Start at {startMarker}, end at {endMarker}.";
        foreach(var p in path)
        {
            message += $" > {p.WorldPosition}";
        }

        message += $" > end at {endMarker}";
    }

    private void RandomizeAll()
    {
        Debug.Log("Randomising All");
        gridManager.RandomizeTerrain();
        RandomizeMarkers();
    }

    private void RandomizeMarkers()
    {
        int gridSizeX = gridManager.GridSettings.GridSizeX;
        int gridSizeY = gridManager.GridSettings.GridSizeY;
        float nodeSize = gridManager.GridSettings.NodeSize;

        int StartX = Random.Range(0, gridSizeX);
        int StartY = Random.Range(0, gridSizeY);
        startMarker.position = new Vector3(StartX * nodeSize, markerHeight, StartY * nodeSize);

        int endX, endY;
        do
        {
                endX = Random.Range(0, gridSizeX);
                endY = Random.Range(0, gridSizeY);
        }while(endX == StartX && endY == StartY);

        endMarker.position = new Vector3(endX * nodeSize, markerHeight, endY * nodeSize);
    }

    private void PlaceStartingCastle()
    {
        int centerX = gridManager.GridSettings.GridSizeX / 2 - (castleBuilding.Width / 2);
        int centerY = gridManager.GridSettings.GridSizeY / 2 - (castleBuilding.Length / 2);

        if (!gridManager.CanPlaceBuilding(centerX, centerY, castleBuilding))
        {
            Debug.LogError("Failed to place starting castle: position not valid.");
            return;
        }

        // Mark nodes as occupied
        for (int x = 0; x < castleBuilding.Width; x++)
        {
            for (int y = 0; y < castleBuilding.Length; y++)
            {
                int nodeX = centerX + x;
                int nodeY = centerY + y;

                GridNode node = gridManager.GetNode(nodeX, nodeY);
                node.BuildingData = castleBuilding;
                node.Walkable = false;
                gridManager.gridNodes[nodeX, nodeY] = node;
            }
        }

        Vector3 baseWorldPos = gridManager.GetNode(centerX, centerY).WorldPosition;

        float offsetX = (castleBuilding.Width * gridManager.GridSettings.NodeSize) / 2f - (gridManager.GridSettings.NodeSize / 2f);
        float offsetZ = (castleBuilding.Length * gridManager.GridSettings.NodeSize) / 2f - (gridManager.GridSettings.NodeSize / 2f);

        Vector3 spawnPos = baseWorldPos + new Vector3(offsetX, -1, offsetZ);

        GameObject castleGO = Instantiate(castleBuilding.BuildingPrefab, spawnPos, Quaternion.Euler(-90f, 90f, 0f));
        castleGO.transform.localScale = Vector3.one;

        Camera.main.transform.position = spawnPos + new Vector3(0, 20f, -20f); // Offset to look from above
        Camera.main.transform.LookAt(castleGO.transform);

        var buildingInstance = castleGO.GetComponent<BuildingInstance>();
        if (buildingInstance != null)
        {
            buildingInstance.Initialize(castleBuilding);
        }

        // Optional: Tag for win/loss detection
        castleGO.tag = "Castle";

        // Optional: Set up spawner if applicable
        if (castleBuilding.CanSpawnUnits)
        {
            var spawner = castleGO.GetComponent<BarracksSpawner>();
            if (spawner != null)
            {
                spawner.buildingData = castleBuilding;
                spawner.gridManager = gridManager;
                spawner.pathfinder = pathfinder;
                spawner.armyManager = FindAnyObjectByType<PlayerArmyManager>()?.ArmyManagerRef;
            }
            else
            {
                Debug.LogWarning("Castle building can spawn units but has no BarracksSpawner component.");
            }
        }
    }
}
