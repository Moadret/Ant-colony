using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeDatabase", menuName = "Scriptable Objects/UpgradeDatabase")]
public class UpgradeDatabase : ScriptableObject
{
    public List<Upgrades> upgrades;

    Dictionary<string, Upgrades> lookup;

    void OnEnable()
    {
        lookup = new Dictionary<string, Upgrades>();
        foreach (var up in upgrades)
            lookup[up.id] = up;
    }

    public Upgrades Get(string id)
    {
        return lookup.TryGetValue(id, out var up) ? up : null;
    }

    public List<Upgrades> GetRandomUpgrades(int count)
    {
        // Defensive copy so we don't modify the original list
        List<Upgrades> pool = new List<Upgrades>(upgrades);
        List<Upgrades> result = new List<Upgrades>();

        // Clamp count to available upgrades
        count = Mathf.Min(count, pool.Count);

        Debug.Log($"Selecting {count} random upgrades from a pool of {pool.Count}");

        for (int pick = 0; pick < count; pick++)
        {
            // 1. Compute total weight
            int totalWeight = 0;
            foreach (var up in pool)
                totalWeight += up.rarity;

            // 2. Roll
            int roll = Random.Range(0, totalWeight);

            // 3. Find selected upgrade
            int cumulative = 0;
            Upgrades selected = null;

            foreach (var up in pool)
            {
                cumulative += up.rarity;
                if (roll < cumulative)
                {
                    selected = up;
                    break;
                }
            }

            // Safety check (should never happen)
            if (selected == null)
                selected = pool[pool.Count - 1];

            // 4. Add and remove
            result.Add(selected);
            pool.Remove(selected);
        }

        Debug.Log($"Selected upgrades: {string.Join(", ", result.Select(u => u.displayName))}");
        return result;
    }

}
