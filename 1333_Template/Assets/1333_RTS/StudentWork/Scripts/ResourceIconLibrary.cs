using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ResourceIconLibrary", menuName = "Game/Resource Icon Library")]
public class ResourceIconLibrary : ScriptableObject
{
    [System.Serializable]
    public class ResourceIconEntry
    {
        public ResourceType resourceType;
        public Sprite icon;
    }

    public List<ResourceIconEntry> icons = new();

    private Dictionary<ResourceType, Sprite> iconMap;

    private void OnEnable()
    {
        iconMap = new Dictionary<ResourceType, Sprite>();
        foreach (var entry in icons)
        {
            iconMap[entry.resourceType] = entry.icon;
        }
    }

    public Sprite GetIcon(ResourceType type)
    {
        if (iconMap == null || !iconMap.ContainsKey(type))
        {
            Debug.LogWarning($"Missing icon for resource type: {type}");
            return null;
        }

        return iconMap[type];
    }
}
