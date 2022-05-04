using UnityEngine;

public class TauntEffect : OnMessage<BattleStateChanged>
{
    [SerializeField] private GameObject vfx;

    private Member _member;

    private void Awake() => Hide();

    public void Init(Member m)
    {
        _member = m;
        Render();
    }

    private void Hide() => vfx.SetActive(false);
    
    protected override void Execute(BattleStateChanged msg)
    {
        if (_member == null)
            return;
        
        Render();
    }

    public void Render()
    {
        if (_member == null)
            return;
        
        vfx.SetActive(_member.HasTaunt());
    }
}