using UnityEngine;

public class OnActiveFor10MinutesRecordAchievement : MonoBehaviour
{
    [SerializeField] private string achievementId;
    
    private bool _finished;
    private float _elapsedSeconds;
    
    private void OnEnable()
    {
        _elapsedSeconds = 0;
    }

    private void Update()
    {
        if (string.IsNullOrWhiteSpace(achievementId) || _finished)
            return;
        
        _elapsedSeconds += Time.unscaledDeltaTime;
        if (!(_elapsedSeconds >= 10 * 60)) 
            return;
        
        _finished = true;
        Achievements.Record(achievementId);
    }
}
