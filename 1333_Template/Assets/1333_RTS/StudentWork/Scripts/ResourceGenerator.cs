using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;

public class ResourceGenerator : MonoBehaviour
{
    private List<ResourceProductionData> productionData;
    private ResourceManager resourceManager;
    private bool isProducing = false;

    public void Initialize(List<ResourceProductionData> resources)
    {
        productionData = resources;
        resourceManager = FindObjectOfType<ResourceManager>();

        if (resourceManager != null && productionData != null && productionData.Count > 0)
        {
            StartCoroutine(ProduceResources());
        }
    }

    private IEnumerator ProduceResources()
    {
        isProducing = true;

        while (isProducing)
        {
            foreach (var resource in productionData)
            {
                yield return new WaitForSeconds(resource.ProductionInterval);
                resourceManager.AddResource(resource.ResourceType, resource.AmountPerCycle);
            }
        }
    }

    private void OnDestroy()
    {
        isProducing = false;
    }
}
