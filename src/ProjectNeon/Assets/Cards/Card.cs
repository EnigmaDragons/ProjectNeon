using UnityEngine;

[CreateAssetMenu()]
public class Card : ScriptableObject
{
    [SerializeField] private Sprite Art;
    [SerializeField] private CardEffect Effect;
    [SerializeField] private string Description;
    [SerializeField] private string TypeDescription;
}
