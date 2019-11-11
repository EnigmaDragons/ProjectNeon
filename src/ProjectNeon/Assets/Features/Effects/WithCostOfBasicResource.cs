class WithCostOfBasicResource : Effect
{
    private Effect _effect;
    private int _quantity;

    public WithCostOfBasicResource (Effect effect, int quantity)
    {
        _effect = effect;
        _quantity = quantity;
    }

    void Effect.Apply(Member source, Target target)
    {
        throw new System.NotImplementedException();
    }
}

