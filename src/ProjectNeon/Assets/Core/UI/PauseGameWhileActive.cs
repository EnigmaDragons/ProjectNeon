using UnityEngine;

public sealed class PauseGameWhileActive : MonoBehaviour
{
    private void OnEnable()
    {
        Time.timeScale = 0;
        Message.Publish(new GamePaused());
    }

    private void OnDisable()
    {
        Time.timeScale = 1;
        Message.Publish(new GameContinued());
    }
}
