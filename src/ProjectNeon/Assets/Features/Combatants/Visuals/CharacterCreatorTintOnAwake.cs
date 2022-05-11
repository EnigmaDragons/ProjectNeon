using CharacterCreator2D;
using UnityEngine;

public class CharacterCreatorTintOnAwake : MonoBehaviour
{
    [SerializeField] private Color tint;
    [SerializeField] private CharacterViewer viewer;

    private void Awake() => viewer.TintColor = tint;
}
