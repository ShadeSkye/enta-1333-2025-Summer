using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class BuildingInstance : MonoBehaviour, IAttackable
{
    [Header("Health Bar")]
    [SerializeField] private GameObject healthBarPrefab;

    private Image healthBarFill;
    private Transform healthBar;
    private BuildingData buildingData;
    private int currentHealth;
    private List<GridNode> _occupiedNodes = new();
    private int _startX;
    private int _startY;
    private int _width;
    private int _length;

    private bool _isDead = false;

    public List<GridNode> OccupiedNodes => _occupiedNodes;
    public bool IsDead => _isDead;

    public Vector3 Position => transform.position;

    public UnitState State => _isDead ? UnitState.Dead : UnitState.Nothing;

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(0, currentHealth);

        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthBar()
    {
        float percent = (float)currentHealth / buildingData.Health;
        if (healthBarFill != null)
            healthBarFill.fillAmount = percent;
    }

    private void Die()
    {
        UnitManager.Instance.UnregisterPlayerBuilding(this);
        Destroy(gameObject);
    }

    public void Initialize(BuildingData data, List<GridNode> occupiedNodes, int startX, int startY)
    {
        buildingData = data;
        currentHealth = data.Health;
        _occupiedNodes = occupiedNodes;

        _startX = startX;
        _startY = startY;
        _width = data.Width;
        _length = data.Length;

        UnitManager.Instance.RegisterPlayerBuilding(this);
        SetupHealthBar();
    }

    private void SetupHealthBar()
    {
        if (healthBarPrefab != null)
        {
            GameObject hb = Instantiate(healthBarPrefab, transform);
            healthBar = hb.transform;
            healthBar.localPosition = new Vector3(0, 0, buildingData.Height + 1f); // adjusts based on building height
            healthBarFill = healthBar.Find("HealthBarBackground/HealthBarFill").GetComponent<Image>();

            UpdateHealthBar();
        }
    }

    public List<GridNode> GetWalkablePerimeter(GridManager gridManager)
    {
        HashSet<GridNode> perimeter = new();

        foreach (var node in _occupiedNodes)
        {
            List<GridNode> neighbors = gridManager.GetNeighbors(node);

            foreach (var neighbor in neighbors)
            {
                if (neighbor.Walkable)
                {
                    perimeter.Add(neighbor);
                }
            }
        }

        return new List<GridNode>(perimeter);
    }

    public List<GridNode> GetPerimeterNodes()
    {
        List<GridNode> perimeter = new List<GridNode>();

        for (int x = -1; x <= _width; x++)
        {
            for (int y = -1; y <= _length; y++)
            {
                bool isEdge = x == -1 || x == _width || y == -1 || y == _length;
                if (isEdge)
                {
                    int gridX = _startX + x;
                    int gridY = _startY + y;

                    if (GridManager.instance.IsWithinBounds(gridX, gridY))
                    {
                        GridNode node = GridManager.instance.GetNode(gridX, gridY);
                        if (node.IsValid && node.Walkable)
                            perimeter.Add(node);
                    }
                }
            }
        }

        return perimeter;
    }
}
