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
            var targetPosition = battleState.GetCenterPoint(animationContext.Target);
            var direction = (targetPosition - position).normalized;
            var rotation = Quaternion.LookRotation(direction);
            Instantiate(bullet, position, rotation);
            yield return new WaitForSeconds(animationContext.AnimationData.TimeAmount);
        }
        gameObject.SetActive(false);
    }
}