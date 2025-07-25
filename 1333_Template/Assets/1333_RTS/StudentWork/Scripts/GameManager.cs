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

    private void Awake()
    {
        if (!ValidateReferences())
        {
            Debug.LogError("Missing required references");
            enabled = false;
            return;
        }

        gridManager.InitializeGrid();

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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RandomizeAndPathfind();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            AudioManager.instance.ChangeMusic();
        }
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

    public List<GridNode> GetNeighbors(GridNode node)
    {
        List<GridNode> neighbors = new();

        Vector2Int coords = gridManager.GetCoordinatesFromNode(node);
        int x = coords.x;
        int y = coords.y;

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (Mathf.Abs(dx) + Mathf.Abs(dy) != 1) continue; // no diagonals

                int nx = x + dx;
                int ny = y + dy;

                if (nx >= 0 && nx < gridManager.GridSettings.GridSizeX &&
                    ny >= 0 && ny < gridManager.GridSettings.GridSizeY)
                {
                    neighbors.Add(gridManager.GetNode(nx, ny));
                }
            }
        }

        return neighbors;
    }
}
