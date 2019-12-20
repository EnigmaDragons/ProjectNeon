using UnityEditor;
using UnityEngine;

static class GameDataMenu
{
    [MenuItem("Assets/Create/Card")]
    static void Card() => Create<Card>();

    [MenuItem("Assets/Create/GameContent/Hero")]
    static void Hero() => Create<Hero>();
    [MenuItem("Assets/Create/GameContent/Enemy")]
    static void Enemy() => Create<Enemy>();

    [MenuItem("Assets/Create/GameContent/SimpleResourceType")]
    static void SimpleResourceType() => Create<SimpleResourceType>();
    [MenuItem("Assets/Create/GameContent/Deck")]
    static void Deck() => Create<Deck>();
    [MenuItem("Assets/Create/GameContent/DumbAI")]
    static void DumbAI() => Create<DumbAI>();

    [MenuItem("Assets/Create/Variable/Spell Hits Last Card Multiplier")]
    static void SpellsHitLastTurn()
    {
        var asset = ScriptableObject.CreateInstance<SpellHitsLastCardMultiplier>();
        var path = AssetDatabase.GetAssetPath(Selection.activeObject);
        path += $"/New{nameof(SpellHitsLastCardMultiplier)}.asset";
        ProjectWindowUtil.CreateAsset(asset, path);
    }

    private static void Create<T>() where T : ScriptableObject
    {
        var asset = ScriptableObject.CreateInstance<T>();
        var path = AssetDatabase.GetAssetPath(Selection.activeObject);
        path += $"/New{typeof(T).Name}.asset";
        ProjectWindowUtil.CreateAsset(asset, path);
    }
}
