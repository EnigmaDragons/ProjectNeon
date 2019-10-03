using System.Collections.Generic;
using UnityEngine;

public class DeckController : MonoBehaviour
{
    [SerializeField] CharacterController characterController;
    [SerializeField] List<DeckVisualizer> deckVisualizer;

    private void Start()
    {
        characterController.OnCharacterChanged += OnCharacterChanged;
    }
    private void OnCharacterChanged()
    {
        deckVisualizer.ForEach(x => x.UpdateDeckListView());
    }
    private void OnDisable()
    {
        characterController.OnCharacterChanged -= OnCharacterChanged;
    }
}
