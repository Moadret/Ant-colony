using System.Resources;
using UnityEngine;
using UnityEngine.InputSystem;

public class ResourceSourceClickable : MonoBehaviour
{
    private ResourceSource source;
    private ResourceInfoUI infoUI;

    private Camera cam;
    private DefaultInputActions input;

    private void Awake()
    {
        source = GetComponent<ResourceSource>();
        infoUI = GetComponentInChildren<ResourceInfoUI>(true);
        cam = Camera.main;
        input = new DefaultInputActions();

        if (infoUI == null)
            Debug.LogError($"[{name}] Missing ResourceInfoUI in children!");

        if (source == null)
            Debug.LogError($"[{name}] Missing ResourceSource!");
    }


    private void OnEnable()
    {
        input.Enable();
        input.Player.Click.performed += OnClick;
    }

    private void OnDisable()
    {
        input.Player.Click.performed -= OnClick;
        input.Disable();
    }

    private void OnClick(InputAction.CallbackContext ctx)
    {
        Vector2 screenPos = Pointer.current.position.ReadValue();
        Vector2 worldPos = cam.ScreenToWorldPoint(screenPos);

        RaycastHit2D hit =
            Physics2D.Raycast(worldPos, Vector2.zero);

        if (hit.collider != null && hit.collider.gameObject == gameObject)
        {
            Debug.Log("Resource clicked (Input System)");
            if (infoUI.gameObject.activeSelf)
            {
                infoUI.Hide();
            }
            else
            {
                infoUI.Show(source);
            }
        }
    }
}