using TMPro;
using UnityEngine;

public class DeckBuilderNavigator : MonoBehaviour
{
    [SerializeField] private DeckBuilderState state;
    [SerializeField] private Navigator navigator;
    [SerializeField] private TextMeshProUGUI errorText;

    private void OnEnable()
    {
        errorText.enabled = false;
    }

    public void NavigateToSquadSelection()
    {
        if (state.HasDeckMinimumSize())
            navigator.NavigateToGameScene();
        else
        {
            errorText.text = "You have to choose at least " + state.MinimumDeckSize + " cards for your deck!";
            errorText.enabled = true;
        }
    }

}
