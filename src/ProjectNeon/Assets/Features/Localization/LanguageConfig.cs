using UnityEngine;

[CreateAssetMenu]
public class LanguageConfig : ScriptableObject
{
    [SerializeField] private LanguageOption[] languages;

    public LanguageOption[] Languages => languages;
}