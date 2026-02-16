using System.Collections.Generic;
using UnityEngine;

public class PathDatabase : MonoBehaviour
{
    public static PathDatabase Instance;
    public ResourcePath ResourcePath;


    public Material pathMaterial;
    public float lineWidth = 0.1f;


    private List<List<Vector2>> storedPaths = new List<List<Vector2>>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void AddPath(List<Vector2> path, ResourceSource source)
    {
        if (path == null || path.Count < 2)
            return;

        storedPaths.Add(new List<Vector2>(path)); // store a copy

        DrawPath(path, source);

        Debug.Log("Path stored. Total: " + storedPaths.Count);
    }

    void DrawPath(List<Vector2> path, ResourceSource source)
    {
        // Log source info to debug connection problems
        Debug.Log($"DrawPath called. source={source}");

        ResourcePath.Name = "StoredPath" + storedPaths.Count;
        GameObject lineObj = new GameObject( ResourcePath.Name );
        lineObj.transform.SetParent(transform);

        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lr.positionCount = path.Count;

        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.useWorldSpace = true;

        lr.material = pathMaterial;

        for (int i = 0; i < path.Count; i++)
            lr.SetPosition(i, path[i]);

        // Add ResourcePath
        ResourcePath rp = lineObj.AddComponent<ResourcePath>();
        rp.Initialize(path, ResourcePath.Name, source);

    }


    public List<Vector2> GetPath(int index)
    {
        if (index < 0 || index >= storedPaths.Count)
            return null;

        return new List<Vector2>(storedPaths[index]);
    }

    public int PathCount => storedPaths.Count;
}
