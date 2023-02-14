using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOnce/AllBosses")]
public class AllBosses : ScriptableObject
{
    [SerializeField] public Boss[] bosses;

    public Boss RandomBoss() => bosses.Random();
}