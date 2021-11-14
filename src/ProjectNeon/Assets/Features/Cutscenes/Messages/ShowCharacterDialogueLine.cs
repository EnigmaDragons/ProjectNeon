
public class ShowCharacterDialogueLine
{
    public string CharacterAlias { get; }
    public string Text { get; }
    
    public ShowCharacterDialogueLine(string characterAlias, string text)
    {
        CharacterAlias = characterAlias;
        Text = text;
    }
}
