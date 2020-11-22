using UnityEngine;

public class EnableOnMapVersion : MonoBehaviour
{
    [SerializeField] private CurrentAdventure current;
    [SerializeField] private bool version1Only;
    [SerializeField] private GameObject toEnable;

    private void Awake()
    {
        if ((current.Adventure.IsV2 && !version1Only) || (!current.Adventure.IsV2 && version1Only))
            toEnable.SetActive(true);
    }
}