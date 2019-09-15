using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSetup : MonoBehaviour
{
    // Start is called before the first frame update
    void Start(Deck player, List<Deck> enemy)
    {
        player.Shuffle();
        foreach(Deck deck in enemy){
            deck.Shuffle();
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
