using System.Linq;
using UnityEditor;

public class PopulateAutoProperties
{
    [MenuItem("EnigmaDragons/Repopulate Auto Properties")]
    public static void Go()
    {
        RepopulatePartyAdventureStates();
    }

    private static void RepopulatePartyAdventureStates()
    {
        var partyAdventureStates = ScriptableExtensions.GetAllInstances<PartyAdventureState>();
        var cardTypes = ScriptableExtensions.GetAllInstances<CardType>();
        partyAdventureStates.ForEach(x =>
        {
            x.allCards = cardTypes;
            EditorUtility.SetDirty(x);
        });
    }
}
