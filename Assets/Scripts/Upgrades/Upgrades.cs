using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Upgrades", menuName = "Scriptable Objects/Upgrades")]
public class Upgrades : ScriptableObject
{
    [Header("UI")]
    public string id;
    public string displayName;
    public int rarity;
    public Sprite icon;
    [TextArea] public string description;

    [Header("Research")]
    public float cost;

    [Header("Effects")]
    public List<StatModifier> modifiers;

    public enum StatType
    {
        ScoutSpeed,
        ScoutEnergy,
        ScoutCost,
        WorkerCost,
        ScoutBounceGain
    }

    public enum ModifierType
    {
        Add,
        Multiply
    }

    [System.Serializable]
    public class StatModifier
    {
        public StatType stat;
        public ModifierType type;
        public float value;
    }

    public void ApplyUpgrade(ResourceManager resourceManager)
    {
        foreach (var modifier in modifiers)
        {
            resourceManager.ApplyModifier(
                modifier.stat, 
                modifier.type, 
                modifier.value);
        }
    }
}
