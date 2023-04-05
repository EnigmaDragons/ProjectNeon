using UnityEngine;

public class StoryCardRewardPresenter : MonoBehaviour
{
    [SerializeField] private CardPresenter presenter;

    public void Init(CardType c) => presenter.Set(c, () => { });
}
