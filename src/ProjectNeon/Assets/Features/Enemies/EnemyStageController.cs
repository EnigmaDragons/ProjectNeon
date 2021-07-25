using UnityEngine;

public class EnemyStageController : OnMessage<ShowEnemyOnStage>
{
    [SerializeField] private GameObject stage;
    
    private Vector3 _initialStagePosition;

    private void Awake()
    {
        _initialStagePosition = stage.transform.localPosition;
    }
    
    protected override void Execute(ShowEnemyOnStage msg) => Show(msg.Enemy);

    public void Show(EnemyInstance e)
    {
        stage.DestroyAllChildren();
        RenderEnemyBody(e);
    }
    
    private void RenderEnemyBody(EnemyInstance e)
    {
        stage.transform.localPosition = _initialStagePosition - e.LibraryCameraOffset;
        var enemyBody = Instantiate(e.Prefab, stage.transform);
        
        var enemyUi = enemyBody.GetComponentInChildren<EnemyBattleUIPresenter>();
        if (enemyUi != null)
            enemyUi.gameObject.SetActive(false);
        var enemyAngleShift = enemyBody.GetComponentInChildren<Universal2DAngleShift>();
        if (enemyAngleShift != null)
            enemyAngleShift.Revert();
        var shield = enemyBody.GetComponentInChildren<ShieldVisual>();
        if (shield != null)
            shield.Init(e.AsMember(InfoMemberId.Get()));
    }
}