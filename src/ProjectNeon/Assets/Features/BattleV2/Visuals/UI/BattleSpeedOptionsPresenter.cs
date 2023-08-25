using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleSpeedOptionsPresenter : MonoBehaviour
{
    [SerializeField] private Button normal;
    [SerializeField] private Button fast;
    [SerializeField] private Button faster;
    [SerializeField] private string[] battleSceneNames;

    public int CurrentFactor => CurrentGameOptions.Data.BattleSpeedFactor;
    
    private void Awake()
    {
        normal.onClick.AddListener(() => SetFactor(1));
        fast.onClick.AddListener(() => SetFactor(2));
        faster.onClick.AddListener(() => SetFactor(4));
        SetFactor(CurrentFactor);
    }
    
    private void SetFactor(int factor)
    {
        normal.interactable = factor != 1;
        fast.interactable = factor != 2;
        faster.interactable = factor != 4;
        if (battleSceneNames.Contains(SceneManager.GetActiveScene().name))
            Time.timeScale = factor;
        if (factor != CurrentFactor)
            Message.Publish(new BattleSpeedChanged(factor)); 
        CurrentGameOptions.SetBattleSpeed(factor);
    }
}