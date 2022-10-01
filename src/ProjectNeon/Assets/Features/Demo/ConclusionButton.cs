using UnityEngine;

public class ConclusionButton : MonoBehaviour
{
    [SerializeField] private Navigator navigator;
    [SerializeField] private BoolReference isDemo;

    public void NavigateAway()
    {
        if (CurrentProgressionData.Data.HasShownWishlistScene || !isDemo.Value)
        {
            navigator.NavigateToTitleScreen();
        }
        else
        {
            CurrentProgressionData.Mutate(x => x.HasShownWishlistScene = true);
            navigator.NavigateToWishlistScene();
        }
    }
}