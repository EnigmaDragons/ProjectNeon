#if UNITY_EDITOR
using System;
using System.Linq;
using I2.Loc;
using UnityEngine;
using UnityEngine.UI;

public class DisplayAllCharacters : OnMessage<LanguageChanged>
{
    [SerializeField] private int charactersPerPage;
    [SerializeField] private Button next;
    [SerializeField] private Button prev;
    [SerializeField] private Localize text;

    private char[] _characters;
    private int _maxIndex;
    private int _index;

    private void Start()
    {
        next.onClick.AddListener(NextPage);
        prev.onClick.AddListener(PrevPage);
        UpdateTerms();
    }
    
    protected override void Execute(LanguageChanged msg) => UpdateTerms();

    private void UpdateTerms()
    {
        _characters = CategoryTranslator.GetSource().mTerms.SelectMany(x => x.Term.ToLocalized().ToArray()).Distinct().ToArray();
        _maxIndex = (int)Math.Floor((decimal)_characters.Length / charactersPerPage);
        _index = 0;
        UpdateText();
    }

    private void UpdateText()
    {
        text.SetFinalText(new string(_characters.Skip(charactersPerPage * _index).Take(charactersPerPage).ToArray()));
        next.gameObject.SetActive(_index != _maxIndex);
        prev.gameObject.SetActive(_index != 0);
    }

    private void NextPage()
    {
        _index++;
        UpdateText();
    }
    
    private void PrevPage()
    {
        _index--;
        UpdateText();
    }
}
#endif