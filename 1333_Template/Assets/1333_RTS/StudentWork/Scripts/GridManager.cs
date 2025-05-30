using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Android.Types;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private GridSettings gridSettings;
    [SerializeField] private List<TerrainType> chosenTerrain;
    public GridSettings GridSettings => gridSettings;

    public GridNode[,] gridNodes;

    private List<GridNode> AllNodes = new();

    public bool IsInitialized { get; private set; } = false;

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

    public GridNode getNodeFromWorldPosition(Vector3 position)
    {
        int x = gridSettings.UseXZPlane ? Mathf.RoundToInt(position.x / gridSettings.NodeSize) : Mathf.RoundToInt(position.x / gridSettings.NodeSize);
        int y = gridSettings.UseXZPlane ? Mathf.RoundToInt(position.z / gridSettings.NodeSize) : Mathf.RoundToInt(position.z / gridSettings.NodeSize);

        x = Mathf.Clamp(x, 0, gridSettings.GridSizeX - 1);
        y = Mathf.Clamp(y, 0, gridSettings .GridSizeY - 1);

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
}
