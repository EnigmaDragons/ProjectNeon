using System;
using UnityEngine;
using UnityEngine.UI;

public class PartyMapToken : MonoBehaviour
{
    [SerializeField] private Image heroBust;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private Sprite backupPartyBust;

    void Awake()
    {
        if (party == null || heroBust == null)
        {
            Log.Error("Developer Error - PartyMapToken - Something is null");
            heroBust.sprite = null;
            return;
        }

        try
        {
            if (party.BaseHeroes != null && party.BaseHeroes.Length > 0 && party.BaseHeroes[0].Bust != null)
                heroBust.sprite = party.BaseHeroes[0].Bust;
            else
            {
                if (backupPartyBust != null)
                {
                    Log.NonCrashingError("Tried to draw a Party Map Token with no Party Base Heroes. Using Fallback Icon");
                    heroBust.sprite = backupPartyBust;
                }
                else
                {
                    Log.NonCrashingError("Tried to draw a Party Map Token with no Party Base Heroes. No Fallback Icon found.");
                    heroBust.sprite = null;
                }
            }
        }
        catch (Exception e)
        {
            Log.NonCrashingError(e);
        }
    }
}
