using UnityEngine;

[CreateAssetMenu(menuName = "Cutscene/Setting/GameObject")]
public class GameObjectSetting : CutsceneSetting
{
    [SerializeField] private string settingDisplayName;
    [SerializeField] private GameObject battlefield;

    public GameObject Battlefield => battlefield;
    
    public override string GetDisplayName() 
        => string.IsNullOrWhiteSpace(settingDisplayName) ? name.WithSpaceBetweenWords() : settingDisplayName;
    
    public override void SpawnTo(GameObject parent)
    {
        var obj = Instantiate(battlefield, parent.transform);
        obj.GetComponent<CutsceneCharacterRoster>()?.Init();
    }
}
