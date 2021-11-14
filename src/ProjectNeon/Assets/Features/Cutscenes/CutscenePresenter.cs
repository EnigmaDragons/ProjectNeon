
using UnityEngine;

public class CutscenePresenter : OnMessage<AdvanceCutsceneRequested>
{
    [SerializeField] private CurrentCutscene cutscene;
    [SerializeField] private GameObject settingParent;
    
    private void Start()
    {
        cutscene.Current.Setting.SpawnTo(settingParent);    
    }
    
    protected override void Execute(AdvanceCutsceneRequested msg)
    {
        throw new System.NotImplementedException();
    }
}
