using UnityEngine;

public class SetupBattlefield : MonoBehaviour
{
    [SerializeField] private BattleState battleState;
    [SerializeField] private GameEvent onFinished;
    [SerializeField] private float scale = 0.929f;

    private void Awake()
    {
        var battlefield = Instantiate(battleState.Battlefield, new Vector3(0, 0, 10), Quaternion.identity, transform);
        battlefield.transform.localScale = new Vector3(scale, scale, scale);
    }

    void Start()
    {
        onFinished.Publish();
    }
}
