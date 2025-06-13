using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPlacementController : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;

    private GameObject currentGhost;
    private BuildingData currentBuilding;

    private void Update()
    {
        currentBuilding = CurrentlySelectedBuilding.GetCurrentBuilding();
        if (currentBuilding == null || !gridManager.IsInitialized)
        {
            if (currentGhost != null)
            {
                Destroy(currentGhost);
                currentGhost = null;
            }
            return;
        }

        Vector3 mouseWorldPos = GetMouseWorldPosition();

        // Convert mouse position to grid coords (bottom-left corner)
        int rawX = Mathf.FloorToInt(mouseWorldPos.x / gridManager.GridSettings.NodeSize);
        int rawY = Mathf.FloorToInt(mouseWorldPos.z / gridManager.GridSettings.NodeSize);

        int maxX = gridManager.GridSettings.GridSizeX - currentBuilding.Width;
        int maxY = gridManager.GridSettings.GridSizeY - currentBuilding.Length;

        int clampedX = Mathf.Clamp(rawX, 0, maxX);
        int clampedY = Mathf.Clamp(rawY, 0, maxY);

        Vector2Int gridPos = new Vector2Int(clampedX, clampedY);

        ShowGhostBuilding(gridPos);

        if (Input.GetMouseButtonDown(0))
        {
            if (gridManager.CanPlaceBuilding(gridPos.x, gridPos.y, currentBuilding))
            {
                PlaceBuilding(gridPos.x, gridPos.y, currentBuilding);
            }
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero); // y = 0 plane

        if (groundPlane.Raycast(ray, out float enter))
        {
            return ray.GetPoint(enter);
        }

        return Vector3.zero; // fallback
    }

    private void ShowGhostBuilding(Vector2Int gridPos)
    {
        if (currentBuilding.BuildingPrefab == null || currentBuilding.BuildingPrefabGhost == null)
        {
            Debug.LogWarning($"BuildingData for {currentBuilding.BuildingName} is missing a prefab!");
            return;
        }

        // Position of bottom-left corner node in world space
        Vector3 baseWorldPos = gridManager.GetNode(gridPos.x, gridPos.y).WorldPosition;

        // Calculate the center offset for the ghost so it covers the full footprint correctly
        float offsetX = (currentBuilding.Width * gridManager.GridSettings.NodeSize) / 2f - (gridManager.GridSettings.NodeSize / 2f);
        float offsetZ = (currentBuilding.Length * gridManager.GridSettings.NodeSize) / 2f - (gridManager.GridSettings.NodeSize / 2f);

        Vector3 ghostPos = gridManager.GridSettings.UseXZPlane
            ? baseWorldPos + new Vector3(offsetX, 0, offsetZ)
            : baseWorldPos + new Vector3(offsetX, offsetZ, 0);

        if (currentGhost == null)
        {
            currentGhost = Instantiate(currentBuilding.BuildingPrefabGhost);
        }

        currentGhost.transform.position = ghostPos;
        currentGhost.transform.localScale = Vector3.one;
        currentGhost.transform.rotation = Quaternion.Euler(-90, 0, 0);

        // Color logic: green = valid, red = invalid
        bool canPlace = gridManager.CanPlaceBuilding(gridPos.x, gridPos.y, currentBuilding);

        Color targetColor = canPlace ? new Color(0f, 1f, 0f, 0.5f) : new Color(1f, 0f, 0f, 0.5f);

        SetGhostColor(targetColor);
    }

    private void PlaceBuilding(int startX, int startY, BuildingData building)
    {
        // Mark grid nodes as occupied
        for (int x = 0; x < building.Width; x++)
        {
            for (int y = 0; y < building.Length; y++)
            {
                GridNode node = gridManager.GetNode(startX + x, startY + y);
                node.BuildingData = building;
                node.Walkable = false;
                gridManager.gridNodes[startX + x, startY + y] = node;
            }
        }

        // Calculate the spawn position of the building GameObject so it’s centered over the footprint
        Vector3 baseWorldPos = gridManager.GetNode(startX, startY).WorldPosition;

        float offsetX = (building.Width * gridManager.GridSettings.NodeSize) / 2f - (gridManager.GridSettings.NodeSize / 2f);
        float offsetZ = (building.Length * gridManager.GridSettings.NodeSize) / 2f - (gridManager.GridSettings.NodeSize / 2f);

        Vector3 spawnPos = gridManager.GridSettings.UseXZPlane
            ? baseWorldPos + new Vector3(offsetX, 0, offsetZ)
            : baseWorldPos + new Vector3(offsetX, offsetZ, 0);

        if (building.BuildingPrefab == null || building.BuildingPrefabGhost == null)
        {
            Debug.LogWarning($"BuildingData for {building.BuildingName} is missing a prefab!");
            return;
        }

        GameObject buildingGO = Instantiate(building.BuildingPrefab, spawnPos, Quaternion.Euler(-90f, 0f, 0f));
        buildingGO.transform.localScale = Vector3.one;
    }

    private void SetGhostColor(Color color)
    {
        if (currentGhost == null) return;

        Renderer[] renderers = currentGhost.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            // Ensure we create a new instance of the material (won’t affect other objects)
            Material mat = renderer.material;
            if (mat.HasProperty("_Color"))
            {
                mat.color = color;
            }
        }
    }
}
