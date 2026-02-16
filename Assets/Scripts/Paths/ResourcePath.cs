using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static Unity.VisualScripting.Member;

public class ResourcePath : MonoBehaviour
{
    public List<Vector2> path;
    public ResourceManager resourceManager;

    public float Length;
    public float Yield = 1f;
    public string Name;
    public int activeWorkers = 0;


    public LineRenderer Line { get; private set; } 

    void Update()
    {

    }

 
    private void FixedUpdate()
    {

    }
 


    public void Initialize(List<Vector2> newPath, string name, ResourceSource source)
    {
        path = newPath;
        Name = name;
        CalculateLength();

        resourceManager = ResourceManager.Instance;
        Line = GetComponent<LineRenderer>();

        PathEvents.OnPathCreated?.Invoke(this); // name, Length
        
        if (source != null)
        {
            source.RegisterPath(this);
        }

    }

    void CalculateLength()
    {
        Length = 0f;
        for (int i = 1; i < path.Count; i++)
            Length += Vector2.Distance(path[i - 1], path[i]);
    }

    public float GetRate() // returns resources per 1s 
    {
        float Rate =  1f / Length * activeWorkers;
        return Rate;
    }


    public void Highlight(bool on)
    {
        Line.startColor = on ? Color.yellow : Color.white;
        Line.endColor = on ? Color.yellow : Color.white;
    }

    public void AddWorkerToPath()
    {
        if (resourceManager.workers >= 1)
        {
            resourceManager.workers -= 1;
            activeWorkers += 1;
        }
    }

    public void RemoveWorkerFromPath()
    {
        if (activeWorkers >= 1)
        {
            resourceManager.workers += 1;
            activeWorkers -= 1;
        }
    }

}
