using System.Collections;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    [SerializeField] private GameObject bullet;
    [SerializeField] private CurrentAnimationContext animationContext;
    [SerializeField] private BattleState battleState;
    
    private void OnEnable() => StartCoroutine(Shoot());

    private IEnumerator Shoot()
    {
        for (var i = 0; i < animationContext.AnimationData.IntAmount; i++)
        {
            var position = transform.position;
            Instantiate(bullet, position, Quaternion.LookRotation((battleState.GetCenterPoint(animationContext.Target) - position).normalized));
            yield return new WaitForSeconds(animationContext.AnimationData.TimeAmount);
        }
        gameObject.SetActive(false);
    }
}