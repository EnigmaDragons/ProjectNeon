using System.Collections.Generic;
using UnityEngine;

public class BattleSetup : MonoBehaviour
{
    [SerializeField] private List<Deck> enemy;
    
    void Start()
    {
        foreach(Deck deck in enemy){
            // @todo #1:15min Reimplement Enemy Battle Setup once an Enemy deck is ready
        }
    }
}
