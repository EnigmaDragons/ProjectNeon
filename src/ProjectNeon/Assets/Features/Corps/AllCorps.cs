using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOnce/All Corps")]
public class AllCorps : ScriptableObject
{
    private Dictionary<string, Corp> _map;
    [SerializeField] private StaticCorp none;
    [UnityEngine.UI.Extensions.ReadOnly] public StaticCorp[] Corps; //Unity Collection Readonly
    [SerializeField] private StaticCorp polyCorp;
    [SerializeField] private StaticCorp[] gearSellingCorps;
    [SerializeField] private StaticCorp[] clinicCorps;
    
    public Dictionary<string, Corp> GetMap() => _map ??= Corps.ToDictionary(x => x.Name, x => (Corp)x);
    public Maybe<Corp> GetCorpByName(string corpName) => GetMap().ValueOrMaybe(corpName);
    public Corp GetCorpByNameOrNone(string corpName) => GetCorpByName(corpName).OrDefault(none);
    public Corp[] GetCorps() => GetMap().Values.ToArray();
    public Corp PolyCorp => polyCorp;
    public Corp[] GearSellingCorps => gearSellingCorps;
    public Corp[] ClinicCorps => clinicCorps;
}
