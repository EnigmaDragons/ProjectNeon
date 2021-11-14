using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOnce/CurrentCutscene")]
public class CurrentCutscene : ScriptableObject
{
    [SerializeField] private Cutscene cutscene;
    [SerializeField] private int segmentIndex;

    public void Init(Cutscene c)
    {
        cutscene = c;
        segmentIndex = 0;
    }
}
