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

        foreach(Renderer renderer in _unitSkin.GetComponentsInChildren<Renderer>())
        {
            
        }
    }

    private void Update()
    {
        
        if (!_isMoving || _currentPath == null || _currentPath.Count == 0 || _pathIndex >= _currentPath.Count)
        {
            if(State == UnitState.Moving)
                State = UnitState.Nothing; //reset state when done moving
            return;
        }
            

        
        Vector3 nextWaypoint = _currentPath[_pathIndex].WorldPosition;
       
        Vector3 direction = (nextWaypoint - transform.position).normalized;
        float step = _unitType.MovementSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, nextWaypoint, step);

        
        if (Vector3.Distance(transform.position, nextWaypoint) < 0.05f)
        {
            _pathIndex++;
            
            if (_pathIndex >= _currentPath.Count)
            {
                _isMoving = false;
            }
        }
    }

    /// <summary>
    /// sets the target based off the world position
    /// </summary>
    public void SetTarget(Vector3 worldPosition)
    {
        // Store the target.
        _targetWorldPosition = worldPosition;
        // Request a path from Pathfinder.
        _currentPath = _pathfinder.FindPath(transform.position, worldPosition);
        _pathIndex = 0;
        _isMoving = _currentPath != null && _currentPath.Count > 1;
    }

    /// <summary>
    /// sets the target based off the grid node
    /// </summary>
    public void SetTarget(GridNode node)
    {
        SetTarget(node.WorldPosition);
    }

    /// <summary>
    /// moves the unit to the target
    /// </summary>
    public override void MoveTo(GridNode targetNode)
    {
        SetTarget(targetNode);
        State = UnitState.Moving;
    }

    public void StopMoving()
    {
        _isMoving = false;
        _currentPath.Clear();
        _pathIndex = 0;
        State = UnitState.Nothing;
    }

    public void SetTeamMaterial(Material teamMaterial)
    {
        foreach (SkinnedMeshRenderer skinRenderer in _unitSkin.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            //assign the team material to each renderer
            skinRenderer.material = teamMaterial;
        }
    }
}
