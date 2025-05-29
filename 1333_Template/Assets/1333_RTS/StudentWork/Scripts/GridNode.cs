using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct GridNode
{
    public TerrainType TerrainType;
    public string Name;
    public Vector3 WorldPosition;
    public bool Walkable;
    public int Weight;
    public Color GizmoColor;
}
