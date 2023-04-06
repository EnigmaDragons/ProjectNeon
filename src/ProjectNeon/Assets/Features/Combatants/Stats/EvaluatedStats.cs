using System;

[Serializable]
public class EvaluatedStats : IStats
{
    public float _maxHP,
        _attack,
        _magic,
        _armor,
        _resistance,
        _damagability,
        _healability,
        _leadership,
        _maxShield,
        _startingShield,
        _economy,
        _extraCardPlays,
        _power,
        _hp,
        _shield,
        _disabled,
        _stun,
        _taunt,
        _doubleDamage,
        _stealth,
        _confused,
        _blind,
        _injury,
        _lifesteal,
        _phase,
        _inhibit,
        _dodge,
        _aegis,
        _preventDeath,
        _prominent,
        _marked,
        _preventResourceGains,
        _vulnerable,
        _antiHeal,
        _primaryResource,
        _overkillDamageAmount;

    public float this[StatType statType]
    {
        get
        {
            if (statType == StatType.MaxHP)
                return _maxHP;
            if (statType == StatType.Attack)
                return _attack;
            if (statType == StatType.Magic)
                return _magic;
            if (statType == StatType.Armor)
                return _armor;
            if (statType == StatType.Resistance)
                return _resistance;
            if (statType == StatType.Damagability)
                return _damagability;
            if (statType == StatType.Healability)
                return _healability;
            if (statType == StatType.Leadership)
                return _leadership;
            if (statType == StatType.MaxShield)
                return _maxShield;
            if (statType == StatType.StartingShield)
                return _startingShield;
            if (statType == StatType.Economy)
                return _economy;
            if (statType == StatType.ExtraCardPlays)
                return _extraCardPlays;
            if (statType == StatType.Power)
                return _power;
            Log.NonCrashingError($"Asked for an unknown Stat Type: {statType}");
            return 0;
        }
    }

    public float this[TemporalStatType statType]
    {
        get
        {
            if (statType == TemporalStatType.HP)
                return _hp;
            if (statType == TemporalStatType.Shield)
                return _shield;
            if (statType == TemporalStatType.Disabled)
                return _disabled;
            if (statType == TemporalStatType.Stun)
                return _stun;
            if (statType == TemporalStatType.Taunt)
                return _taunt;
            if (statType == TemporalStatType.DoubleDamage)
                return _doubleDamage;
            if (statType == TemporalStatType.Stealth)
                return _stealth;
            if (statType == TemporalStatType.Confused)
                return _confused;
            if (statType == TemporalStatType.Blind)
                return _blind;
            if (statType == TemporalStatType.Injury)
                return _injury;
            if (statType == TemporalStatType.Lifesteal)
                return _lifesteal;
            if (statType == TemporalStatType.Phase)
                return _phase;
            if (statType == TemporalStatType.Inhibit)
                return _inhibit;
            if (statType == TemporalStatType.Dodge)
                return _dodge;
            if (statType == TemporalStatType.Aegis)
                return _aegis;
            if (statType == TemporalStatType.PreventDeath)
                return _preventDeath;
            if (statType == TemporalStatType.Prominent)
                return _prominent;
            if (statType == TemporalStatType.Marked)
                return _marked;
            if (statType == TemporalStatType.PreventResourceGains)
                return _preventResourceGains;
            if (statType == TemporalStatType.Vulnerable)
                return _vulnerable;
            if (statType == TemporalStatType.AntiHeal)
                return _antiHeal;
            if (statType == TemporalStatType.PrimaryResource)
                return _primaryResource;
            if (statType == TemporalStatType.OverkillDamageAmount)
                return _overkillDamageAmount;
            Log.NonCrashingError($"Asked for an unknown Temporal Stat Type: {statType}");
            return 0;
        }
    }
    
    public InMemoryResourceType[] ResourceTypes { get; }

    public EvaluatedStats() : this(new StatAddends(), StatType.Power) {}
    public EvaluatedStats(IStats stats, StatType primaryStat)
    {
        ResourceTypes = stats.ResourceTypes;
        _maxHP = stats[StatType.MaxHP];
        _attack = stats[StatType.Attack];
        _magic = stats[StatType.Magic];
        _armor = stats[StatType.Armor];
        _resistance = stats[StatType.Resistance];
        _damagability = stats[StatType.Damagability];
        _healability = stats[StatType.Healability];
        _leadership = stats[StatType.Leadership];
        _maxShield = stats[StatType.MaxShield];
        _startingShield = stats[StatType.StartingShield];
        _economy = stats[StatType.Economy];
        _extraCardPlays = stats[StatType.ExtraCardPlays];
        _power = stats[primaryStat];
        _hp = stats[TemporalStatType.HP];
        _shield = stats[TemporalStatType.Shield];
        _disabled = stats[TemporalStatType.Disabled];
        _stun = stats[TemporalStatType.Stun];
        _taunt = stats[TemporalStatType.Taunt];
        _doubleDamage = stats[TemporalStatType.DoubleDamage];
        _stealth = stats[TemporalStatType.Stealth];
        _confused = stats[TemporalStatType.Confused];
        _blind = stats[TemporalStatType.Blind];
        _injury = stats[TemporalStatType.Injury];
        _lifesteal = stats[TemporalStatType.Lifesteal];
        _phase = stats[TemporalStatType.Phase];
        _inhibit = stats[TemporalStatType.Inhibit];
        _dodge = stats[TemporalStatType.Dodge];
        _aegis = stats[TemporalStatType.Aegis];
        _preventDeath = stats[TemporalStatType.PreventDeath];
        _prominent = stats[TemporalStatType.Prominent];
        _marked = stats[TemporalStatType.Marked];
        _preventResourceGains = stats[TemporalStatType.PreventResourceGains];
        _vulnerable = stats[TemporalStatType.Vulnerable];
        _antiHeal = stats[TemporalStatType.AntiHeal];
        _primaryResource = stats[TemporalStatType.PrimaryResource];
        _overkillDamageAmount = stats[TemporalStatType.OverkillDamageAmount];
    } 
}