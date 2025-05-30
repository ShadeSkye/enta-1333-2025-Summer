using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// represents a specific unit instance in the game, inherits from UnitBase
/// </summary>
public class UnitInstance : UnitBase
{
    [Header("Prefab Stuff")]
    [SerializeField] private Animator _characterAnimator;
    [SerializeField] private GameObject _unitSkin;
    [SerializeField] private ParticleSystem _hurtParticles;

    private Pathfinder _pathfinder;
    private List<GridNode> _currentPath = new List<GridNode>();
    private int _pathIndex = 0;
    private Vector3? _targetWorldPosition = null;
    private bool _isMoving = false;

    public bool IsMoving => _isMoving;
    public List<GridNode> CurrentPath => _currentPath;

    public void Initialize(Pathfinder pathfinder, UnitType unitType)
    {
        _pathfinder = pathfinder;
        _unitType = unitType;

        foreach(SkinnedMeshRenderer skin in _unitSkin.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            //change materials to match team
        }
    }

    public void SetTarget(Vector3 worldPosition)
    {
        _targetWorldPosition = worldPosition;
    }

    public override void MoveTo(GridNode targetNode)
    {
        throw new System.NotImplementedException();
    }
}
