using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOnce/NewsTickerEntries")]
public class AllTickerEntries : ScriptableObject
{
    [SerializeField] private TickerEntry[] entries;

    public TickerEntry[] Entries => entries.ToArray();
}
