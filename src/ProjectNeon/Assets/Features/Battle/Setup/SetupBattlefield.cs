using UnityEngine;

public class SetupBattlefield : MonoBehaviour
{
    [SerializeField] private BattleState battleState;
    [SerializeField] private GameEvent onFinished;

    private void Awake()
    {
        Instantiate(battleState.Battlefield, new Vector3(0, 0, 10), Quaternion.identity);
    }

    void Start()
    {
        onFinished.Publish();
    }
}
