using UnityEditor;
using UnityEngine;

static class StateAssetMenu
{
    [MenuItem("Assets/Create/Game State/Library")]
    static void Library() => Create<Library>();
    [MenuItem("Assets/Create/Game State/Battle State")]
    static void BattleState() => Create<BattleState>();
    [MenuItem("Assets/Create/Game State/Battle Player Targeting State")]
    static void BattlePlayerTargetingState() => Create<BattlePlayerTargetingState>();
    [MenuItem("Assets/Create/Game State/Party")]
    static void Party() => Create<Party>();
    [MenuItem("Assets/Create/Game State/Hero Pool")]
    static void HeroPool() => Create<HeroPool>();
    [MenuItem("Assets/Create/Game State/Adventure Progress")]
    static void AdventureProgress() => Create<AdventureProgress>();
    [MenuItem("Assets/Create/Game State/Deck State")]
    static void DeckBuilderState() => Create<DeckBuilderState>();

    private static void Create<T>() where T : ScriptableObject
    {
        var asset = ScriptableObject.CreateInstance<T>();
        var path = AssetDatabase.GetAssetPath(Selection.activeObject);
        path += $"/New{typeof(T).Name}.asset";
        ProjectWindowUtil.CreateAsset(asset, path);
    }
}
