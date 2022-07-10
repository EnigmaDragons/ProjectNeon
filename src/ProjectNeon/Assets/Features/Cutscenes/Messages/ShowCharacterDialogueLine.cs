
public class ShowCharacterDialogueLine
{
    public string CharacterAlias { get; }
    public string Text { get; }
    public bool ForceAutoAdvanceBecauseThisIsASingleMessage { get; }
    
    public ShowCharacterDialogueLine(string characterAlias, string text, bool forceAutoAdvanceBecauseThisIsASingleMessage = false)
    {
        CharacterAlias = characterAlias;
        Text = text;
        ForceAutoAdvanceBecauseThisIsASingleMessage = forceAutoAdvanceBecauseThisIsASingleMessage;
    }
}
