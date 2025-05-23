using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private UnitManager unitManager;
    [SerializeField] private PathfindingAlgorithm pathfinder;

    private void Awake()
    {
        gridManager.InitializeGrid();
    }
}
