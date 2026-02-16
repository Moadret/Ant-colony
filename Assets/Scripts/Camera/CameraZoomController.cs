using Unity.Cinemachine;
using UnityEngine;

public class CameraZoomController : MonoBehaviour
{
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float minZoom = 3f;
    [SerializeField] private float maxZoom = 20f;

    [SerializeField] private CinemachineCamera[] cameras;

    private DefaultInputActions input;
    private float currentZoom;

    private void Awake()
    {
        input = new DefaultInputActions();

        // Initialize zoom from first camera
        if (cameras.Length > 0)
            currentZoom = cameras[0].Lens.OrthographicSize;
    }

    private void OnEnable() => input.Enable();
    private void OnDisable() => input.Disable();

    private void Update()
    {
        float scroll = input.Player.Zoom.ReadValue<float>();
        if (Mathf.Abs(scroll) < 0.01f)
            return;

        currentZoom -= scroll * zoomSpeed * Time.deltaTime;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

        ApplyZoom();
    }

    private void ApplyZoom()
    {
        foreach (var cam in cameras)
        {
            cam.Lens.OrthographicSize = currentZoom;
        }
    }
}
