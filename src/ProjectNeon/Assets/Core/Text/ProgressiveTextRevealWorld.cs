using System;
using System.Collections;
using TMPro;
using UnityEngine;

public sealed class ProgressiveTextRevealWorld : MonoBehaviour
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
    
    private int _cursor;
    private bool _shouldAutoProceed = false;
    private bool _manualInterventionDisablesAuto = true;
    private bool _finished = false;
    private Action _onFinished = () => { };

    private static bool _debugLog = false;
    
    public void Hide()
    {
        if (!chatBox.gameObject.activeSelf || isRevealing)
            return;
        
        Info($"Text Box - Hide");
        chatBox.gameObject.SetActive(false);
    }

    public void Display(string text) 
        => Display(text,  false, true, () => { });
    public void Display(string text, Action onFinished) 
        => Display(text, false, true, onFinished);
    public void Display(string text, bool shouldAutoProceed, Action onFinished) 
        => Display(text, shouldAutoProceed, true, onFinished);
    public void Display(string text, bool shouldAutoProceed, bool manualInterventionDisablesAuto,  Action onFinished) 
    {
        if (isRevealing)
            return;

        chatBox.gameObject.SetActive(true);
        fullText = text;
        _onFinished = onFinished;
        _shouldAutoProceed = shouldAutoProceed;
        _manualInterventionDisablesAuto = manualInterventionDisablesAuto;
        _finished = false;
        StartCoroutine(BeginReveal());
    }

    public void Proceed() => Proceed(false);
    public void Proceed(bool isAuto)
    {
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
    }

    public void ReversePanelFacing()
    {
        if (panelBg != null)
            panelBg.transform.Rotate(0, 180, 0);
        textBox.transform.localPosition = textBox.transform.localPosition + new Vector3(reversedTextBoxOffset.x, reversedTextBoxOffset.y);
    }

    private IEnumerator BeginReveal()
    {
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
            textBox.text = shownText;
            _cursor++;
            if (sfx != null)
                sfx.Play(transform.position);
            //This advances past markdown
            while (_cursor < fullText.Length && fullText[_cursor - 1] == '<')
                _cursor = fullText.IndexOf('>', _cursor) + 2;
            
            yield return new WaitUntil(() => Time.timeScale > 0.1f);
            yield return new WaitForSeconds(secondsPerCharacter);
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
