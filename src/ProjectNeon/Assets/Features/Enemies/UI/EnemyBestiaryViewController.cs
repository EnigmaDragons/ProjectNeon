using UnityEngine;

public class EnemyBestiaryViewController : OnMessage<ToggleBestiary>
{
    [SerializeField] private GameObject target;

    private void Awake() => target.SetActive(false);

    protected override void Execute(ToggleBestiary msg) => target.SetActive(!target.activeSelf);
}
