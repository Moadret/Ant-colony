using NUnit.Framework;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;


public class ScoutTrail : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public float recordDistance = 0.1f;
    public float rewindSpeed = 5f;
    public DrawManager drawManager;


    private scout_movement movement;
    private List<Vector2> positions = new List<Vector2>();
    private bool rewinding = false;
    private int rewindIndex;
    
    public Vector2 startPosition { get; private set; }

    private void Awake()
    {
        movement = GetComponent<scout_movement>();
        startPosition = transform.position;
    }


    private void Update()
    {
        //for now press R to rewind
        if (Keyboard.current.rKey.wasPressedThisFrame && !movement.moving && positions.Count > 1)
        {
            drawManager.ClearAllLines();
            rewinding = true;
            rewindIndex = positions.Count - 1;
        }
    }

    private void FixedUpdate()
    {
        if (movement.moving && !rewinding)
        {
            Vector2 currentPos = transform.position;

            if (positions.Count == 0 || Vector2.Distance(positions[positions.Count - 1], currentPos) >= recordDistance)
            {
                positions.Add(currentPos);
                UpdateLineRenderer();
            }
        }




        if (rewinding)
        {
            RewindStep();
        }



    }

    private void RewindStep()
    {
        if (rewindIndex <= 0)
        {
            rewinding = false;

            ClearPath();

            return;
        }

        Vector2 targetPos = positions[rewindIndex - 1];
        Vector2 moveDir = (targetPos - (Vector2)transform.position).normalized;
        float step = rewindSpeed* Time.fixedDeltaTime;

        if (Vector2.Distance(transform.position, targetPos) <= step)
        {
            transform.position = targetPos;
            rewindIndex--;
        }
        else
        {
            transform.position += (Vector3)(moveDir * step);
        }
    }

    private void UpdateLineRenderer()
    {
        lineRenderer.positionCount = positions.Count;
        for (int i = 0; i < positions.Count; i++)
        {
            lineRenderer.SetPosition(i, positions[i]);
        }
    }

    public List<Vector2> GetPath()
    {
        return new List<Vector2>(positions);
    }

    public void ClearPath()
    {
        positions.Clear();

        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 0;
        }
    }


}
