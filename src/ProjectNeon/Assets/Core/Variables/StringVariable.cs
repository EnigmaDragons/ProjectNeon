using UnityEngine;

[CreateAssetMenu(menuName = "String Variable", fileName = "NewStringVariable", order = -20)]
public class StringVariable : ScriptableObject
{
    [SerializeField]
    private string value = "";

    public string Value
    {
        get { return value; }
        set { this.value = value; }
    }

    public void SetValue(string str) => value = str;

    public override bool Equals(object other)
    {
        if (other is string)
            return value.Equals(other);
        if (other is StringVariable v)
            return value.Equals(v.value);
        if (other is StringReference r)
            return value.Equals(r.Value);
        return false;
    }
}
