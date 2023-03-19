using System;
using UnityEngine;

public class InitFmodAudio : MonoBehaviour
{
    [SerializeField] private FmodGameAudioManager fmodAudio;

    private void Awake() => AttemptInit(0);

    private void AttemptInit(int numPastTries)
    {       
        try
        {
            fmodAudio.Init();
        }
        catch (Exception e)
        {
            if (numPastTries == 5) 
                Log.Error(e);
            if (numPastTries < 5)
                this.ExecuteAfterDelay(() => AttemptInit(numPastTries + 1), 0.1f);
            if (numPastTries >= 5 && numPastTries < 12)
                this.ExecuteAfterDelay(() => AttemptInit(numPastTries + 1), 0.6f);
        }
    }
}
