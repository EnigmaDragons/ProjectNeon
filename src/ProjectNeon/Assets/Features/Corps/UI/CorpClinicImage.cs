using UnityEngine;
using UnityEngine.UI;

public class CorpClinicImage : CorpUiBase
{
    [SerializeField] private Image corpClinicImage;
    
    public override void Init(Corp c)
    {
        var clinicImage = c.ClinicImage;
        if (clinicImage == null)
            corpClinicImage.enabled = false;
        else
        {
            corpClinicImage.enabled = true;
            corpClinicImage.sprite = clinicImage;
        }
    }
}
