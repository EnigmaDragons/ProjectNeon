using UnityEngine;

[CreateAssetMenu(menuName = "Cutscene/Setting/GameObject")]
public class GameObjectSetting : CutsceneSetting
{
    [SerializeField] private GameObject battlefield;

    public override void SpawnTo(GameObject parent)
    {
        Instantiate(battlefield, parent.transform);
    }
}
