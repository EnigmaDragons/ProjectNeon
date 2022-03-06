using System.Linq;
using UnityEngine;

public class CutsceneCharacterRoster : MonoBehaviour
{
    [SerializeField] private CutsceneCharacterRole[] characters;
    
    public void Init()
    {
        characters.ForEach(c => c.Character.Init(c.Aliases.Select(a => a.Value).ToArray()));
    }
}
