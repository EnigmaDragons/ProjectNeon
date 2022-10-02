using System;
using UnityEngine;

[Obsolete]
public class SurveyButton : MonoBehaviour
{
    public void GoToSurvey()
    {
        Application.OpenURL("https://forms.gle/SsgkqR4D7GDLmS4Y9");
    }    
}