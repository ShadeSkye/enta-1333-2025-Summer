using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmyManager
{
    /// <summary>
    /// unique army ID (player will be 0)
    /// </summary>
    public int ArmyID;

    /// <summary>
    /// needed for changing unit materials
    /// </summary>
    public Material TeamMaterial;

    /// <summary>
    /// returns true if player army
    /// </summary>
    public bool IsPlayer => ArmyID == 0;

    /// <summary>
    /// list of all units in the army
    /// </summary>
    public List<UnitBase> Units = new List<UnitBase>();

    /// <summary>
    /// reference to the GridManager
    /// </summary>
    public GridManager GridManager;

    /// <summary>
    /// moves all units to a target in the world
    /// </summary>
    public void MoveAllUnitsTo(Vector3 worldPosition)
    {
        foreach (var unit in Units)
        {
            unit.MoveTo(GridManager.GetNodeFromWorldPosition(worldPosition));
        }
    }

    /// <summary>
    /// moves all units to a target node
    /// </summary>
    public void MoveAllUnitsTo(GridNode node)
    {
        foreach (var unit in Units)
        {
            unit.MoveTo(node);
        }
    }
}
