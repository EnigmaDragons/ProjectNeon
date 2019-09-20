using UnityEditor;
using UnityEngine;

static class StateAssetMenu
{
    [MenuItem("Assets/Create/Game State/Library")]
    static void Library() => Create<Library>();
    [MenuItem("Assets/Create/Game State/Battle State")]
    static void BattleState() => Create<BattleState>();
    [MenuItem("Assets/Create/Game State/Party")]
    static void Party() => Create<Party>();
    [MenuItem("Assets/Create/Game State/Party Decks")]
    static void PartyDecks() => Create<PartyDecks>();
    [MenuItem("Assets/Create/Game State/Character Pool")]
    static void CharacterPool() => Create<CharacterPool>();

    private static void Create<T>() where T : ScriptableObject
    {
        var asset = ScriptableObject.CreateInstance<T>();
        var path = AssetDatabase.GetAssetPath(Selection.activeObject);
        path += $"/New{typeof(T).Name}.asset";
        ProjectWindowUtil.CreateAsset(asset, path);
    }
}
