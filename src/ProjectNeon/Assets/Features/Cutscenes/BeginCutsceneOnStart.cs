using UnityEngine;

public class BeginCutsceneOnStart : MonoBehaviour
{
    [SerializeField] private Cutscene Cutscene;
    
    private void Start()
    {
        Message.Publish(new StartCutsceneRequested(Cutscene));
    }
}
