using UnityEngine;

public class GeneralDialoguePresenter : OnMessage<ShowCharacterDialogueLine, HideCharacterSpeechBubbleRequested>
{
    [SerializeField] private CutsceneCharacter[] characters;
    [SerializeField] private FloatReference dialogueWaitDelay;
    [SerializeField] private FloatReference messageLingerDuration = new FloatReference(4f);
    
    protected bool _debugLoggingEnabled = true;

    protected override void AfterEnable()
    {
        HideAllSpeeches();
    }
    
    protected override void Execute(ShowCharacterDialogueLine msg)
    {        
        characters.FirstOrMaybe(c => c.Matches(msg.CharacterAlias))
            .ExecuteIfPresentOrElse(c =>
                {
                    var useAutoAdvance = msg.ForceAutoAdvanceBecauseThisIsASingleMessage || CurrentGameOptions.Data.UseAutoAdvance;
                    c.SetTalkingState(true);
                    var speech = c.SpeechBubble;
                    speech.SetAllowManualAdvance(!useAutoAdvance);
                    speech.SetOnFullyShown(() => c.SetTalkingState(false));
                    if (useAutoAdvance)
                        speech.Display(msg.Text, shouldAutoProceed: true, manualInterventionDisablesAuto: false,
                            () => Async.ExecuteAfterDelay(dialogueWaitDelay, FinishCurrentSegment));
                    else
                        speech.Display(msg.Text, shouldAutoProceed: false, FinishCurrentSegment);
                },
                () =>
                {
                    Log.Warn($"Character Not Found by General Dialogue Presenter {msg.CharacterAlias}");
                    FinishCurrentSegment();
                });
    }

    protected override void Execute(HideCharacterSpeechBubbleRequested msg) => HideSpeech(msg.CharacterAlias);

    private void FinishCurrentSegment()
    {
        characters.ForEach(c => c.SetTalkingState(false));
        this.ExecuteAfterDelay(messageLingerDuration, HideAllSpeeches);
    }

    private void HideAllSpeeches()
    {
        DebugLog("Hide All Speeches");
        characters.ForEach(c => c.ForceEndConversation());
    }
    
    private void HideSpeech(string characterAlias)
    {
        characters.FirstOrMaybe(c => c.Matches(characterAlias))
            .ExecuteIfPresentOrElse(c => c.ForceEndConversation(), () => {});
    }

    private void DebugLog(string msg)
    {
        if (_debugLoggingEnabled)
            Log.Info($"General Cutscene Presenter - {msg}");
    }
}