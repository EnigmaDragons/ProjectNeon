
using UnityEditor;
using UnityEngine;

static class GameDataMenu
{
    [MenuItem("Assets/Create/GameContent/Card")]
    static void Card() => Create<Card>();
    [MenuItem("Assets/Create/GameContent/Character")]
    static void Character() => Create<Character>();
    [MenuItem("Assets/Create/GameContent/Enemy")]
    static void Enemy() => Create<Enemy>();

    [MenuItem("Assets/Create/GameContent/Stats")]
    static void Stats() => Create<Stats>();
    [MenuItem("Assets/Create/GameContent/Deck")]
    static void Deck() => Create<Deck>();
    [MenuItem("Assets/Create/GameContent/CardEffect")]
    static void CardEffect() => Create<CardEffect>();
    [MenuItem("Assets/Create/GameContent/EffectAction")]
    static void EffectAction() => Create<EffectAction>();
    [MenuItem("Assets/Create/GameContent/EnemyAI")]
    static void TurnAI() => Create<TurnAI>();

    private static void Create<T>() where T : ScriptableObject
    {
        var asset = ScriptableObject.CreateInstance<T>();
        var path = AssetDatabase.GetAssetPath(Selection.activeObject);
        path += $"/New{typeof(T).Name}.asset";
        ProjectWindowUtil.CreateAsset(asset, path);
    }
}
