
using UnityEditor;
using UnityEngine;

static class GameDataMenu
{
    [MenuItem("Assets/Create/GameContent/Card")]
    static void Card() => Create<Card>();

    //Effects menu
    [MenuItem("Assets/Create/GameContent/Effects/Damage")]
    static void Damage() => Create<Damage>();

    [MenuItem("Assets/Create/GameContent/Effects/Heal")]
    static void Heal() => Create<Heal>();

    [MenuItem("Assets/Create/GameContent/Effects/NoEffect")]
    static void NoEffect() => Create<NoEffect>();


    [MenuItem("Assets/Create/GameContent/CardAction")]
    static void CardAction() => Create<CardAction>();
    
    [MenuItem("Assets/Create/GameContent/Character")]
    static void Character() => Create<Character>();
    [MenuItem("Assets/Create/GameContent/Enemy")]
    static void Enemy() => Create<Enemy>();

    [MenuItem("Assets/Create/GameContent/BaseStats")]
    static void BaseStats() => Create<BaseStats>();
    [MenuItem("Assets/Create/GameContent/SimpleResourceType")]
    static void SimpleResourceType() => Create<SimpleResourceType>();
    [MenuItem("Assets/Create/GameContent/Deck")]
    static void Deck() => Create<Deck>();
    [MenuItem("Assets/Create/GameContent/DumbAI")]
    static void DumbAI() => Create<DumbAI>();

    private static void Create<T>() where T : ScriptableObject
    {
        var asset = ScriptableObject.CreateInstance<T>();
        var path = AssetDatabase.GetAssetPath(Selection.activeObject);
        path += $"/New{typeof(T).Name}.asset";
        ProjectWindowUtil.CreateAsset(asset, path);
    }
}
