using System.Collections;
using System.Collections.Generic;
using Unity.Android.Types;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private GridSettings gridSettings;
    [SerializeField] private TerrainType defaultTerrainType;
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

                GridNode node = new GridNode
                {
                    Name = $"Cell_{(x + gridSettings.GridSizeX * x) + y}",
                    WorldPosition = worldPos,
                    Walkable = true,
                    Weight = 1
                };
                gridNodes[x, y] = node;
            }
        }
        IsInitialized = true;

    }

    /*
    public GridNode GetNode(int x, int y)
    {

    }

    public void SetWalkable(int x, int y, bool walkable)
    {

    } */

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
