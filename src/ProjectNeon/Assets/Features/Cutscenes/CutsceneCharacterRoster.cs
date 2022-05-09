using System;
using UnityEngine;

public class CutsceneCharacterRoster : MonoBehaviour
{
    [SerializeField] private CutsceneCharacterRole[] characters;
    
    public void Init()
    {
        try
        {
            if (characters != null)
                characters.ForEach(c => c.Init());
        }
        catch (Exception e)
        {
            Log.Error(e);
        }
    }
}
