using System.Collections.Generic;
using UnityEngine;

public class ResourceSource : MonoBehaviour
{
    public ResourceManager resourceManager;
    public UpgradeDatabase upgradesDatabase;
    public float SourceTotal = 500f;
    public float Yield = 1f;
    public float totalDrainPerSecond_export = 0f;
    public string resourceName = "Resource";
    public string evolveName = null;

    private List<ResourcePath> connectedPaths = new List<ResourcePath>();

    Upgrades currentUpgrade;
    bool isEvolving = false;

    private void FixedUpdate()
    {
        if (connectedPaths.Count == 0)
            return;

        float totalDrainPerSecond = 0f;
        
        foreach (var path in connectedPaths)
        {
            totalDrainPerSecond += 0.02f * path.GetRate();
        }

        totalDrainPerSecond *= Yield;

        if (CompareTag("Resource"))
        {
            SourceTotal -= totalDrainPerSecond;
            resourceManager.totalResource += totalDrainPerSecond;
            totalDrainPerSecond_export = totalDrainPerSecond / 0.02f;
        }

        if (CompareTag("Evolve"))
        {
            if (!string.IsNullOrEmpty(evolveName) && !isEvolving)
            {
                currentUpgrade = upgradesDatabase.Get(evolveName);

                if (currentUpgrade != null)
                {
                    SourceTotal = currentUpgrade.cost;
                    resourceName = currentUpgrade.displayName;
                    isEvolving = true;
                }
            }

            if (isEvolving)
            {
                SourceTotal -= totalDrainPerSecond;
                resourceManager.totalResource -= totalDrainPerSecond;
                totalDrainPerSecond_export = totalDrainPerSecond / 0.02f;
            }
        }

        if (SourceTotal <= 0)
        {
            if (CompareTag("Evolve"))
            {
                currentUpgrade.ApplyUpgrade(resourceManager);
            }
            DestroySource();
        } 
    }

    public void RegisterPath(ResourcePath path)
    {
        Debug.Log($"RegisterPath called on '{name}' (tag='{tag}') with path='{(path != null ? path.name : "null")}'.");

        if (!connectedPaths.Contains(path))
        {
            connectedPaths.Add(path);
            Debug.Log($"Path '{path.name}' registered to source '{name}'. ConnectedPaths count = {connectedPaths.Count}.");
            if (CompareTag("Evolve"))
            {
                if (connectedPaths.Count <= 1)
                {
                    EvolveSelectUI evolveSelectUI = GetComponentInChildren<EvolveSelectUI>(true);
                    evolveSelectUI.ShowSelectUI(this);
                }
            }
        }
        else
        {
            Debug.Log($"Path '{path.name}' was already connected to source '{name}'.");
        }
    }

    public void DestroySource()
    {
        Debug.Log("Source Depleted! Destroying " + connectedPaths.Count + " paths.");

        // Destroy all connected paths
        // We iterate backwards when removing/destroying from a list to avoid index errors
        for (int i = connectedPaths.Count - 1; i >= 0; i--)
        {
            if (connectedPaths[i] != null)
            {
                if (connectedPaths[i].activeWorkers > 0)
                {
                    resourceManager.workers += connectedPaths[i].activeWorkers; // refund workers
                }
                Destroy(connectedPaths[i].gameObject);
            }
        }

        // Clear list
        connectedPaths.Clear();

        // Destroy the source itself
        Destroy(gameObject);
    }
}
