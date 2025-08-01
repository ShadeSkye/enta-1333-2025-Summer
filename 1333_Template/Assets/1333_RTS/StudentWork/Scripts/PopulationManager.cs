using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulationManager : MonoBehaviour
{
    public static PopulationManager Instance;
    public int CurrentPopulation { get; private set; } = 0;
    public int MaxPopulation { get; private set; } = 0;

    private void Awake()
    {
        Instance = this;
    }

    public void AddCapacity(int amount)
    {
        MaxPopulation += amount;
        Debug.Log($"Max Population increased by {amount}. New max: {MaxPopulation}");
        UIManager.Instance?.UpdatePopulationUI();
    }

    public void RemoveCapacity(int amount)
    {
        MaxPopulation -= amount;
        if (MaxPopulation < 0) MaxPopulation = 0;
        Debug.Log($"Max Population decreased by {amount}. New max: {MaxPopulation}");
        UIManager.Instance?.UpdatePopulationUI();
    }

    public bool CanAddUnits(int amount)
    {
        return CurrentPopulation + amount <= MaxPopulation;
    }

    public bool AddUnits(int amount)
    {
        if (CanAddUnits(amount))
        {
            CurrentPopulation += amount;
            Debug.Log($"Added {amount} units. Current population: {CurrentPopulation}");
            UIManager.Instance?.UpdatePopulationUI();
            return true;
        }
        Debug.LogWarning("Not enough population capacity!");
        return false;
    }

    public void RemoveUnits(int amount)
    {
        CurrentPopulation -= amount;
        if (CurrentPopulation < 0) CurrentPopulation = 0;
        Debug.Log($"Removed {amount} units. Current population: {CurrentPopulation}");
        UIManager.Instance?.UpdatePopulationUI();
    }
}
