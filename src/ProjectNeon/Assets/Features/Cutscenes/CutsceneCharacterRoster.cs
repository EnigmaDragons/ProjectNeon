using System.Linq;
using UnityEngine;

public class CutsceneCharacterRoster : MonoBehaviour
{
    [SerializeField] private CutsceneCharacterRole[] characters;

    private void Awake()
    {
        characters.ForEach(c => c.Character.Init(c.Aliases.Select(a => a.Value).ToArray()));
    }
}
