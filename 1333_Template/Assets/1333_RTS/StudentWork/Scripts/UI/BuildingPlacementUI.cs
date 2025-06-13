using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPlacementUI : MonoBehaviour
{
    [SerializeField] private RectTransform LayoutGroupParent;
    [SerializeField] private SelectBuildingButton ButtonPrefab;
    [SerializeField] private BuildingTypes BuildingData;

    private void Start()
    {
        if(BuildingData.Buildings != null)
        {
            foreach (BuildingData t in BuildingData.Buildings)
            {
                SelectBuildingButton button = Instantiate(ButtonPrefab, LayoutGroupParent);

                button.Setup(t);
            }
        }
    }
}
