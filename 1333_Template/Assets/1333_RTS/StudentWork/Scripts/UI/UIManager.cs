using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Population UI")]
    public TextMeshProUGUI populationText;

    [Header("Resources UI")]
    public ResourceUIEntry foodEntry;
    public ResourceUIEntry woodEntry;
    public ResourceUIEntry metalEntry;

    private Dictionary<ResourceType, ResourceUIEntry> resourceEntries = new();

    void Awake()
    {
        Instance = this;

        resourceEntries[ResourceType.Food] = foodEntry;
        resourceEntries[ResourceType.Wood] = woodEntry;
        resourceEntries[ResourceType.Metal] = metalEntry;
    }

    void Start()
    {
        UpdatePopulationUI();
        UpdateResourcesUI();
    }

    public void UpdatePopulationUI()
    {
        int current = PopulationManager.Instance.CurrentPopulation;
        int max = PopulationManager.Instance.MaxPopulation;
        populationText.text = $"Population: {current} / {max}";
    }

    public void UpdateResourcesUI()
    {
        var allResources = ResourceManager.Instance.GetAllResources();

        foreach (var resource in allResources)
        {
            if (resourceEntries.TryGetValue(resource.Type, out var entry))
            {
                entry.UpdateAmount(resource.Amount);
            }
        }
    }
}
