
public class ShowInfoDialog
{
    public bool UseDarken { get; }
    public string Info { get; }
    public string DoneButtonText { get; }

    public ShowInfoDialog(string info, string doneButtonText, bool useDarken = true)
    {
        UseDarken = useDarken;
        Info = info;
        DoneButtonText = doneButtonText;
    }
}