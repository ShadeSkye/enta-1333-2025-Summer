using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{

    [SerializeField] private AStarPathfinder aStarPathfinder;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private bool drawLastPathGizmos = true;
    [SerializeField] private Color pathGizmoColor = Color.cyan;

    private List<GridNode> lastPath = new();
    public List<GridNode> FindPath(GridNode start, GridNode end)
    {
        List<GridNode> path;

        //Debug.Log(aStarPathfinder);

        path = aStarPathfinder.FindPath(gridManager, start, end);

        if(drawLastPathGizmos) lastPath = path;
        return path;
    }

    public List<GridNode> FindPath(Vector3 start, Vector3 end)
    {
        GridNode startNode = gridManager.GetNodeFromWorldPosition(start);
        GridNode endNode = gridManager.GetNodeFromWorldPosition(end);
        return FindPath(startNode, endNode);
    }

    private void OnDrawGizmos()
    {
        if(!drawLastPathGizmos || lastPath == null || lastPath.Count < 2) return;

        Gizmos.color = pathGizmoColor;

        for(int i = 0;  i < lastPath.Count - 1; i++)
        {
            Gizmos.DrawLine(lastPath[i].WorldPosition, lastPath[i + 1].WorldPosition);
        }
    }
}
