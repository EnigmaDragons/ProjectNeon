using System;
using UnityEngine;

public class BeginCutsceneOnStart : MonoBehaviour
{
    [SerializeField] private Cutscene Cutscene;
    
    private void Start()
    {
        Log.Info($"Beginning Cutscene {Cutscene.name}");
        Message.Publish(new StartCutsceneRequested(Cutscene, Maybe<Action>.Missing()));
    }
}
