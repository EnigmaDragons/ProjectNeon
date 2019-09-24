using UnityEngine;

public class Member : Target
{
    // @todo #1:30min We have a circular reference between Member and Target. Evolve the architecture;

    [SerializeField] private Character character;
    [SerializeField] private Deck deck;
    [SerializeField] public int hp;

    /**
     * @todo #55:30min This constructor does not provide values to all class members. Create a default value for
     *  Deck which may be used here, and define default hit points for this case.
     */
    public Member Init(Character character)
    {
        this.character = character;
        return this;
    }

    public Deck GetDeck()
    {
        return deck;
    }

    // @todo #54:30min hp property should not be accessed and mutated by other classes. Create accessors for this one so we can set up reactive bindings.
}
