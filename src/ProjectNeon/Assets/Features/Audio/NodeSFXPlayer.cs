
using UnityEngine;

public class NodeSFXPlayer : MonoBehaviour
{
    public ArrivedAtNode arrived;
    public MapNodeType nodeType;
    public bool whatNodeType;
    [SerializeField, FMODUnity.EventRef] private string ArrivedAtCombat;
    [SerializeField, FMODUnity.EventRef] private string ArrivedAtBoss;

    private void OnEnable ()

        {
       
        }
}
