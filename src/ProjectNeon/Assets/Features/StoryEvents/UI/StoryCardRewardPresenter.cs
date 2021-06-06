using UnityEngine;

public class StoryCardRewardPresenter : MonoBehaviour
{
    [SerializeField] private CardPresenter presenter;

    public void Init(CardTypeData c) => presenter.Set(c, () => { });
}
