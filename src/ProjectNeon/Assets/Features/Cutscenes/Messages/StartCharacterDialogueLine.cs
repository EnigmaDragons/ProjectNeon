
public class StartCharacterDialogueLine
{
    public string CharacterAlias { get; }
    public string Text { get; }
    
    public StartCharacterDialogueLine(string characterAlias, string text)
    {
        CharacterAlias = characterAlias;
        Text = text;
    }
}
