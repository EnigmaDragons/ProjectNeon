using System.Linq;
using UnityEngine;

public class AutoInitSystemLanguageOnAwake : MonoBehaviour
{
    [SerializeField] private LanguageSelectionPresenter presenter; 
    
    private void Awake()
    {
        if (CurrentAcademyData.Data.HasSelectedLanguage)
            return;

        var d = presenter.Options.Where(x => x.Enabled).SafeToDictionary(x => x.UnityLanguage, x => x);
        if (d.TryGetValue(Application.systemLanguage, out var l))
            l.Select();
    }
}
