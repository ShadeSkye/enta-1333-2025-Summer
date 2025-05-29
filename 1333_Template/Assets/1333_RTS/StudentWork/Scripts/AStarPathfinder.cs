using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinder : PathfindingAlgorithm
{

    private int Heuristic(GridNode a, GridNode b)
    {
        float dx = Mathf.Abs(a.WorldPosition.x - b.WorldPosition.x);
        float dz = Mathf.Abs(a.WorldPosition.z - b.WorldPosition.z);
        return Mathf.RoundToInt(dx + dz);
    }

    private List<GridNode> GetNeighbors(GridManager gridManager, GridNode node)
    {
        List<GridNode> neighbors = new List<GridNode>();
        int gridSizeX = gridManager.GridSettings.GridSizeX;
        int gridSizeY = gridManager.GridSettings.GridSizeY;
        float nodeSize = gridManager.GridSettings.NodeSize;
        int nodeX = Mathf.RoundToInt(node.WorldPosition.x / nodeSize);
        int nodeY = Mathf.RoundToInt(node.WorldPosition.y / nodeSize);
        if(nodeY + 1 < gridSizeY) neighbors.Add(gridManager.GetNode(nodeX, nodeY + 1));
        if(nodeY - 1 >= 0) neighbors.Add(gridManager.GetNode(nodeX, nodeY - 1));
        if (nodeX + 1 < gridSizeY) neighbors.Add(gridManager.GetNode(nodeX + 1, nodeY));
        if (nodeX - 1 >= 0) neighbors.Add(gridManager.GetNode(nodeX - 1, nodeY));
        return neighbors;
    }

    private bool IsAreaWalkable(GridManager gridManager, GridNode node)
    {
        float nodeSize = gridManager.GridSettings.NodeSize;
        int x = Mathf.RoundToInt(node.WorldPosition.x / nodeSize);
        int y = Mathf.RoundToInt(node.WorldPosition.z / nodeSize);
        return true;
    }


    public List<GridNode> FindPath(GridManager gridManager, GridNode start, GridNode end)
    {
        // preparing data structures
        List<GridNode> openSet = new List<GridNode>();
        Dictionary<GridNode, int> costSoFar = new Dictionary<GridNode, int>();
        Dictionary<GridNode, int> estimatedTotalCost = new Dictionary<GridNode, int>();
        Dictionary<GridNode, GridNode> cameFrom = new Dictionary<GridNode, GridNode>();

        // initializing algorithm
        openSet.Add(start);
        costSoFar[start] = 0;
        estimatedTotalCost[start] = Heuristic(start, end);
        cameFrom[start] = start;

        // loop searches nodes that have yet to be explored
        while (openSet.Count > 0)
        {
            GridNode current = openSet[0];
            foreach(var node in openSet)
            {
                if (estimatedTotalCost[node] < estimatedTotalCost[current])
                    current = node;
            }

            // stops if end node is reached
            if (current.Equals(end))
                break;

            openSet.Remove(current);

            // explores sorrounding grids
            foreach(GridNode neighbor in GetNeighbors(gridManager, current))
            {
                if (!IsAreaWalkable(gridManager, neighbor))
                    continue;

                int newCost = costSoFar[current] + neighbor.Weight;

                if(!costSoFar.ContainsKey(neighbor) || newCost < costSoFar[neighbor])
                {
                    costSoFar[neighbor] = newCost;
                    estimatedTotalCost[neighbor] = newCost + Heuristic(neighbor, end);
                    cameFrom[neighbor] = current;

                    if(!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        List<GridNode> path = new List<GridNode>();
        GridNode pathNode = end;

        if(!cameFrom.ContainsKey(end))
            return path;

        while (!pathNode.Equals(start))
        {
            path.Add(pathNode);
            pathNode = cameFrom[pathNode];
        }
        path.Add(start);

        path.Reverse();

        return path;
    }

    public override List<GridNode> FindPath(GridNode start, GridNode end)
    {
        throw new System.NotImplementedException();
    } 

    public override List<GridNode> FindPath(Vector3 start, Vector3 end)
    {
        throw new System.NotImplementedException();
    }
}
