using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeckBuilderNavigator : Navigator
{
    [SerializeField] private DeckBuilderState state;
    [SerializeField] private Navigator navigator;
    [SerializeField] private TextMeshProUGUI errorText;

    private void OnEnable()
    {
        errorText.enabled = false;
    }

    public new void NavigateToSquadSelection()
    {
        if (this.state.HasDeckMinimumSize())
            navigator.NavigateToSquadSelection();
        else
        {
            errorText.text = "You have to choose at least " + state.MinimumDeckSize + " cards for your deck!";
            errorText.enabled = true;

            /**
             * @todo #284:30min Define and implement how the message about the minimum deck size
             *  must be shown to the player. Then remove the debug.log line which prints the message to
             *  console.
             */
        }
    }

}
