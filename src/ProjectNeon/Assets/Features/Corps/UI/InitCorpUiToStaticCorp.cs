using UnityEngine;

public class InitCorpUiToStaticCorp : MonoBehaviour
{
    [SerializeField] private StaticCorp corp;
    [SerializeField] private CorpUiBase[] uiElements;

    private void Awake() => uiElements.ForEach(u => u.Init(corp));
}
