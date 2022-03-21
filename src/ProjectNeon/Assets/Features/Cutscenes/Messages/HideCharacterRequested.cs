public class HideCharacterRequested
{
    public string CharacterAlias { get; }

    public HideCharacterRequested(string characterAlias)
        => CharacterAlias = characterAlias;
}