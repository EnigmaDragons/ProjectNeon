using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroSelector : MonoBehaviour
{
    [SerializeField] private CharacterController controller;
    [SerializeField] private DeckBuilderState state;

    public void Update()
    {
        //state.CurrentHero = CharacterController.currentCharacter;
        /**
         * @todo #282:30min At the time we don't have a Hero reference in 
         * CharacterController which points to the current selected Hero.
         * CharacterController is the responsible for Hero change event in
         * DeckBuilder. As it is obsolete, we should provide another way of 
         * controlling the hero currently selected.
         */
    }
}
