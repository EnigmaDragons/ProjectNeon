using UnityEngine;

public abstract class StatusVisualEffect : OnMessage<BattleStateChanged>, IMemberUi
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

    private void Render()
    {
        if (_member == null)
            return;
        
        vfx.SetActive(IsActive(_member));
    }

    protected abstract bool IsActive(Member m);
}