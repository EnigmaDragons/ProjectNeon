using UnityEngine;

public class FusionController : MemberInitable
{
    [SerializeField] private GameObject[] modules;
    [SerializeField] private GameObject[] toScale;
    [SerializeField] private float stage1Scale;
    [SerializeField] private float stage2Scale;
    [SerializeField] private float stage3Scale;
    [SerializeField] private int stage1ModuleCount = 13;
    [SerializeField] private int stage2ModuleCount = 7;
    [SerializeField] private int stage3ModuleCount = 1;
    [SerializeField] private BattleVFX pieceDeath;
    
    private Member _member;
    private int _moduleCount;

    public override void Init(Member member)
    {
        _member = member;
        toScale.ForEach(x => x.transform.localScale = new Vector3(stage1Scale, stage1Scale, 1));
        _moduleCount = stage1ModuleCount;
    }

    private void OnEnable() => Message.Subscribe<MemberStateChanged>(Execute, this);
    private void OnDisable() => Message.Unsubscribe(this);

    private void Execute(MemberStateChanged msg)
    {
        if (msg.State.MemberId != _member?.Id || _moduleCount == _member.State[StatType.ExtraCardPlays])
            return;
        _moduleCount--;
        Instantiate(pieceDeath, modules[_moduleCount].transform.position, Quaternion.identity, gameObject.transform).Initialized();
        modules[_moduleCount].SetActive(false);
        if (_moduleCount == stage2ModuleCount)
            toScale.ForEach(x => x.transform.localScale = new Vector3(stage2Scale, stage2Scale, 1));
        else if (_moduleCount == stage3ModuleCount)
            toScale.ForEach(x => x.transform.localScale = new Vector3(stage3Scale, stage3Scale, 1));
    }
}