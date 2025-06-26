using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSelectionController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private BarracksUIController barracksUI; // Add more UIs as needed

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TrySelectBuilding();
        }

        if (Input.GetMouseButtonDown(1))
        {
            DeselectAll();
        }
    }

    private void TrySelectBuilding()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Check if the clicked object has a building component
            var barracks = hit.collider.GetComponent<BarracksSpawner>();

            if (barracks != null)
            {
                ShowBarracksUI(barracks);
                return;
            }

            //  Add checks for other building types here
        }
    }

    private void ShowBarracksUI(BarracksSpawner spawner)
    {
        DeselectAll(); // hide other UIs
        barracksUI.ShowUI(spawner);
    }

    private void DeselectAll()
    {
        barracksUI.HideUI();

        //  Add HideUI for other UIs here
    }
}
