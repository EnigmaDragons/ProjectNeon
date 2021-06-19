using System;

public class ShowTwoChoiceDialog
{
    public bool UseDarken { get; set; } = true;
    public string Prompt { get; set; }
    public Action PrimaryAction { get; set; } = () => { };
    public string PrimaryButtonText { get; set; }
    public Action SecondaryAction { get; set; } = () => { };
    public string SecondaryButtonText { get; set; }
}
