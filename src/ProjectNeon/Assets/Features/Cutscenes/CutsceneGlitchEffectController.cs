using UnityEngine;

public class CutsceneGlitchEffectController : OnMessage<CutsceneGlitchEffectRequested>
{
    [SerializeField] private GameObject glitchEffect;

    private void Awake() => glitchEffect.SetActive(false);
    
    protected override void Execute(CutsceneGlitchEffectRequested msg)
    {
        Log.Info($"Cutscene Glitch Effect - {msg.Active}");
        glitchEffect.SetActive(msg.Active);
        Message.Publish(new Finished<ShowCutsceneSegment>());
    }
}
