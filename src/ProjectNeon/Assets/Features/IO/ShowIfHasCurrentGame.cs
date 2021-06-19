using UnityEngine;

public class ShowIfHasCurrentGame : MonoBehaviour
{
    [SerializeField] private GameObject target;

    private void Start() => target.SetActive(CurrentGameData.HasActiveGame);
}