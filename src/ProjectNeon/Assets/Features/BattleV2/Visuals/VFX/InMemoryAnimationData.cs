
public class InMemoryAnimationData : IAnimationData
{
    public string AnimationName { get; }
    public int NumTimes { get; }
    public float TimeAmount { get; }

    public InMemoryAnimationData(string animationName, float timeAmount, int numTimes = 1)
    {
        AnimationName = animationName;
        TimeAmount = timeAmount;
        NumTimes = numTimes;
    }
}
