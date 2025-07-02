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
    [SerializeField] private float _attackCooldown = 1.5f;


    private float _lastAttackTime = 0f;
    private UnitInstance _targetEnemy;
    private int _currentHealth;
    private Pathfinder _pathfinder;
    private List<GridNode> _currentPath = new List<GridNode>();
    private int _pathIndex = 0;
    private Vector3? _targetWorldPosition = null;
    private bool _isMoving = false;
    public bool PlayerTeam;

   
    public bool IsMoving => _isMoving;

   
    public List<GridNode> CurrentPath => _currentPath;

    public void Initialize(Pathfinder pathfinder, UnitType unitType)
    {
        _pathfinder = pathfinder;
        _unitType = unitType;
        _currentHealth = unitType.Health;
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

    /*public void SetTeamMaterial(Material teamMaterial)
    {
        // Update all Renderer types under the unit skin
        foreach (Renderer renderer in _unitSkin.GetComponentsInChildren<Renderer>(true))
        {
            Material[] newMats = new Material[renderer.sharedMaterials.Length];
            for (int i = 0; i < newMats.Length; i++)
            {
                newMats[i] = teamMaterial;
            }
            renderer.materials = newMats;
        }
    }*/

    public void SetTeamMaterial(Material teamMaterial)
    {
        foreach (Renderer renderer in _unitSkin.GetComponentsInChildren<Renderer>(true))
        {
            int matCount = renderer.sharedMaterials.Length;

            Material[] newMats = new Material[matCount];
            for (int i = 0; i < matCount; i++)
            {
                newMats[i] = teamMaterial;
            }

            renderer.materials = newMats;
        }
    }

    public void TryFindTarget(List<UnitInstance> enemyUnits)
    {
        foreach (var enemy in enemyUnits)
        {
            if (enemy == null || !enemy.gameObject.activeInHierarchy) continue;

            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance <= _unitType.Range)
            {
                _targetEnemy = enemy;
                State = UnitState.Attacking;
                break;
            }
        }
    }

    protected override void HandleAttack()
    {
        if (_targetEnemy == null)
        {
            State = UnitState.Nothing;
            return;
        }

        float distance = Vector3.Distance(transform.position, _targetEnemy.transform.position);
        if (distance > _unitType.Range)
        {
            State = UnitState.Nothing;
            _targetEnemy = null;
            return;
        }

        if (Time.time - _lastAttackTime >= _attackCooldown)
        {
            _lastAttackTime = Time.time;
            _targetEnemy.TakeDamage(_unitType.Damage);
        }
    }

    public void TakeDamage(int amount)
    {
        _currentHealth -= amount;

        _hurtParticles?.Play();

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        gameObject.SetActive(false); // or Destroy(gameObject)
        State = UnitState.Dead;
    }
}
