using System;
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
        try
        {
            stage.transform.localPosition = _initialStagePosition - e.LibraryCameraOffset;
            var enemyBody = Instantiate(e.Prefab, stage.transform);

            var enemyUi = enemyBody.GetComponentInChildren<EnemyBattleUIPresenter>();
            if (enemyUi != null)
                enemyUi.gameObject.SetActive(false);
            var enemyAngleShift = enemyBody.GetComponentInChildren<Universal2DAngleShift>();
            if (enemyAngleShift != null)
                enemyAngleShift.UseIdentityRotation();
            var shield = enemyBody.GetComponentInChildren<ShieldVisual>();
            if (shield != null)
                shield.Init(e.AsMember(InfoMemberId.Get()));
            foreach (Transform c in enemyBody.transform)
            {
                if (c.name.ContainsAnyCase("Shadow"))
                    c.gameObject.SetActive(false);
            }
        }
        catch (Exception ex)
        {
            Log.Error($"Cannot Setup {e.Name}");
            Log.Error(ex);
        }
    }
}