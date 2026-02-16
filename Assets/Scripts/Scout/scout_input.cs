using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.LowLevelPhysics2D;
using UnityEngine.UIElements;

public class scout_input : MonoBehaviour
{
    public LineRenderer aimLine;
    public float aimLineLength = 5f;
    public float startArea = 1f;
    public ScoutTrail scoutTrail;
    public ResourceManager resourceManager;

    public Color validAimColor = Color.white;
    public Color cancelAimColor = Color.red;

    private scout_movement movement;
    private bool isAiming = false;
    private bool isCancelHover = false;



    private void Awake()
    {
        movement = GetComponent<scout_movement>();

        if (aimLine != null )
            aimLine.enabled = false;
    }

    private void Update()
    {
        if (movement.moving)
            return;

        if (Vector2.Distance(transform.position, scoutTrail.startPosition) >= 0.05f)
            return;

        if (resourceManager.totalResource < resourceManager.scoutCost)
            return;

        //start aiming
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Collider2D hit = GetMouseHit();
            if (hit != null && hit.gameObject == gameObject)
            {
                isAiming = true;
                aimLine.enabled = true;
            }
        }


        if (!isAiming)
            return;

        // UPDATE AIM
        UpdateAimLine();

        // CHECK CANCEL HOVER
        Collider2D hoverHit = GetMouseHit();
        isCancelHover = IsCancelTarget(hoverHit);
        UpdateAimLineColor(isCancelHover);

        // RELEASE
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            if (isCancelHover)
            {
                CancelAim();
            }
            else
            {
                Fire();
                resourceManager.totalResource -= resourceManager.scoutCost;
            }
        }
    }


    private void CancelAim()
    {
        aimLine.enabled = false;
        isAiming = false;
    }

    private Collider2D GetMouseHit()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        return Physics2D.OverlapPoint(mousePos);
    }

    private bool IsCancelTarget(Collider2D hit)
    {
        if (hit == null)
            return false;

        // Cancel if releasing on the scout itself
        if (hit.gameObject == gameObject)
            return true;

       /* // Optional: cancel on start area
        if (hit.CompareTag("ScoutStart"))
            return true;
       */

        return false;
    }



    private void Fire()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 direction = mousePos - (Vector2)transform.position;

        aimLine.enabled = false;
        isAiming = false;

        UpdateAimLineColor(false); // reset color

        movement.Shoot(direction);
    }

    private void UpdateAimLine()
    {
        if (aimLine == null)
            return;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        Vector2 origin = transform.position;
        Vector2 direction = (mousePos - origin).normalized;
        
        aimLine.enabled = true;
        aimLine.positionCount = 2;

        aimLine.SetPosition(0, new Vector3(origin.x, origin.y, 0f));
        aimLine.SetPosition(1, new Vector3(
            origin.x + direction.x * aimLineLength,
            origin.y + direction.y * aimLineLength,
            0f));
    }


    private void UpdateAimLineColor(bool cancel)
    {
        if (aimLine == null)
            return;

        aimLine.startColor = cancel ? cancelAimColor : validAimColor;
        aimLine.endColor = cancel ? cancelAimColor : validAimColor;
    }
}
