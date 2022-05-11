public class ShowCharacterRequested
{
    public string CharacterAlias { get; }

    public ShowCharacterRequested(string characterAlias)
        => CharacterAlias = characterAlias;
}