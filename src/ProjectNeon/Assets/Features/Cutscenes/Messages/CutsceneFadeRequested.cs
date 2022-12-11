
public class CutsceneFadeRequested
{
    public bool FadeIn { get; }
    public float Duration { get; }

    public CutsceneFadeRequested(bool fadeIn, float duration)
    {
        FadeIn = fadeIn;
        Duration = duration;
    }
}
