using UnityEngine;

public class Hero : ScriptableObject
{
    [SerializeField] private Sprite bust;
    [SerializeField] private Stats stats;
    [SerializeField] private StringVariable className;

    public Sprite Bust => bust;
    public Stats Stats => stats;
    public StringVariable ClassName => className;
}
