using System;
using System.Linq;

[Serializable]
public class CutsceneCharacterRole
{
    public CutsceneCharacter Character;
    public StringReference[] Aliases;

    public string[] GetAliases() 
        => Aliases == null 
            ? new string[0] 
            : Aliases.Where(a => a != null).Select(a => a.Value).ToArray();

    public void Init()
    {
        if (Character != null)
            Character.Init(GetAliases());
    }
}
