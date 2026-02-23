using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DrawManager : MonoBehaviour
{
    public GameObject linePrefab;
    public float maxLineLength = 5f;
    public int maxUses = 3;

    private float currentLength;
    private int remainingUses;

    private DrawnLine activeLine;
    private Camera cam;
    private DefaultInputActions input;

    private bool slowHeld;
    private bool clickHeld;

    private List<DrawnLine> spawnedLines = new List<DrawnLine>();

    void Awake()
    {
        input = new DefaultInputActions();

        input.Player.Slow.started += ctx => slowHeld = true;
        input.Player.Slow.canceled += ctx => slowHeld = false;

        input.Player.Attack.started += OnClickStarted;
        input.Player.Attack.canceled += OnClickCanceled;
    }

    void OnEnable()
    {
        input.Enable();
    }

    void OnDisable()
    {
        input.Disable();
    }

    void Start()
    {
        cam = Camera.main;
        remainingUses = maxUses;
    }

    void Update()
    {
        if (activeLine != null && slowHeld && clickHeld)
        {
            UpdateLine();
        }
    }

    void OnClickStarted(InputAction.CallbackContext ctx)
    {
        clickHeld = true;

        if (slowHeld && remainingUses > 0)
        {
            StartLine();
        }
    }

    void OnClickCanceled(InputAction.CallbackContext ctx)
    {
        clickHeld = false;

        if (activeLine != null)
        {
            EndLine();
        }
    }

    void StartLine()
    {
        GameObject lineObj = Instantiate(linePrefab);
        activeLine = lineObj.GetComponent<DrawnLine>();

        spawnedLines.Add(activeLine);

        currentLength = 0f;
        activeLine.AddPoint(GetMouseWorldPosition());
    }

    void EndLine()
    {
        activeLine = null;
        remainingUses--;
    }

    void UpdateLine()
    {
        Vector2 mousePos = GetMouseWorldPosition();
        Vector2 lastPoint = activeLine.GetLastPoint();

        float distance = Vector2.Distance(lastPoint, mousePos);

        if (currentLength + distance > maxLineLength)
        {
            float remaining = maxLineLength - currentLength;
            Vector2 direction = (mousePos - lastPoint).normalized;
            Vector2 finalPoint = lastPoint + direction * remaining;

            activeLine.AddPoint(finalPoint);
            EndLine();
            return;
        }

        if (distance > 0.1f)
        {
            activeLine.AddPoint(mousePos);
            currentLength += distance;
        }
    }

    Vector2 GetMouseWorldPosition()
    {
        Vector2 mouse = Mouse.current.position.ReadValue();
        return cam.ScreenToWorldPoint(mouse);
    }

    public void ClearAllLines()
    {
        foreach (var line in spawnedLines)
        {
            if (line != null)
                Destroy(line.gameObject);
        }

        spawnedLines.Clear();
        remainingUses = maxUses;
    }
}