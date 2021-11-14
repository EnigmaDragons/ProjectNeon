using System;

public class GetUserSelectedHero
{
    public BaseHero[] Options { get; }
    public string Prompt { get; }
    public Action<BaseHero> OnSelected { get; }
    
    public GetUserSelectedHero(string prompt, BaseHero[] options, Action<BaseHero> onSelected)
    {
        Prompt = prompt;
        Options = options;
        OnSelected = onSelected;
    }
}
