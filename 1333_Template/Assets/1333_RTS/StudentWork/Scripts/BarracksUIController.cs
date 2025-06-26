using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarracksUIController : MonoBehaviour
{
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private GameObject unitButtonPrefab;

    private BarracksSpawner currentSpawner;

    public void ShowUI(BarracksSpawner spawner)
    {
        gameObject.SetActive(true);

        // If we're already showing this spawner, don't refresh
        if (currentSpawner == spawner)
            return;

        currentSpawner = spawner;

        ClearButtons();
        PopulateUnitButtons();
    }

    public void HideUI()
    {
        gameObject.SetActive(false);
        currentSpawner = null;
        ClearButtons();
    }

    private void ClearButtons()
    {
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }
    }

    /*private void PopulateUnitButtons()
    {
        if (currentSpawner == null || currentSpawner.buildingData == null)
        {
            Debug.LogWarning("CurrentSpawner or BuildingData is null");
            return;
        }

        foreach (UnitTypePrefab unitTypePrefab in currentSpawner.buildingData.SpawnableUnits)
        {
            GameObject buttonGO = Instantiate(unitButtonPrefab, buttonContainer);
            SelectUnitButton buttonComponent = buttonGO.GetComponent<SelectUnitButton>();

            if (buttonComponent != null)
            {
                buttonComponent.SetUp(unitTypePrefab.unitType, OnUnitButtonClicked);
            }
            else
            {
                Debug.LogWarning("Unit button prefab is missing SelectUnitButton component!");
            }
        }
    }*/

    private void PopulateUnitButtons()
    {
        if (currentSpawner == null || currentSpawner.buildingData == null)
        {
            Debug.LogWarning("CurrentSpawner or BuildingData is null");
            return;
        }

        if (currentSpawner.buildingData.SpawnableUnits == null || currentSpawner.buildingData.SpawnableUnits.Count == 0)
        {
            Debug.LogWarning("No spawnable units found on this building");
            return;
        }

        foreach (UnitTypePrefab unitTypePrefab in currentSpawner.buildingData.SpawnableUnits)
        {
            Debug.Log($"Creating button for unit: {unitTypePrefab.unitType.name}");

            GameObject buttonGO = Instantiate(unitButtonPrefab, buttonContainer);
            SelectUnitButton buttonComponent = buttonGO.GetComponent<SelectUnitButton>();

            if (buttonComponent != null)
            {
                buttonComponent.SetUp(unitTypePrefab.unitType, OnUnitButtonClicked);
            }
            else
            {
                Debug.LogWarning("Unit button prefab is missing SelectUnitButton component!");
            }
        }
    }

    private void OnUnitButtonClicked(UnitType unitType)
    {
        currentSpawner.SpawnUnitByType(unitType);
    }
}
