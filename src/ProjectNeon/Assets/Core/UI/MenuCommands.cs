using System.Linq;
using UnityEngine;

public sealed class MenuCommands : MonoBehaviour
{
    [SerializeField] private NamedCommand[] commands;
    [SerializeField] private GameObject buttonParent;
    [SerializeField] private TextCommandButton buttonTemplate;

    private IndexSelector<TextCommandButton> _cursor;

    private void Awake()
    {
        for(var i = 0; i < buttonParent.transform.childCount; i++)
            buttonParent.transform.GetChild(i).gameObject.SetActive(false);
        _cursor = new IndexSelector<TextCommandButton>(InstantiateButtons());
    }

    private void Start() => _cursor.Current.Select();

    private TextCommandButton[] InstantiateButtons() 
        => commands
            .Select(c => Instantiate(buttonTemplate, buttonParent.transform).Initialized(c))
            .ToArray();

    public void MoveNext() => _cursor.MoveNext().Select();
    public void MovePrevious() => _cursor.MovePrevious().Select();
    public void ExecuteSelectedCommand() => _cursor.Current.Execute();
}
