using UnityEngine;

public class Member : Target
{
    // @todo #1:30min We have a circular reference between Member and Target. Evolve the architecture;

    [SerializeField] private Character character;
    [SerializeField] private Deck deck;
    [SerializeField] public int hp;

    // @todo #54:30min hp property should not be accessed and mutated by other classes. Create accessors for this one so we can set up reactive bindings.
}
