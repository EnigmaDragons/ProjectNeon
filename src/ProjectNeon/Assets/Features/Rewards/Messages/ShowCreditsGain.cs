
public class ShowCreditsGain
{
    public Rarity Rarity { get; }
    public int NumCredits { get; }

    public ShowCreditsGain(Rarity rarity, int numCredits)
    {
        Rarity = rarity;
        NumCredits = numCredits;
    }
}
