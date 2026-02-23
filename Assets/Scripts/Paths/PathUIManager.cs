using UnityEngine;

public class PathUIManager : MonoBehaviour
{
    public static PathUIManager Instance;

    public Transform contentParent;
    public GameObject entryPrefab;

    ResourcePath selected;

    void Awake()
    {
        Instance = this;
    }

    void OnEnable()
    {
        PathEvents.OnPathCreated += AddEntry;

        if (entryPrefab == null)
            Debug.LogError("PathUIManager: entryPrefab není nastavený v inspectoru.");

        if (contentParent == null)
            Debug.LogError("PathUIManager: contentParent není nastavený v inspectoru.");

        var existingPaths = FindObjectsOfType<ResourcePath>();
        Debug.Log($"PathUIManager: nalezeno {existingPaths.Length} existujících ResourcePath instancí.");
        foreach (var existing in existingPaths)
        {
            AddEntry(existing);
        }
    }

    void OnDisable()
    {
        PathEvents.OnPathCreated -= AddEntry;
    }

    void AddEntry(ResourcePath path)
    {
        if (path == null)
        {
            Debug.LogWarning("PathUIManager.AddEntry: pøedán null path.");
            return;
        }

        if (string.IsNullOrEmpty(path.Name) && path.Length <= 0f)
        {
            Debug.Log($"PathUIManager.AddEntry: pøeskoèeno neinicializované ResourcePath (objekt {path.gameObject.name}).");
            return;
        }

        if (entryPrefab == null || contentParent == null)
        {
            Debug.LogError("PathUIManager.AddEntry: chybí entryPrefab nebo contentParent, nelze vytvoøit UI položku.");
            return;
        }

        GameObject go;
        try
        {
            go = Instantiate(entryPrefab, contentParent, false);

            var rt = go.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.localScale = Vector3.one;
                rt.localRotation = Quaternion.identity;
                rt.anchoredPosition = Vector2.zero;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"PathUIManager.AddEntry: Instantiate selhalo: {ex.Message}");
            return;
        }

        var entry = go.GetComponent<PathEntryUI>();
        if (entry == null)
        {
            Debug.LogError("PathUIManager.AddEntry: entryPrefab neobsahuje komponentu PathEntryUI.");
            return;
        }

        Debug.Log($"PathUIManager.AddEntry: vytváøím UI záznam pro path '{path.Name}' (Length={path.Length:F1}).");
        entry.Initialize(path);
    }

    public void Select(ResourcePath path)
    {
        if (selected != null)
            selected.Highlight(false);

        selected = path;
        selected.Highlight(true);
    }
}