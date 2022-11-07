using UnityEngine;

[CreateAssetMenu(menuName = "Adventure/Difficulty")]
public class Difficulty : ScriptableObject
{
    [SerializeField] public int id;
    [SerializeField, PreviewSprite] private Sprite img; 
    [SerializeField] public string difficultyName;
    [SerializeField] public string description;
    [SerializeField, TextArea(minLines: 3, maxLines: 9)] public string changes;
    [SerializeField] private bool resetAfterDeath;
    [SerializeField] private StaticGlobalEffect[] globalEffects;

    public int Id => id;
    public string Name => difficultyName;
    public string NameTerm => $"Difficulties/Difficulty{id}Name";
    public string DescriptionTerm => $"Difficulties/Difficulty{id}Description";
    public string ChangesTerm => $"Difficulties/Difficulty{id}Changes";
    public bool ResetAfterDeath => resetAfterDeath;
    public StaticGlobalEffect[] GlobalEffects => globalEffects;
    public Sprite Image => img;
}