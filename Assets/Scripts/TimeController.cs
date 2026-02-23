using UnityEngine;

public class TimeController : MonoBehaviour
{
    public static TimeController Instance;

    [Range(0.01f, 1f)]
    public float slowTimeScale = 0.3f;

    private float normalFixedDeltaTime;

    private DefaultInputActions input;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        normalFixedDeltaTime = Time.fixedDeltaTime;

        input = new DefaultInputActions();

        input.Enable();
        input.Player.Slow.started += ctx => SetSlowMotion(true);
        input.Player.Slow.canceled += ctx => SetSlowMotion(false);
    }

    public void SetSlowMotion(bool enabled)
    {
        if (enabled)
        {
            Time.timeScale = slowTimeScale;
            Time.fixedDeltaTime = normalFixedDeltaTime * slowTimeScale;
        }
        else
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = normalFixedDeltaTime;
        }
    }

}