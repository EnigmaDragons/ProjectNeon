using System.Linq;
using UnityEngine;

public class CutsceneCharacterAdditionalVisual : MonoBehaviour
{
    [SerializeField] private StringReference[] aliases;

    public string[] OwnerAliases => aliases.Select(a => a.Value).ToArray();
}
