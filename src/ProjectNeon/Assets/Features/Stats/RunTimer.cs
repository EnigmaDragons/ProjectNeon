using UnityEngine;

public class RunTimer : MonoBehaviour
{
    [SerializeField] private CurrentRunStats runStats;

    private void Update()
    {
        if (Time.timeScale > 0)
            runStats.TotalElapsed += Time.unscaledDeltaTime;
    }
}
