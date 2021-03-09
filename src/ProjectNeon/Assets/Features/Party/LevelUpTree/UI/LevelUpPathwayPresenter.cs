using UnityEngine;

public sealed class LevelUpPathwayPresenter : MonoBehaviour
{
    [SerializeField] private GameObject rowParent;
    [SerializeField] private LevelUpOptionsPresenter rowPrototype;

    public void Init(HeroLevelUpPathway pathway)
    {
        rowParent.DestroyAllChildren();
        for (var i = 15; i > 1; i--)
        {
            Instantiate(rowPrototype, rowParent.transform).Init(i, pathway.ForLevel(i));
        }
    }
}
