using System;
using I2.Loc;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyPresenter : MonoBehaviour
{
    [SerializeField] private Localize nameLabel;
    [SerializeField] private Localize changes;
    [SerializeField] private Localize description;
    [SerializeField] private Button selectButton;
    [SerializeField] private Image difficultyImage;
    [SerializeField] private GameObject lockVisual;
    [SerializeField] private SelectableComponent selectable;

    public void Init(Difficulty difficulty, bool locked, Action onSelect)
    {
        nameLabel.SetTerm(difficulty.NameTerm);
        changes.SetTerm(difficulty.ChangesTerm);
        description.SetTerm(difficulty.DescriptionTerm);
        difficultyImage.sprite = difficulty.Image;
        
        selectButton.onClick.AddListener(() => onSelect());
        selectButton.enabled = !locked;
        lockVisual.SetActive(locked);
        if (locked)
            Destroy(selectable);
    }
}