using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class ProgressiveTextRevealWorld : ProgressiveText
{
    [SerializeField] private GameObject chatBox;
    [SerializeField] private GameObject panelBg;
    [SerializeField] private TextMeshPro textBox;
    [SerializeField] private FloatReference secondsPerCharacter = new FloatReference(0.07f);
    [SerializeField] private FloatReference autoAdvanceDelay = new FloatReference(0.8f);
    [SerializeField] private PlayableUiSound sfx;
    [SerializeField] private Vector2 reversedTextBoxOffset = Vector2.zero;
    
    [Header("Debug Info")]
    [SerializeField, ReadOnly] private bool isRevealing;
    [SerializeField, ReadOnly] private string fullText;

    private int sfxEveryXCharacters = 1;
    private bool _reversed;
    private int _cursor;
    private bool _allowManualAdvance = true;
    private bool _shouldAutoProceed = false;
    private bool _manualInterventionDisablesAuto = true;
    private bool _finished = false;
    private Action _onFullyShown = () => { };
    private Action _onFinished = () => { };

    private static bool _debugLog = false;

    public override void Hide()
    {
        if (!chatBox.gameObject.activeSelf || isRevealing)
            return;

        PerformHideUpdates();
    }
    
    public override void ForceHide()
    {
        if (!chatBox.gameObject.activeSelf)
            return;

        PerformHideUpdates();
    }
    
    public override void Display(string text, bool shouldAutoProceed, bool manualInterventionDisablesAuto, Action onFinished) 
    {
        if (isRevealing)
            return;

        chatBox.gameObject.SetActive(true);
        fullText = text;
        _onFinished = onFinished;
        _shouldAutoProceed = shouldAutoProceed;
        _manualInterventionDisablesAuto = manualInterventionDisablesAuto;
        _finished = false;
        gameObject.SetActive(true);
        enabled = true;
        StartCoroutine(BeginReveal());
    }

    public override void Proceed(bool isAuto)
    {        
        if (!isAuto && !_allowManualAdvance)
            return;
        
        Info($"Text Box - Proceed Auto: {isAuto}");
        if (_finished)
            return;
        if (!isAuto && _manualInterventionDisablesAuto)
            _shouldAutoProceed = false;
        if (isRevealing)
            ShowCompletely();
        else
        {
            Finish();
            return;
        }

        if (_shouldAutoProceed && isAuto)
            this.ExecuteAfterDelay(Finish, autoAdvanceDelay);
    }

    public override void SetAllowManualAdvance(bool allow)
    {
        _allowManualAdvance = allow;
    }

    public override void SetDisplayReversed(bool reversed)
    {
        if (_reversed != reversed)
            ReversePanelFacing();
    }

    public override void SetOnFullyShown(Action action) => _onFullyShown = action;
    
    private void PerformHideUpdates()
    {
        Info($"Text Box - Hide");
        chatBox.gameObject.SetActive(false);
        isRevealing = false;
    }
    
    private void Finish()
    {
        if (_finished)
            return;
        
        Info($"Text Box - Finished");
        _finished = true;
        _onFinished();
    }

    private void ShowCompletely()
    {
        Info($"Text Box - Displayed Completely");
        isRevealing = false;
        textBox.text = fullText;
        _onFullyShown();
    }

    public void ReversePanelFacing()
    {
        _reversed = !_reversed;
        if (panelBg != null)
            panelBg.transform.Rotate(0, 180, 0);
        textBox.transform.localPosition = textBox.transform.localPosition + new Vector3(reversedTextBoxOffset.x, reversedTextBoxOffset.y);
    }
    
    private IEnumerator BeginReveal()
    {
        if (!gameObject.activeSelf)
            yield break;
        
        var waitUntilGameUnpaused = new WaitUntil(() => Time.timeScale > 0.1f);
        var waitForNextChar = new WaitForSecondsRealtime(secondsPerCharacter);
        if (secondsPerCharacter.Value < 0.01f)
        {
            ShowCompletely();
            this.ExecuteAfterDelay(Proceed, autoAdvanceDelay);
            yield break;
        }
        
        isRevealing = true;
        chatBox.gameObject.SetActive(true);
        _cursor = 1;
        //This advances past markdown
        while (_cursor < fullText.Length && fullText[_cursor - 1] == '<')
            _cursor = fullText.IndexOf('>', _cursor) + 2;
        while (isRevealing && _cursor < fullText.Length)
        {
            var shownText = fullText.Substring(0, _cursor);
            var containsTextColoring = fullText.ContainsAnyCase("<color=");
            textBox.text = containsTextColoring ? shownText : $"{shownText}<color=\"white\">{fullText.Substring(_cursor)}</color>";
            _cursor++;
            if (sfx != null && shownText.Length % sfxEveryXCharacters == 0)
                sfx.Play(transform.position);
            
            //This advances past markdown
            while (_cursor < fullText.Length && fullText[_cursor - 1] == '<')
                _cursor = fullText.IndexOf('>', _cursor) + 2;

            if (Time.timeScale <= 0.1f)
                yield return waitUntilGameUnpaused;
            yield return waitForNextChar;
        }

        ShowCompletely();
        if (_shouldAutoProceed)
            Proceed(isAuto: true);
    }
    
    private Color FullAlphaColor(Color c) => new Color(c.r, c.g, c.b, 255f);

    private void Info(string message)
    {
        if (_debugLog)
            Log.Info(message);
    }
}
