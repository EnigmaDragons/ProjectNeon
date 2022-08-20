using UnityEngine;

[CreateAssetMenu(menuName = "Adventure/Difficulty")]
public class Difficulty : ScriptableObject
{
    [SerializeField] public int id;
    [SerializeField] private string difficultyName;
    [SerializeField] private string description;
    [SerializeField, TextArea(minLines: 3, maxLines: 9)] private string changes;
    [SerializeField] private bool resetAfterDeath;
    [SerializeField] private StaticGlobalEffect[] globalEffects;

    public int Id => id;
    public string Name => difficultyName;
    public string Description => description;
    public string Changes => changes;
    public bool ResetAfterDeath => resetAfterDeath;
    public StaticGlobalEffect[] GlobalEffects => globalEffects;
}