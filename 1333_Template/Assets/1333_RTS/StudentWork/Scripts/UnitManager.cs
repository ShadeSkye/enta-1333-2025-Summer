using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    public void SpawnDummyUnit(Transform parent)
    {
        if (!gridManager.IsInitialized)
        {
            Debug.Log("Grid not initialized");
            return;
        }

        int randomX = Random.Range(0, gridManager.GridSettings.GridSizeX);
        int randomY = Random.Range(0, gridManager.GridSettings.GridSizeY);
    }
}
