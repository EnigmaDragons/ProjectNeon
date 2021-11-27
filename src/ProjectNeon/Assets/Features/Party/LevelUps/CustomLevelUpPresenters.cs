using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOnce/CustomLevelUpPresenters")]
public class CustomLevelUpPresenters : ScriptableObject
{
    [SerializeField] private CardPresenter cardPresenterPrototype;

    public CardPresenter CardPrototype => cardPresenterPrototype;
}