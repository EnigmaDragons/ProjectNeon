using I2.Loc;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyItemCompletionPresenter : MonoBehaviour
{
    [SerializeField] private Image itemArtImage;
    [SerializeField] private Localize difficultyLabel;
    [SerializeField] private GameObject completedCheck;
    //[SerializeField] private Localize nameLabel;

    public void Init(Sprite itemArt, Maybe<Difficulty> difficulty, bool isComplete)
    {
        itemArtImage.sprite = itemArt;
        if (difficulty.IsPresent)
            difficultyLabel.SetTerm(difficulty.Value.NameTerm);
        else
            difficultyLabel.SetTerm("Progressions/Incomplete");
        gameObject.SetActive(true);
        completedCheck.SetActive(isComplete);
    }
    
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}


