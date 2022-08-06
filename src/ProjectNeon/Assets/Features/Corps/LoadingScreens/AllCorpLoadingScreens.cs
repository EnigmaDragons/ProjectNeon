using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOnce/AllCorpLoadingScreens")]
public class AllCorpLoadingScreens : ScriptableObject
{
    public CorpLoadingScreen[] allScreens = new CorpLoadingScreen[0]; 
    [SerializeField] private CorpLoadingScreen last;

    public CorpLoadingScreen GetRandomNext()
    {
        var selected = allScreens.Except(last).Random(DeterministicRng.CreateRandom());
        last = selected;
        return selected;
    }
}
