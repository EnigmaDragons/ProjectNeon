using System.Collections.Generic;
using UnityEngine;

public class CutscenePresenter : OnMessage<AdvanceCutsceneRequested>
{
    [SerializeField] private CurrentCutscene cutscene;
    [SerializeField] private GameObject settingParent;

    private readonly List<CutsceneCharacter> _characters = new List<CutsceneCharacter>();
    
    private void Start()
    {
        _characters.Clear();
        cutscene.Current.Setting.SpawnTo(settingParent);
        var characters = settingParent.GetComponentsInChildren<CutsceneCharacter>();
        characters.ForEach(c => _characters.Add(c));
        Log.Info($"{_characters.Count} Characters in cutscene");
    }
    
    protected override void Execute(AdvanceCutsceneRequested msg)
    {
        throw new System.NotImplementedException();
    }
}
