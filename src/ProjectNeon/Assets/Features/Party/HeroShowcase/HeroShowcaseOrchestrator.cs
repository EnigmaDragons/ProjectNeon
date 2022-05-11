using System.Collections;
using DG.Tweening;
using UnityEngine;

public class HeroShowcaseOrchestrator : OnMessage<BeginHeroShowcaseRequested>
{
    [SerializeField] private bool beginOnStart = true;
    [SerializeField] private GameObject heroLocation;
    [SerializeField] private Camera viewingCamera;
    [SerializeField] private Transform[] cameraTargetTransforms;
    [SerializeField] private float[] cameraTriggerDurations;
    [SerializeField] private CharacterAnimationType animationType;
    [SerializeField] private float animationRepeatInterval;

    private int _memberId = 1;
    private bool _isFinished = false; 

    private void Start()
    {
        if (beginOnStart)
            BeginShowcase();
    }
    
    public void BeginShowcase()
    {
        StopAllCoroutines();
        _isFinished = false;
        
        StartCoroutine(CameraFlow());
        StartCoroutine(AnimFlow());
    }

    private void TweenToCameraPosition(Transform target, float duration)
    {
        viewingCamera.transform.DOLocalMove(target.position, duration).SetEase(Ease.InQuad);
        viewingCamera.transform.DOLocalRotate(target.rotation.eulerAngles, duration).SetEase(Ease.InQuad);
    }
    
    private IEnumerator CameraFlow()
    {
        var camTransform = viewingCamera.transform;
        camTransform.localPosition = cameraTargetTransforms[0].localPosition;
        camTransform.localRotation = cameraTargetTransforms[0].localRotation;

        for (var i = 1; i < cameraTargetTransforms.Length && i < cameraTriggerDurations.Length; i++)
        {
            var duration = cameraTriggerDurations[i];
            TweenToCameraPosition(cameraTargetTransforms[i], duration);
            yield return new WaitForSeconds(duration);
        }

        Log.Info("Is Finished");
        _isFinished = true;
    }

    private IEnumerator AnimFlow()
    {
        Log.Info("Anim Flow Start");
        while (!_isFinished)
        {
            Log.Info("Anim Flow");
            Message.Publish(new CharacterAnimationRequested2(_memberId, animationType));
            yield return new WaitForSeconds(animationRepeatInterval);
        }
    }

    protected override void Execute(BeginHeroShowcaseRequested msg)
    {
        var hero = msg.Hero;
        heroLocation.DestroyAllChildren();
        var obj = Instantiate(hero.Body, heroLocation.transform);
        var ccAnimator = obj.GetComponentInChildren<CharacterCreatorAnimationController>();
        ccAnimator.Init(_memberId, hero.Animations, TeamType.Party);
        
        BeginShowcase();
    }
}
