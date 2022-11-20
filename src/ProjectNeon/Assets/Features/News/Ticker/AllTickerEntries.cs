using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOnce/NewsTickerEntries")]
public class AllTickerEntries : ScriptableObject, ILocalizeTerms
{
    [SerializeField] private TickerEntry[] entries;

    public TickerEntry[] Entries => entries.ToArray();

    public string[] GetLocalizeTerms()
        => entries.Select(x => x.Term).ToArray();
}
