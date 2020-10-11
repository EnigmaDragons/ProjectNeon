using UnityEngine;

[CreateAssetMenu(menuName = "GameContent/Animation")]
public class AnimationData : ScriptableObject
{
    [SerializeField] private StringReference animationName;
    [SerializeField] private IntReference intAmount;
    [SerializeField] private FloatReference timeAmount;

    public string AnimationName => animationName;
    public int IntAmount => intAmount;
    public float TimeAmount => timeAmount;
}