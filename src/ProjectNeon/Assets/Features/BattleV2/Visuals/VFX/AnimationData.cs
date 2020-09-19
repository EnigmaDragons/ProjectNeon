using UnityEngine;

[CreateAssetMenu(menuName = "GameContent/Animation")]
public class AnimationData : ScriptableObject
{
    [SerializeField] private string animationName;
    [SerializeField] private AnimationPosition position;
    [SerializeField] private Vector2 offset;
    [SerializeField] private bool flipped;

    public string AnimationName => animationName;
    public AnimationPosition Position => position;
    public Vector2 Offset => offset;
    public bool Flipped => flipped;
}