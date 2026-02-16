using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;

    [Header("Resources")]
    public float totalResource = 50;

    [Header("Workers")]
    public int workers = 0;
    public float workerCost = 5f;

    [Header("Scout")]
    public float scoutCost = 5f;
    public float scoutSpeed = 10f;
    public float scoutEnergy = 5f;
    public float scoutBounceGain = 1f;  

    public TMP_Text ResourcesText;
    public TMP_Text WorkersText;
    public Button AddWorkerButton;

    public TMP_Text ScoutSpeedText;
    public TMP_Text ScoutEnergyText;

    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        AddWorkerButton.onClick.AddListener(BuyWorker);

        ResourcesText.text = $"Resource: {Mathf.Floor(totalResource * 10f) / 10f:0.0}";
        WorkersText.text = "Iddle Workers: " + workers;
        ScoutSpeedText.text = $"Scout Speed: {scoutSpeed:0.0}";
        ScoutEnergyText.text = $"Scout Energy: {scoutEnergy:0.0}";
    }

    private void FixedUpdate()
    {

        ResourcesText.text = $"Resource: {Mathf.Floor(totalResource * 10f) / 10f:0.0}";
        WorkersText.text = "Workers: " + workers;
    }

    public bool Spend(int amount)
    {
        if (totalResource < amount)
            return false;

        totalResource -= amount;
        return true;
    }

    public void Add(int amount)
    {
        totalResource += amount;
    }

    public void BuyWorker()
    {
        if (totalResource >= workerCost)
        {
            totalResource -= workerCost;
            workers += 1;
        }
    }

    public void BuyNewScout()
    {
        if (totalResource >= scoutCost*2)
        {
            totalResource -= scoutCost*2;
            // Instantiate new scout here
        }
    }

    public void ApplyModifier(
        Upgrades.StatType stat,
        Upgrades.ModifierType type,
        float value
    )
    {
        switch (stat)
        {
            case Upgrades.StatType.ScoutSpeed:
                Modify(ref scoutSpeed, type, value);
                break;

            case Upgrades.StatType.ScoutEnergy:
                Modify(ref scoutEnergy, type, value);
                break;

            case Upgrades.StatType.ScoutCost:
                Modify(ref scoutCost, type, value);
                break;

            case Upgrades.StatType.WorkerCost:
                Modify(ref workerCost, type, value);
                break;

            case Upgrades.StatType.ScoutBounceGain:
                Modify(ref scoutBounceGain, type, value);
                break;
        }
    }

    void Modify(ref float stat, Upgrades.ModifierType type, float value)
    {
        if (type == Upgrades.ModifierType.Add)
            stat += value;
        else if (type == Upgrades.ModifierType.Multiply)
            stat *= value;
    }

}
