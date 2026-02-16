using Unity.Cinemachine;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField] private CinemachineCamera mainCamera;
    [SerializeField] private CinemachineCamera scoutCamera;
    [SerializeField] private Transform mainCameraTarget;

    [SerializeField] private int activePriority = 20;
    [SerializeField] private int inactivePriority = 0;

    private DefaultInputActions input;

    private void Awake()
    {
        input = new DefaultInputActions();

        input.Player.ScoutView.performed += _ => EnterScoutView();
        input.Player.ScoutView.canceled += _ => ExitScoutView();
    }

    private void OnEnable() => input.Enable();
    private void OnDisable() => input.Disable();

    private void EnterScoutView()
    {
        scoutCamera.Priority = activePriority;
        mainCamera.Priority = inactivePriority;
    }

    private void ExitScoutView()
    {
        // Align main camera target to scout camera position
        mainCameraTarget.position = scoutCamera.transform.position;

        // Switch back
        scoutCamera.Priority = inactivePriority;
        mainCamera.Priority = activePriority;
    }
}
