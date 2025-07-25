using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Android.Types;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager instance;

    [SerializeField] private GridSettings gridSettings;
    [SerializeField] private List<TerrainType> chosenTerrain;
    public GridSettings GridSettings => gridSettings;

    public GridNode[,] gridNodes;

    private List<GridNode> AllNodes = new();

    public bool IsInitialized { get; private set; } = false;

    private void Awake()
    {
        instance = this;
    }

    public void InitializeGrid()
    {
        gridNodes = new GridNode[gridSettings.GridSizeX, gridSettings.GridSizeY];
        for (int x = 0; x < gridSettings.GridSizeX; x++)
        {
            for (int y = 0; y < gridSettings.GridSizeY; y++)
            {
                Vector3 worldPos = gridSettings.UseXZPlane
                    ? new Vector3(x, 0, y) * gridSettings.NodeSize
                    : new Vector3(x, y, 0) * gridSettings.NodeSize;

                TerrainType terrain = chosenTerrain[UnityEngine.Random.Range(0, chosenTerrain.Count)];

                GridNode node = new GridNode
                {
                    Name = $"Cell_{(x + gridSettings.GridSizeX * x) + y}",
                    WorldPosition = worldPos,
                    TerrainType = terrain,
                    Walkable = terrain.Walkable,
                    Weight = terrain.Weight,
                    GizmoColor = terrain.GizmoColor
                };
                gridNodes[x, y] = node;
            }
        }
        IsInitialized = true;

    }

    private void PopulateDebugList()
    {
        AllNodes.Clear();

        for(int x = 0; x < gridSettings.GridSizeX; x++)
        {
            for(int y = 0; y < gridSettings.GridSizeY; y++)
            {
                GridNode node = gridNodes[x, y];
                AllNodes.Add(new GridNode
                {
                    Name = $"Cell_{x}_{y}",
                    WorldPosition = node.WorldPosition,
                    Walkable = node.Walkable,
                    Weight = node.Weight,
                    GizmoColor = node.GizmoColor
                });
            }
        }
    }

    public GridNode GetNode(int x, int y)
    {
        if (x < 0 || x >= gridSettings.GridSizeX || y < 0 || y >= gridSettings.GridSizeY)
            throw new System.IndexOutOfRangeException("Grid node indices out of range.");

        return gridNodes[x, y];
    }

    public void SetWalkable(int x, int y, bool isWalkable)
    {
        gridNodes[x, y].Walkable = isWalkable;
    }

    private void OnDrawGizmos()
    {
        if (gridNodes == null || gridSettings == null) return;

        for (int x = 0; x < gridSettings.GridSizeX; x++)
        {
            for (int y = 0;y < gridSettings.GridSizeY; y++)
            {
                GridNode node = gridNodes[x, y];
                Gizmos.color = node.GizmoColor;
                Gizmos.DrawWireCube(node.WorldPosition, Vector3.one * GridSettings.NodeSize * 0.9f);
            }
        }

        PopulateDebugList();
    }

    /*public GridNode GetNodeFromWorldPosition(Vector3 position)
    {
        int x = gridSettings.UseXZPlane
            ? Mathf.FloorToInt(position.x / gridSettings.NodeSize)
            : Mathf.FloorToInt(position.x / gridSettings.NodeSize);

        int y = gridSettings.UseXZPlane
            ? Mathf.FloorToInt(position.z / gridSettings.NodeSize)
            : Mathf.FloorToInt(position.z / gridSettings.NodeSize);

        x = Mathf.Clamp(x, 0, gridSettings.GridSizeX - 1);
        y = Mathf.Clamp(y, 0, gridSettings.GridSizeY - 1);

        return GetNode(x, y);
    }*/

    public GridNode GetNodeFromWorldPosition(Vector3 position)
    {
        int x = Mathf.FloorToInt(position.x / gridSettings.NodeSize);
        int y = Mathf.FloorToInt(position.z / gridSettings.NodeSize); // because we use XZ plane

        x = Mathf.Clamp(x, 0, gridSettings.GridSizeX - 1);
        y = Mathf.Clamp(y, 0, gridSettings.GridSizeY - 1);

        return GetNode(x, y);
    }

    public void RandomizeTerrain()
    {

        int gridSizeX = gridSettings.GridSizeX;
        int gridSizeY = gridSettings.GridSizeY;

        for(int x = 0; x < gridSizeX; x++)
        {
            for(int y = 0; y < gridSizeY; y++)
            {
                TerrainType randomTerrain = gridSettings.TerrainTypes[UnityEngine.Random.Range(0, GridSettings.TerrainTypes.Length)];
                SetTerrainType(x, y, randomTerrain);
            }
        }

    }

    private void SetTerrainType(int x, int y, TerrainType terrainType)
    {
        if(!IsValidCoordinate(x, y)) return;

        GridNode node = gridNodes[x, y];
        node.TerrainType = terrainType;
        node.Walkable = terrainType.Walkable;
        node.Weight = terrainType.Weight;
        node.GizmoColor = terrainType.GizmoColor;
        gridNodes[x, y] = node;
    }

    private bool IsValidCoordinate(int x, int y)
    {
        return x >= 0 && x < gridSettings.GridSizeX && y >= 0 && y < gridSettings.GridSizeY;
    }

    /*public Vector2Int GetCoordinatesFromNode(GridNode node)
    {
        for (int x = 0; x < gridSettings.GridSizeX; x++)
        {
            for (int y = 0; y < gridSettings.GridSizeY; y++)
            {
                if (gridNodes[x, y].WorldPosition == node.WorldPosition)
                    return new Vector2Int(x, y);
            }
        }
        return Vector2Int.zero; // Or throw exception if not found
    }*/

    public Vector2Int GetCoordinatesFromNode(GridNode node)
    {
        int x = Mathf.RoundToInt(node.WorldPosition.x / gridSettings.NodeSize);
        int y = Mathf.RoundToInt(node.WorldPosition.z / gridSettings.NodeSize);

        x = Mathf.Clamp(x, 0, gridSettings.GridSizeX - 1);
        y = Mathf.Clamp(y, 0, gridSettings.GridSizeY - 1);

        return new Vector2Int(x, y);
    }

    public bool CanPlaceBuilding(int startX, int startY, BuildingData building)
    {
        //Debug.Log($"Trying to place building '{building.BuildingName}' at {startX}, {startY} with size {building.Width}x{building.Length}");

        if (startX < 0 || startY < 0 ||
            startX + building.Width > gridSettings.GridSizeX ||
            startY + building.Length > gridSettings.GridSizeY)
        {
            Debug.LogWarning("Placement out of bounds: Building footprint extends outside grid.");
            return false;
        }

        for (int x = 0; x < building.Width; x++)
        {
            for (int y = 0; y < building.Length; y++)
            {
                int checkX = startX + x;
                int checkY = startY + y;

                if (checkX < 0 || checkY < 0 || checkX >= gridSettings.GridSizeX || checkY >= gridSettings.GridSizeY)
                {
                    Debug.LogWarning($"Placement failed: Node ({checkX}, {checkY}) is out of bounds.");
                    return false;
                }

                GridNode node = gridNodes[checkX, checkY];

                if (!node.Walkable || node.BuildingData != null)
                {
                    Debug.LogWarning($"Placement failed: Node ({checkX}, {checkY}) is occupied or not walkable.");
                    return false;
                }
            }
        }

        //Debug.Log("Placement is valid.");
        return true;
    }

    public IEnumerable<GridNode> GetAllNodes()
    {
        for (int x = 0; x < GridSettings.GridSizeX; x++)
        {
            for (int y = 0; y < GridSettings.GridSizeY; y++)
            {
                yield return gridNodes[x, y];
            }
        }
    }

    public bool IsWithinBounds(int x, int y)
    {
        return x >= 0 && x < gridSettings.GridSizeX && y >= 0 && y < gridSettings.GridSizeY;
    }

    public List<GridNode> GetNeighbors(GridNode node)
    {
        List<GridNode> neighbors = new();
        Vector2Int coord = GetCoordinatesFromNode(node);

        int x = coord.x;
        int y = coord.y;

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;

                int checkX = x + dx;
                int checkY = y + dy;

                if (IsWithinBounds(checkX, checkY))
                {
                    neighbors.Add(gridNodes[checkX, checkY]);
                }
            }
        }

        return neighbors;
    }
}
