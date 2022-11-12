using System;

public class ShowLocalizedDialog
{
    public bool UseDarken { get; set; } = true;
    public string PromptTerm { get; set; }
    public Action PrimaryAction { get; set; } = () => { };
    public string PrimaryButtonTerm { get; set; }
    public Action SecondaryAction { get; set; } = () => { };
    public string SecondaryButtonTerm { get; set; }

    public static ShowLocalizedDialog Info(string infoTerm, string doneButtonTerm, bool useDarken = true)
        => new ShowLocalizedDialog
        {
            UseDarken = useDarken,
            PromptTerm = infoTerm,
            PrimaryButtonTerm = doneButtonTerm
        };
}
