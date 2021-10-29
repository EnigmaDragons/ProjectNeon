using UnityEngine;

[CreateAssetMenu(menuName = "GameContent/CharacterAnimation")]
public class CharacterAnimation : ScriptableObject
{
    [SerializeField] private CharacterAnimationStep[] steps;

    public CharacterAnimationStep[] Steps => steps;
}