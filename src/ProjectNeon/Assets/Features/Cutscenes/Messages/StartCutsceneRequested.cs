using System;

public class StartCutsceneRequested
{
    public Cutscene Cutscene { get; }
    public Maybe<Action> OnFinished { get; }

    public StartCutsceneRequested(Cutscene cutscene, Maybe<Action> onFinished)
    {
        Cutscene = cutscene;
        OnFinished = onFinished;
    }
}
