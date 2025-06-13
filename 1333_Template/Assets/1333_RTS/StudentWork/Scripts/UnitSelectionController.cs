using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelectionController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GridManager gridManager;

    private UnitInstance selectedUnit;

    void Update()
    {
        // Select unit on left-click
        if (Input.GetMouseButtonDown(0))
        {
            TrySelectUnit();
        }

        // Deselect unit on right-click
        if (Input.GetMouseButtonDown(1))
        {
            DeselectUnit();
        }

        // Move selected unit on left-click if already selected
        if (selectedUnit != null && Input.GetMouseButtonDown(0))
        {
            TryMoveUnit();
        }
    }

    private void TrySelectUnit()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Check if clicked on a UnitInstance
            UnitInstance unit = hit.collider.GetComponent<UnitInstance>();
            if (unit != null)
            {
                selectedUnit = unit;
                selectedUnit.StopMoving(); // <--- stop the unit on selection
                Debug.Log($"Selected unit: {unit.name}");
            }
        }
    }

    private void TryMoveUnit()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        // Create a horizontal plane at Y = 0 (ground level)
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (groundPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);

            GridNode targetNode = gridManager.GetNodeFromWorldPosition(hitPoint);
            if (IsValidNode(targetNode))
            {
                selectedUnit.MoveTo(targetNode);
                Debug.Log($"Moving unit to node at {targetNode.WorldPosition}");
            }
            else
            {
                Debug.LogWarning("Invalid target node selected!");
            }
        }
    }

    private bool IsValidNode(GridNode node)
    {
        // You can enhance this check depending on your GridNode design
        return node.Walkable && node.WorldPosition != Vector3.zero;
    }

    private void DeselectUnit()
    {
        if (selectedUnit != null)
        {
            Debug.Log($"Deselected unit: {selectedUnit.name}");
            selectedUnit = null;
        }
    }
}
