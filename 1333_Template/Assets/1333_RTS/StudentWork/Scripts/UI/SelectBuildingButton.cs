using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectBuildingButton : MonoBehaviour
{
    [SerializeField] private Image buttonImage;
    [SerializeField] private TMP_Text buttontext;
    [SerializeField] private Button button;

    private BuildingData buildingDataForButton;

    public void Setup(BuildingData buildingData)
    {
        buildingDataForButton = buildingData;

        buttontext.text = buildingDataForButton.BuildingName;

        buttonImage.sprite = buildingDataForButton.Icon;
    }

    public void SetCurrentBuilding()
    {
        CurrentlySelectedBuilding.SetCurrentBuilding(buildingDataForButton);
    }
}
