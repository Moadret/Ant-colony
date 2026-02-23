using Unity.VisualScripting;
using UnityEngine;

public class CameraSystem : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float dragSpeed = 0.01f;

    private Vector2 moveInput;
    private Vector2 panDelta;
    private bool isPanning;

    private DefaultInputActions input;

    private void Awake()
    {
        input = new DefaultInputActions();

        input.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        input.Player.Move.canceled += _ => moveInput = Vector2.zero;

        input.Player.Pan.performed += ctx => panDelta = ctx.ReadValue<Vector2>();
        input.Player.Pan.canceled += _ => panDelta = Vector2.zero;

        input.Player.PanButton.performed += _ => isPanning = true;
        input.Player.PanButton.canceled += _ => isPanning = false;
    }

    private void OnEnable() => input.Enable();
    private void OnDisable() => input.Disable();

    private void Update()
    {
        Vector3 movement = new Vector3(moveInput.x, moveInput.y, 0f);

        if (isPanning)
        {
            // Invert so dragging feels natural
            movement += new Vector3(-panDelta.x, -panDelta.y, 0f) * dragSpeed;
        }

        transform.position += movement * moveSpeed * Time.unscaledDeltaTime;   //unaffected by slow time
    }
}
