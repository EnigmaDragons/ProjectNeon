using UnityEngine;

[CreateAssetMenu(menuName = "GameContent/Animation")]
public class AnimationData : ScriptableObject, IAnimationData
{
    [SerializeField] private StringReference animationName;
    [SerializeField] private IntReference intAmount;
    [SerializeField] private FloatReference timeAmount;

    public string AnimationName => animationName;
    public int NumTimes => intAmount;
    public float TimeAmount => timeAmount;
}