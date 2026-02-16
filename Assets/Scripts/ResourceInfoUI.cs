using TMPro;
using UnityEngine;

public class ResourceInfoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI resourceNameText;
    [SerializeField] private TextMeshProUGUI productionText;
    [SerializeField] private TextMeshProUGUI remainingText;

    private ResourceSource currentSource;

    private void Awake()
    {
        gameObject.SetActive(false); // hidden by default
    }


    public void Show(ResourceSource source)
    {
        currentSource = source;
        gameObject.SetActive(true);
        UpdateUI();
    }

    public void Hide()
    {
        currentSource = null;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (currentSource != null)
            UpdateUI();
    }

    private void UpdateUI()
    {
        resourceNameText.text = currentSource.resourceName;
        productionText.text =
            $"Production: +{currentSource.totalDrainPerSecond_export:F1} / sec";
        remainingText.text =
            $"Remaining: {currentSource.SourceTotal:F0} r";
    }
}
