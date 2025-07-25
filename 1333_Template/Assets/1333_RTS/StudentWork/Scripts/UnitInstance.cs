using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// represents a specific unit instance in the game, inherits from UnitBase
/// </summary>
public class UnitInstance : UnitBase, IAttackable
{
    [Header("Prefab Stuff")]
    [SerializeField] private Animator _characterAnimator;
    [SerializeField] private GameObject _unitSkin;
    [SerializeField] private ParticleSystem _hurtParticles;
    [SerializeField] private float _attackCooldown = 1.5f;
    [SerializeField] private GameObject _healthBarPrefab;


    private Image _healthBarFill;
    private Transform _healthBar;
    private float _lastAttackTime = 0f;
    private IAttackable _target;
    private int _currentHealth;
    private Pathfinder _pathfinder;
    private List<GridNode> _currentPath = new List<GridNode>();
    private int _pathIndex = 0;
    private Vector3? _targetWorldPosition = null;
    private bool _isMoving = false;
    public bool PlayerTeam;
    private float _repathInterval = 0.5f;
    private float _lastRepathTime = 0f;
    private Vector3? _lastTargetPosition = null;
    private const float ArrivalRadius = 0.5f;

    public bool IsDead => State == UnitState.Dead;

    public Vector3 Position => transform.position;

    UnitState IAttackable.State => this.State;

    public bool IsMoving => _isMoving;

   
    public List<GridNode> CurrentPath => _currentPath;

    public void Initialize(Pathfinder pathfinder, UnitType unitType)
    {
        _pathfinder = pathfinder;
        _unitType = unitType;
        _currentHealth = unitType.Health;

        GameObject hb = Instantiate(_healthBarPrefab, transform);
        _healthBar = hb.transform;

        _healthBar.localPosition = new Vector3(0, 2f, 0);

        _healthBarFill = _healthBar.Find("HealthBarBackground/HealthBarFill").GetComponent<Image>();
    }

    private void Update()
    {
        if (State == UnitState.Dead) return;

        UpdateAnimator();

        if (State == UnitState.Moving)
        {
            HandleMovement();
        }

        Tick();
    }

    /// <summary>
    /// sets the target based off the world position
    /// </summary>
    public void SetTarget(Vector3 worldPosition)
    {
        _targetWorldPosition = worldPosition;
        _currentPath = _pathfinder.FindPath(transform.position, worldPosition);
        _pathIndex = 0;
        _isMoving = _currentPath != null && _currentPath.Count > 1;

        if (_currentPath == null || _currentPath.Count == 0)
        {
            Debug.LogWarning($"No path found from {transform.position} to {worldPosition}");
        }
        else
        {
            string pathStr = string.Join(" -> ", _currentPath.Select(n => $"({n.X},{n.Y})"));
            Debug.Log($"Path found: {pathStr}");
        }
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

    public void TryFindTarget(List<UnitInstance> enemyUnits, List<BuildingInstance> enemyBuildings)
    {
        Vector3 closestTargetPosition = Vector3.zero;
        IAttackable closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (var target in enemyUnits)
        {
            if (target == null || target.IsDead) continue;

            float dist = Vector3.Distance(transform.position, target.Position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closestTarget = target;
                closestTargetPosition = target.Position;
            }
        }

        foreach (var building in enemyBuildings)
        {
            if (building == null || building.IsDead) continue;

            List<GridNode> perimeterNodes = building.GetPerimeterNodes();

            foreach (var node in perimeterNodes)
            {
                Debug.Log($"Node at {node.X},{node.Y} walkable={node.Walkable}");
            }

            foreach (var node in perimeterNodes)
            {
                if (!node.Walkable) continue;

                float dist = Vector3.Distance(transform.position, node.WorldPosition);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closestTarget = building;
                    closestTargetPosition = node.WorldPosition; // Important: perimeter node position!
                }
            }
        }

        if (closestTarget != null)
        {
            _target = closestTarget;
            SetTarget(closestTargetPosition);  // Use perimeter node position here, not building center
            State = UnitState.Moving;

            if (closestDistance <= _unitType.Range)
            {
                State = UnitState.Attacking;
                _isMoving = false;
            }
        }
    }

    protected override void HandleAttack()
    {
        State = UnitState.Attacking;

        if (_target == null || _target.IsDead)
        {
            _target = null;
            State = UnitState.Nothing;
            return;
        }

        float distance = Vector3.Distance(transform.position, _target.Position);
        if (distance > _unitType.Range)
        {
            State = UnitState.Moving;
            SetTarget(_target.Position);
            return;
        }

        if (Time.time - _lastAttackTime >= _attackCooldown)
        {
            _lastAttackTime = Time.time;
            _target.TakeDamage(_unitType.Damage);
        }
    }

    public void TakeDamage(int amount)
    {
        _currentHealth -= amount;

        _hurtParticles?.Play();

        _characterAnimator.SetTrigger("TakeDamage");

        UpdateHealthBar();

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthBar()
    {
        float healthPercent = Mathf.Clamp01((float)_currentHealth / _unitType.Health);
        if(_healthBarFill != null)
            _healthBarFill.fillAmount = healthPercent;
    }

    private void Die()
    {
        StopMoving();
        AudioManager.instance.PlaySFX(0);
        _characterAnimator.SetTrigger("Die");
        Destroy(gameObject);
        State = UnitState.Dead;
    }

    public override void Tick()
    {
        base.Tick();

        if (State == UnitState.Moving && _target != null)
        {
            float distance = Vector3.Distance(transform.position, _target.transform.position);

            if (distance <= _unitType.Range)
            {
                // Target is within attack range — stop moving and attack
                StopMoving();
                State = UnitState.Attacking;
            }
            else
            {
                // Target is out of range — keep moving and recalculate path if needed
                if (_lastTargetPosition == null ||
                    Vector3.Distance(_lastTargetPosition.Value, _target.transform.position) > 0.5f ||
                    Time.time - _lastRepathTime >= _repathInterval)
                {
                    SetTarget(_target.transform.position);
                    _lastTargetPosition = _target.transform.position;
                    _lastRepathTime = Time.time;
                }
            }
        }
        else if (State == UnitState.Attacking)
        {
            HandleAttack();
        }
        else if (State == UnitState.Moving)
        {
            HandleMovement();
        }
    }

    private void HandleMovement()
    {
        State = UnitState.Moving;

        if (!_isMoving || _currentPath == null || _currentPath.Count == 0 || _pathIndex >= _currentPath.Count)
        {
            State = UnitState.Nothing; // stop moving
            _isMoving = false;
            return;
        }

        Vector3 nextWaypoint = _currentPath[_pathIndex].WorldPosition;
        float distanceToWaypoint = Vector3.Distance(transform.position, nextWaypoint);

        if (distanceToWaypoint <= ArrivalRadius)
        {
            _pathIndex++;

            if (_pathIndex >= _currentPath.Count)
            {
                State = UnitState.Nothing;
                _isMoving = false;
                return;
            }

            nextWaypoint = _currentPath[_pathIndex].WorldPosition;
        }

        Vector3 direction = (nextWaypoint - transform.position).normalized;
        float step = _unitType.MovementSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, nextWaypoint, step);
    }

    private void UpdateAnimator()
    {
        switch (State)
        {
            case UnitState.Nothing:
                _characterAnimator.SetBool("IsIdle", true);
                _characterAnimator.SetBool("IsAttacking", false);
                _characterAnimator.SetBool("IsWalking", false);
                break;

            case UnitState.Attacking:
                _characterAnimator.SetBool("IsAttacking", true);
                _characterAnimator.SetBool("IsIdle", false);
                _characterAnimator.SetBool("IsWalking", false);
                break;

            case UnitState.Moving:
                _characterAnimator.SetBool("IsWalking", true);
                _characterAnimator.SetBool("IsIdle", false);
                _characterAnimator.SetBool("IsAttacking", false);
                break;  
        }
    }
}
