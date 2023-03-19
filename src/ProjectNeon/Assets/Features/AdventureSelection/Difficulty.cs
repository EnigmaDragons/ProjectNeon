using UnityEngine;

[CreateAssetMenu(menuName = "Adventure/Difficulty")]
public class Difficulty : ScriptableObject, ILocalizeTerms
{
    [SerializeField] public int id;
    [SerializeField, PreviewSprite] private Sprite img; 
    [SerializeField, UnityEngine.UI.Extensions.ReadOnly] public string difficultyName;
    [SerializeField, UnityEngine.UI.Extensions.ReadOnly] public string description;
    [SerializeField, TextArea(minLines: 3, maxLines: 9), UnityEngine.UI.Extensions.ReadOnly] public string changes;
    [SerializeField] private bool resetAfterDeath;
    [SerializeField] private StaticGlobalEffect[] globalEffects;

    public int Id => id;
    public string NameTerm => $"Difficulties/Difficulty{id}Name";
    public string DescriptionTerm => $"Difficulties/Difficulty{id}Description";
    public string ChangesTerm => $"Difficulties/Difficulty{id}Changes";
    public bool ResetAfterDeath => resetAfterDeath;
    public StaticGlobalEffect[] GlobalEffects => globalEffects;
    public Sprite Image => img;

    public string[] GetLocalizeTerms()
        => new[] { NameTerm, DescriptionTerm, ChangesTerm };
}