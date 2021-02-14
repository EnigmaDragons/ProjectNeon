using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EnemyDetailsViewController : OnMessage<ShowEnemyDetails, HideEnemyDetails>
{
    [SerializeField] private GameObject viewParent;
    [SerializeField] private Image darken;
    [SerializeField] private EnemyDetailsView enemy;

    private float _duration = 0.4f;
    private Vector3 _itemScale;
    private float _darkenAlpha;
    
    private void Awake()
    {
        _darkenAlpha = darken.color.a;
        _itemScale = enemy.transform.localScale;
        viewParent.SetActive(false);
    }
    
    protected override void Execute(ShowEnemyDetails msg)
    {        
        enemy.Show(msg.Enemy);
        viewParent.SetActive(true);
        darken.color = new Color(darken.color.r, darken.color.g, darken.color.b, 0);
        darken.DOFade(_darkenAlpha, _duration);
        enemy.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        enemy.transform.DOScale(_itemScale, _duration);
    }

    protected override void Execute(HideEnemyDetails msg)
    {
        Message.Publish(new HideChainedCard());
        viewParent.SetActive(false);
    }
}
