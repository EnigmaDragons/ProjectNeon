using AeLa.EasyFeedback;
using UnityEngine;

public class UseDefaultListId : FormElement
{
    [SerializeField] private int columnIndex = 1;
    
    protected override void FormOpened()
    {
    }

    protected override void FormSubmitted()
    {
        Form.CurrentReport.List.id = Form.Config.Board.CategoryIds[columnIndex];
        Form.CurrentReport.List.name = Form.Config.Board.CategoryNames[columnIndex];
    }

    protected override void FormClosed()
    {
    }
}
