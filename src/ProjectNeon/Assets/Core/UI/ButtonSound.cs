using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public sealed class ButtonSound : MonoBehaviour
{
    [SerializeField] private AudioClip sound;
    [SerializeField, Range(0, 1)] private float volume = 0.5f;
    [SerializeField] private UiSfxPlayer player;

    private void Awake() => GetComponent<Button>().onClick.AddListener(() => player.Play(sound, volume));
}
