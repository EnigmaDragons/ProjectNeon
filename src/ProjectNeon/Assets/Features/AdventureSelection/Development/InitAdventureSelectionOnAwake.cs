
using UnityEngine;

public class InitAdventureSelectionOnAwake : MonoBehaviour
{
    [SerializeField] private CurrentAdventure current;
    [SerializeField] private Adventure adventure;

    private void Awake() => current.Adventure = adventure;
}
