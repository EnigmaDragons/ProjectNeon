using System;

public class GetUserSelectedHero
{
    public BaseHero[] Options { get; }
    public string PromptTerm { get; }
    public Action<BaseHero> OnSelected { get; }

    public GetUserSelectedHero(string promptTerm, BaseHero[] options, Action<BaseHero> onSelected)
    {
        PromptTerm = promptTerm;
        Options = options;
        OnSelected = onSelected;
    }
}
