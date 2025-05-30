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
    /*[SerializeField] Vector2Int startMarker;
    [SerializeField] Vector2Int endMarker;*/
    [SerializeField] private LineRenderer pathLine;
    [SerializeField] private float markerHeight = 0.5f;

    private void Awake()
    {
        if (!ValidateReferences())
        {
            Debug.LogError("Missing required references");
            enabled = false;
            return;
        }

        gridManager.InitializeGrid();
    }

    private bool ValidateReferences()
    {
        //if (!gridManager || !pathfinder || !startMarker || !endMarker || !pathLine)
        if (!gridManager || !pathfinder || !pathLine)
        {
            return false; 
        }

        return true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RandomizeAndPathfind();
        }
    }

    private void RandomizeAndPathfind()
    {
        RandomizeAll();


        var path = pathfinder.FindPath(gridManager.getNodeFromWorldPosition(startMarker.position), gridManager.getNodeFromWorldPosition(endMarker.position));
        //Debug.Log(path.Count);
        string message = $"Path found: {path.Count} steps. Start at {startMarker}, end at {endMarker}.";
        foreach(var p in path)
        {
            message += $" > {p.WorldPosition}";
        }

        message += $" > end at {endMarker}";
        Debug.Log(message);
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
}
