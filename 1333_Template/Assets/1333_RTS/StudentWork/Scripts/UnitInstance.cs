using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private GameObject _healthBarPrefab;


    private Image _healthBarFill;
    private Transform _healthBar;
    private float _lastAttackTime = 0f;
    private UnitInstance _targetEnemy;
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

    public event System.Action<UnitInstance> OnDeath;

    public bool IsMoving => _isMoving;

   
    public List<GridNode> CurrentPath => _currentPath;

    public void Initialize(Pathfinder pathfinder, UnitType unitType)
    {
        _pathfinder = pathfinder;
        _unitType = unitType;
        _currentHealth = unitType.Health;

        Vector3 pos = transform.position;
        pos.y = 0f;
        transform.position = pos;

        GameObject hb = Instantiate(_healthBarPrefab, transform);
        _healthBar = hb.transform;

        _healthBar.localPosition = new Vector3(0, 2f, 0);

        _healthBarFill = _healthBar.Find("HealthBarBackground/HealthBarFill").GetComponent<Image>();
    }

    private void Update()
    {
        if (State == UnitState.Dead) return;

        UpdateAnimator();

        if (_targetEnemy != null && _targetEnemy.State != UnitState.Dead)
        {
            float dist = Vector3.Distance(transform.position, _targetEnemy.transform.position);

            if (dist <= _unitType.Range)
            {
                StopMoving(); // stop if close enough
                State = UnitState.Attacking;
            }
        }

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
        UnitInstance closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (var target in enemyUnits)
        {
            if (target == null || target.State == UnitState.Dead) continue;

            float dist = Vector3.Distance(transform.position, target.transform.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closestTarget = target;
            }
        }

        if (closestTarget != null)
        {
            _targetEnemy = closestTarget;
            // Only recalc path if target moved more than threshold or we don't have a path yet
            if (_currentPath == null || !_lastTargetPosition.HasValue ||Vector3.Distance(_lastTargetPosition.Value, closestTarget.transform.position) > ArrivalRadius)
            {
                _currentPath = _pathfinder.FindPath(transform.position, closestTarget.transform.position);
                _pathIndex = 0;
                _isMoving = true;
                State = UnitState.Moving;
                _lastTargetPosition = closestTarget.transform.position;
            }

            // If close enough, switch to attack
            if (closestDistance <= _unitType.Range)
            {
                State = UnitState.Attacking;
                _isMoving = false;
            }
        }
        else
        {
            State = UnitState.Nothing;
            _isMoving = false;
        }
    }

    protected override void HandleAttack()
    {
        State = UnitState.Attacking;

        if (_targetEnemy == null || _targetEnemy.State == UnitState.Dead)
        {
            _targetEnemy = null;
            State = UnitState.Nothing;
            return;
        }

        float distance = Vector3.Distance(transform.position, _targetEnemy.transform.position);
        if (distance > _unitType.Range)
        {
            State = UnitState.Moving;
            SetTarget(_targetEnemy.transform.position); // path once again toward new position
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
        State = UnitState.Dead;

        if (!PlayerTeam)
        {
            OnDeath?.Invoke(this);
        }

        if (PlayerTeam)
        {
            PopulationManager.Instance.RemoveUnits(1);
        }

        Destroy(gameObject);
    }

    public override void Tick()
    {
        base.Tick();

        if(State == UnitState.Moving)
        {
            HandleMovement();
        }
        else if (State == UnitState.Attacking)
        {
            HandleAttack();
        }
        else if (State == UnitState.Moving && _targetEnemy != null)
        {
            float distance = Vector3.Distance(transform.position, _targetEnemy.transform.position);

            if (distance <= _unitType.Range)
            {
                StopMoving();
                State = UnitState.Attacking;
                return;
            }

            // Recalculate path if enemy has moved or every interval
            if (_lastTargetPosition == null ||
                Vector3.Distance(_lastTargetPosition.Value, _targetEnemy.transform.position) > 0.5f ||
                Time.time - _lastRepathTime >= _repathInterval)
            {
                SetTarget(_targetEnemy.transform.position);
                _lastTargetPosition = _targetEnemy.transform.position;
                _lastRepathTime = Time.time;
            }
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
