using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckBuilderNavigator : Navigator
{
    [SerializeField] private DeckBuilderState state;
    [SerializeField] private Navigator navigator;

    public new void NavigateToSquadSelection()
    {
        if (this.state.HasDeckMinimumSize())
            navigator.NavigateToSquadSelection();
        else
        {
            Debug.Log("You have to choose at least " + state.MinimumDeckSize + " cards for your deck!");
            /**
             * @todo #284:30min Define and implement how the message about the minimum deck size
             *  must be shown to the player. Then remove the debug.log line which prints the message to
             *  console.
             */
        }
    }

}
