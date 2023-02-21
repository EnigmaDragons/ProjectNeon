using System.Linq;
using UnityEngine;

public class CardShopkeeperLines : MonoBehaviour
{
    [SerializeField] private CardShopPresenter shop;

    private static readonly string[] StaticMessages =
    {
        "Welcome to my Cyber-Training Dojo. We have a preem selection of rare skills for you to learn.",
        "No practice needed for you to learn a new technique! Just plug and play!",
        "Grab whichever VR routine you need, then pop into any open neuro-transfer chair in the store.",
        "If you ask me, skills are much more important than gear or implants. Most gear is a rip-off.",
        "You can never go wrong with having more skills! The more skills you have, the more situations you'll be ready for.",
        "All gain, no pain. Your brain can do the training while your body rests. What do you want to learn?",
        "I can tell you guys are discerning benefactors. You know just what you need.",
        "If you need any suggestions about which skills to learn, I'm here for you.",
        "Our skill training isn't the cheapest, but we guarantee your privacy. The Corps don't need to know what you learn here.",
        "The streets don't seem quite as friendly as they used to be. Nothing a few more skills won't help solve, in my opinion.",
    };

    private static readonly string[] RareSkillsTemplates =
    {
        "Wow! We've got a [Skill] in stock! You don't see those everyday.",
        "If you've got the creds for it, I'd strongly recommend grabbing that [Skill]. Rumor has it, it's extraordinary!",
        "A discerning benefactor like yourself is probably salivating over that [Skill] right now."
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

    private void SaySomeStandardLine() => Say(StaticMessages.Random());

    private void CommentOnARareSkill()
    {
        var selection = shop.Selection;
        selection.Cards.OrderByDescending(c => (int)c.Rarity).FirstOrMaybe().ExecuteIfPresentOrElse(
            c => Say(RareSkillsTemplates.Random().Replace("[Skill]", c.Name)), 
            SaySomeStandardLine);
    }
    
    private void Say(string line) => Message.Publish(new ShowCharacterDialogueLine("Shopkeeper", line, forceAutoAdvanceBecauseThisIsASingleMessage: true));
}
