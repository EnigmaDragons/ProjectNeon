using UnityEngine;

public class RunTimer : MonoBehaviour
{
    private static RunTimer Instance { get; set; }

    public static float ConsumeElapsedTime()
    {
        if (Instance == null)
        {
            Log.Error("Run Timer Instance Is Null");
            return 0;
        }

        return Instance.ConsumeElapsedTimeInstance();
    }

    [SerializeField] private float elapsedTime;

    private void OnEnable()
    {
        if (Instance == null)
            Instance = this;
    }

    private float ConsumeElapsedTimeInstance()
    {
        var segment = elapsedTime;
        elapsedTime = 0;
        return segment;
    }

    private void Update()
    {
        if (Application.isPlaying && Application.isFocused && Time.timeScale > 0)
            elapsedTime += Time.unscaledDeltaTime;
    }
}
