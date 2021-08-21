
using UnityEngine;

[CreateAssetMenu]
public class LongText : ScriptableObject
{
    [TextArea(4, 10), SerializeField] private string value;

    public string Value => value;
    
    public static implicit operator string(LongText txt) => txt.ToString();
    public override string ToString() => value;
    public void Init(string val) => value = val;
}