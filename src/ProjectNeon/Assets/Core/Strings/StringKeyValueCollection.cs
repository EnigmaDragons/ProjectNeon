using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(order = -19)]
public class StringKeyValueCollection : ScriptableObject
{
    [SerializeField] private List<StringKeyValuePair> items;

    public string this[string s] => items.First(x => x.Key.Value.Equals(s)).Value;
    public string this[StringVariable s] => items.First(x => x.Key.Value.Equals(s.Value)).Value;
    public string this[StringReference s] => items.First(x => x.Key.Value.Equals(s.Value)).Value;

    public string ValueOrDefault(string key, string defaultValue) =>
        items.FirstOrMaybe(x => x.Key.Value.Equals(key))
            .Select(x => x.Value, defaultValue);

    public IEnumerable<StringKeyValuePair> All => items;
}
