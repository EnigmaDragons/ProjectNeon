#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class DisplayAllCharacters : OnMessage<LanguageChanged>
{
    private static string BaseDir = ".\\LocalizationAssets";
    
    [SerializeField] private Button next;
    [SerializeField] private Button prev;
    [SerializeField] private GameObject charDisplaysParent;
    [SerializeField] private Button copyAllUnicodes;
    [SerializeField] private Button copySelectedUnicodes;

    private CharDisplay[] charDisplays;
    private char[] _characters;
    private HashSet<char> _selectedChars;
    private int _maxIndex;
    private int _index;

    private void Start()
    {
        charDisplays = charDisplaysParent.GetComponentsInChildren<CharDisplay>();
        next.onClick.AddListener(NextPage);
        prev.onClick.AddListener(PrevPage);
        copyAllUnicodes.onClick.AddListener(() =>
        {
            if (!Directory.Exists(BaseDir))
                Directory.CreateDirectory(BaseDir);
        
            File.WriteAllText($"{BaseDir}\\allCharacterUnicodes.txt", string.Join(",", _characters.Select(ToUnicode)));
        });
        copySelectedUnicodes.onClick.AddListener(() =>
        {
            if (!Directory.Exists(BaseDir))
                Directory.CreateDirectory(BaseDir);

            File.WriteAllText($"{BaseDir}\\selectedCharacterUnicodes.txt",string.Join(",", _selectedChars.Select(ToUnicode)));
        });
        UpdateTerms();
    }
    
    protected override void Execute(LanguageChanged msg) => UpdateTerms();

    private void UpdateTerms()
    {
        _characters = CategoryTranslator.GetSource().mTerms.SelectMany(x => x.Term.ToLocalized().ToArray()).Distinct().OrderBy(x => x).ToArray();
        _maxIndex = (int)Math.Floor((decimal)_characters.Length / charDisplays.Length);
        _index = 0;
        _selectedChars = new HashSet<char>();
        UpdateText();
    }

    private void UpdateText()
    {
        var characters = _characters.Skip(charDisplays.Length * _index).Take(charDisplays.Length).ToArray();
        for (var i = 0; i < charDisplays.Length; i++)
        {
            if (characters.Length > i)
                charDisplays[i].Init(characters[i], _selectedChars.Contains(characters[i]), (selected, character) =>
                {
                    if (selected)
                        _selectedChars.Add(character);
                    else
                        _selectedChars.Remove(character);
                });
            else
                charDisplays[i].Hide();
        }
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
    
    public string ToUnicode(char character) 
        => BitConverter.ToString(Encoding.BigEndianUnicode.GetBytes(character.ToString())).Replace("-", "");
}
#endif