using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(order = -20)]
public class StringKeyTermCollection : ScriptableObject, ILocalizeTerms
{
    [SerializeField] private List<StringKeyTermPair> items;

    public string this[string s] => items.First(x => x.Key.Value.Equals(s)).Term.Value;
    public string this[StringVariable s] => items.First(x => x.Key.Value.Equals(s.Value)).Term.Value;
    public string this[StringReference s] => items.First(x => x.Key.Value.Equals(s.Value)).Term.Value;

    public string ValueOrDefault(string key, string defaultTerm) =>
        items.FirstOrMaybe(x => x.Key.Value.Equals(key))
            .Select(x => x.Term.Value, defaultTerm);

    public bool Contains(string key) => items.FirstOrMaybe(x => x.Key.Value.Equals(key)).IsPresent;

    public IEnumerable<StringKeyTermPair> All => items;
    
    public string[] GetLocalizeTerms()
        => items.Select(x => x.Term.Value).ToArray();
}