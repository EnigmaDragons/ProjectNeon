public class StartCutsceneRequested
{
    public Cutscene Cutscene { get; }

    public StartCutsceneRequested(Cutscene cutscene) => Cutscene = cutscene;
}
