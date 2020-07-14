using UnityEngine;
using UnityEngine.UI;

public sealed class OnClickPlayUiSound : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private AudioClip sound;
    [SerializeField] private UiSfxPlayer player;
    [SerializeField] private FloatReference volume = new FloatReference(1f);

    void Awake() => button.onClick.AddListener(() => player.Play(sound, volume));
}
