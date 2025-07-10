using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    [SerializeField] private GridManager _gridManager;
    [SerializeField] private GameObject dummy;
    [SerializeField] private Material[] teamMaterials;

    private Dictionary<int, ArmyManager> _armyManager;
    public ArmyManager PlayerArmy => _armyManager?[0];

    private List<UnitInstance> _allUnits = new();
    private List<UnitInstance> _playerUnits = new();
    private List<UnitInstance> _enemyUnits = new();

    private void Update()
    {
        foreach (var unit in _allUnits)
        {
            if (unit == null || unit.State == UnitState.Dead) continue;

            if (unit.State == UnitState.Nothing)
            {
                List<UnitInstance> targets = unit.PlayerTeam ? _enemyUnits : _playerUnits;
                unit.TryFindTarget(targets);
            }

            unit.Tick();
        }
    }

    /// <summary>
    /// Spawns a dummy unit at a random location
    /// </summary>
    public void SpawnDummyUnit(Transform parent)
    {
        if (!_gridManager.IsInitialized)
        {
            Debug.LogError("Grid not initialized!");
            return;
        }

        int randomX = Random.Range(0, _gridManager.GridSettings.GridSizeX);
        int randomY = Random.Range(0, _gridManager.GridSettings.GridSizeY);

        GridNode spawnNode = _gridManager.GetNode(randomX, randomY);
        Debug.Log($"Dummy unit spawned at ({randomX}, {randomY}) - World Position: {spawnNode.WorldPosition}");

        GameObject go = Instantiate(dummy, spawnNode.WorldPosition, Quaternion.identity, parent);
        UnitInstance unit = go.GetComponent<UnitInstance>();

        if (unit != null)
        {
            unit.PlayerTeam = false;
            RegisterUnit(unit);
        }
    }

    public void RegisterUnit(UnitInstance unit)
    {
        if (unit == null || _allUnits.Contains(unit)) return;

        _allUnits.Add(unit);

        if (unit.PlayerTeam)
            _playerUnits.Add(unit);
        else
            _enemyUnits.Add(unit);
    }

    public void UnregisterUnit(UnitInstance unit)
    {
        _allUnits.Remove(unit);
        _playerUnits.Remove(unit);
        _enemyUnits.Remove(unit);
    }

    public void ClearAllUnits()
    {
        foreach (var unit in _allUnits)
        {
            if (unit != null)
                Destroy(unit.gameObject);
        }

        _allUnits.Clear();
        _playerUnits.Clear();
        _enemyUnits.Clear();
    }
}
