using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Member : Target
{
    [SerializeField] private Character character;
    [SerializeField] private Deck deck;
    [SerializeField] public int hp;
    /**
     * @todo #54:30min hp property should not be accessed and mutated by other classes. Create accessors
     * for this one so we can set up reactive bindings.
     */

}
