using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BattleSetup : MonoBehaviour
{
    [SerializeField]
    Deck player;

    [SerializeField]
    List<Deck> enemy;

    // Start is called before the first frame update
    void Start()
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
