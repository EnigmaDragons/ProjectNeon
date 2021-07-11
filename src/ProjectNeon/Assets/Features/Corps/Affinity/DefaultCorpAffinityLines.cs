using UnityEngine;

[CreateAssetMenu(menuName = "GameContent/CorpDefaultAffinityLines")]
public class DefaultCorpAffinityLines : ScriptableObject
{
    [SerializeField] private CorpAffinityLines lines;

    public CorpAffinityLines Lines => lines;
}
