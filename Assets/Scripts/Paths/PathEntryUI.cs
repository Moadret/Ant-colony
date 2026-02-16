using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PathEntryUI : MonoBehaviour
{
    public TextMeshProUGUI label;
    public Button selectButton;
    public Button deleteButton;
    public Button AddWorkerToPath;
    public Button RemoveWorkerFromPath;
    public TextMeshProUGUI PathFlow;
    public TextMeshProUGUI PathWorkersUI;

    ResourcePath path;

    public void Initialize(ResourcePath rp)
    {
        if (rp == null)
        {
            Debug.LogWarning("PathEntryUI.Initialize: rp je null.");
            return;
        }

        path = rp;

        // Fallback: najít TextMeshProUGUI v potomcích, pokud není nastaveno v inspectoru
        if (label == null)
        {
            label = GetComponentInChildren<TextMeshProUGUI>();
            if (label == null)
            {
                Debug.LogError("PathEntryUI.Initialize: label (TextMeshProUGUI) není nastaven v prefab a nebyl nalezen mezi potomky.");
            }
        }




        // Bezpeèné nastavení textu (pokud label existuje)
        if (label != null)
            label.text = $"{(string.IsNullOrEmpty(rp.Name) ? "Unnamed" : rp.Name)} | {rp.Length:F1}";

        if (PathFlow != null)
            PathFlow.text = $"{rp.GetRate()} r/s";

        if (PathFlow != null)
            PathWorkersUI.text = $"Workers: {rp.activeWorkers}";

        // Pøidání listenerù jen pokud tlaèítka existují
        if (selectButton != null)
            selectButton.onClick.AddListener(OnSelect);
        else
            Debug.LogWarning("PathEntryUI.Initialize: selectButton není nastaven.");

        if (deleteButton != null)
            deleteButton.onClick.AddListener(OnDelete);
        else
            Debug.LogWarning("PathEntryUI.Initialize: deleteButton není nastaven.");

        if (AddWorkerToPath != null)
            AddWorkerToPath.onClick.AddListener(rp.AddWorkerToPath);

        if (RemoveWorkerFromPath != null)
            RemoveWorkerFromPath.onClick.AddListener(rp.RemoveWorkerFromPath);
    }

    public void Update()
    {
        if (PathFlow != null)
            PathWorkersUI.text = $"Workers: {path.activeWorkers}";

        if (PathFlow != null)
            PathFlow.text = $"{path.GetRate()} r/s";
    }


    void OnSelect()
    {
        if (PathUIManager.Instance == null)
        {
            Debug.LogWarning("PathEntryUI.OnSelect: PathUIManager.Instance je null.");
            return;
        }

        PathUIManager.Instance.Select(path);
    }

    void OnDelete()
    {
        if (path != null)
            Destroy(path.gameObject);

        Destroy(gameObject);
    }
}
