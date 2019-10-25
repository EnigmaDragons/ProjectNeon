using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu]
public class DeckStorage : ScriptableObject
{
    private List<Deck> _decks;

    public void AddDeck(Deck deck)
    {
        _decks.Add(deck);
    }

    public void DeleteDeck(Deck deck)
    {
        _decks.Remove(deck);
    }

    public List<Deck> GetDecks()
    {
        _decks = _decks.Where(x => x != null).ToList();
        return AssetDatabase.FindAssets("t:" + typeof(Deck).Name)
            .Select(x => AssetDatabase.LoadAssetAtPath<Deck>(AssetDatabase.GUIDToAssetPath(x)))
            .Concat(_decks)
            .ToList();
    }
}
