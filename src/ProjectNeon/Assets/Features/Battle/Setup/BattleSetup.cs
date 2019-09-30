using System.Collections.Generic;
using UnityEngine;

public class BattleSetup : MonoBehaviour
{
    [SerializeField]
    List<Deck> enemy;

    void Start()
    {
        foreach(Deck deck in enemy){
            deck.Shuffle();
        }
    }
}
