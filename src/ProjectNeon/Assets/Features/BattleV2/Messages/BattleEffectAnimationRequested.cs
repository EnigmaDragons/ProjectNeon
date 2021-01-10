
using UnityEngine;

public class BattleEffectAnimationRequested
{
    public string EffectName { get; set; }
    public Scope Scope { get; set; }
    public Group Group { get; set; }
    public Target Target { get; set; }
    public int PerformerId { get; set; }
    public float Size { get; set; } = 1;
    public float Speed { get; set; } = 1;
    public Color Color { get; set; } = new Color(0f, 0f, 0f, 0f);
}
