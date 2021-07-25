using UnityEngine;

public sealed class PauseGameWhileActive : MonoBehaviour
{
    private float _lastTimeScale = 1;
    
    private void OnEnable()
    {
        _lastTimeScale = Time.timeScale;
        Time.timeScale = 0;
        Message.Publish(new GamePaused());
    }

    private void OnDisable()
    {
        Time.timeScale = _lastTimeScale;
        Message.Publish(new GameContinued());
    }
}
