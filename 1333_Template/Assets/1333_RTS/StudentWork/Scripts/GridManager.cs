using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Android.Types;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private GridSettings gridSettings;
    [SerializeField] private List<TerrainType> chosenTerrain;
    public GridSettings GridSettings => gridSettings;

    public GridNode[,] gridNodes;

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

                TerrainType terrain = chosenTerrain[0];

                GridNode node = new GridNode
                {
                    Name = $"{terrain.TerrainName}_{(x + gridSettings.GridSizeX * x) + y}",
                    WorldPosition = worldPos,
                    Walkable = terrain.Walkable,
                    Weight = terrain.Weight
                };
                gridNodes[x, y] = node;
            }
        }
        IsInitialized = true;

    }

    private void OnDrawGizmos()
    {
        if (gridNodes == null || gridSettings == null) return;

        for (int x = 0; x < gridSettings.GridSizeX; x++)
        {
            for (int y = 0;y < gridSettings.GridSizeY; y++)
            {
                GridNode node = gridNodes[x, y];
                Gizmos.color = node.Walkable ? Color.green : Color.red;
                Gizmos.DrawWireCube(node.WorldPosition, Vector3.one * GridSettings.NodeSize * 0.9f);
            }
        }
    }
}
