using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;

    [SerializeField] private ResourceIconLibrary iconLibrary;
    [SerializeField] private List<StartingResource> startingResources;

    private Dictionary<ResourceType, int> resourceTotals = new();

    private void Awake()
    {
        Instance = this;

        foreach (ResourceType type in System.Enum.GetValues(typeof(ResourceType)))
        {
            resourceTotals[type] = 0;
        }

        foreach (var entry in startingResources)
        {
            AddResource(entry.Type, entry.Amount);
        }
    }

    public void AddResource(ResourceType type, int amount)
    {
        resourceTotals[type] += amount;
        Debug.Log($"{type} +{amount} (Total: {resourceTotals[type]})");
        UIManager.Instance?.UpdateResourcesUI();
    }

    public int GetResource(ResourceType type)
    {
        return resourceTotals.ContainsKey(type) ? resourceTotals[type] : 0;
    }

    public List<ResourceData> GetAllResources()
    {
        List<ResourceData> result = new();

        foreach (var kvp in resourceTotals)
        {
            Sprite icon = iconLibrary.GetIcon(kvp.Key);
            result.Add(new ResourceData(kvp.Key, kvp.Value, icon));
        }

        return result;
    }

    public struct ResourceData
    {
        public ResourceType Type;
        public int Amount;
        public Sprite Icon;

        public ResourceData(ResourceType type, int amount, Sprite icon)
        {
            Type = type;
            Amount = amount;
            Icon = icon;
        }
    }
}

[System.Serializable]
public struct StartingResource
{
    public ResourceType Type;
    public int Amount;
}
