using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentlySelectedBuilding
{
    private static BuildingData CurrentBuilding;

    public static BuildingData GetCurrentBuilding()
    {
        return CurrentBuilding;
    }

    public static void SetCurrentBuilding(BuildingData buildingData)
    {
        CurrentBuilding = buildingData;
    }
}
