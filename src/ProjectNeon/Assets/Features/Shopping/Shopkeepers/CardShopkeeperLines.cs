using System.Linq;
using UnityEngine;

public class CardShopkeeperLines : MonoBehaviour, ILocalizeTerms
{
    [SerializeField] private CardShopPresenter shop;

    private static readonly string[] StaticMessages =
    {
        "CardShops/Line001",
        "CardShops/Line002",
        "CardShops/Line003",
        "CardShops/Line004",
        "CardShops/Line005",
        "CardShops/Line006",
        "CardShops/Line007",
        "CardShops/Line008",
        "CardShops/Line009",
        "CardShops/Line010",
    };

    private static readonly string[] RareSkillsTemplates =
    {
        "CardShops/RareSkill001",
        "CardShops/RareSkill002",
        "CardShops/RareSkill003",
    };

    private void OnEnable() => Async.ExecuteAfterDelay(0.5f, SaySomething);

    private void SaySomething()
    {
        if (!gameObject.activeInHierarchy)
            return;
        
        if (shop == null || Rng.Float() < 0.7)
            SaySomeStandardLine();
        else
            CommentOnARareSkill();
    }

    private void SaySomeStandardLine() => Say(StaticMessages.Random().ToLocalized());

    private void CommentOnARareSkill()
    {
        var selection = shop.Selection;
        selection.Cards.OrderByDescending(c => (int)c.Rarity).FirstOrMaybe().ExecuteIfPresentOrElse(
            c => Say(RareSkillsTemplates.Random().ToLocalized().SafeFormat(c.NameTerm.ToLocalized())), 
            SaySomeStandardLine);
    }
    
    private void Say(string line) => Message.Publish(new ShowNonCutsceneCharacterDialogueLine("Shopkeeper", line, forceAutoAdvanceBecauseThisIsASingleMessage: true));

    public string[] GetLocalizeTerms() => StaticMessages.Concat(RareSkillsTemplates).ToArray();
}
