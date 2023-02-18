
using System.Collections.Generic;

public class Achievement
{
    public const string AdventureWonBreakIntoMetroplexZero = "adv-won-break";
    public const string AdventureLostBreakIntoMetroplexZero = "adv-lost-break";
    public const string AdventureWonOrganizedHarvestors = "adv-won-org";
    public const string AdventureLostOrganizedHarvestors = "adv-lost-org";
    public const string AdventureWonAntiRobotSentiments = "adv-won-robot";
    public const string AdventureWonTrioDraft = "adv-won-draft-trio";
    public const string AdventureWonDuoDraft = "adv-won-draft-duo";
    public const string AdventureWonSoloDraft = "adv-won-draft-solo";
    
    public const string CombatLastOneStanding = "combat-last-standing";
    public const string Combat42Damage = "combat-42-dmg";
    public const string CombatFirstTurnVictory = "combat-first-turn-victory";
    
    public const string DifficultyCasual = "difficulty-casual";
    public const string DifficultyVeteran = "difficulty-veteran";
    public const string DifficultyIllegalTech = "difficulty-illegaltech";
    public const string DifficultyPromotions = "difficulty-promotions";
    public const string DifficultyOppression = "difficulty-oppression";
    public const string DifficultyDystopia = "difficulty-dystopia";

    public const string MiscDataAnalyst = "misc-data-analyst"; // NOTE: Referenced from Unity Side
    public const string MiscGirlPower = "misc-girl-power";
    public const string MiscShoppingSpree = "misc-shopping-spree";
    public const string MiscBoughtAnEpicCard = "misc-pay-to-win";
    public const string MiscHealedInjury = "misc-healed-injury";
    public const string MiscSkippedCutscene = "misc-skipped-cutscene";

    public const string PlaystyleOneShot = "playstyle-one-shot";
    
    public const string Progress9HeroesUnlocked = "progress-9-heroes";
    public const string Progress50Percent = "progress-50-percent";
    public const string Progress100Percent = "progress-100-percent";

    public static string HeroVictory(string heroName) => $"hero-{heroName.ToLowerInvariant()}";
    public static string HeroMasterVictory(string heroName) => $"hero-master-{heroName.ToLowerInvariant()}";
}

