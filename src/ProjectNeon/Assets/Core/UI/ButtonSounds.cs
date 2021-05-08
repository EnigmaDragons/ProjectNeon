using UnityEngine;

public class ButtonSounds : MonoBehaviour
{
    [SerializeField] private AudioClipVolume bigClickSound;
    [SerializeField] private AudioClipVolume standardClickSound;
    [SerializeField] private bool useBigClickSound;
    [SerializeField] private AudioClipVolume hoverSound;
    [SerializeField] private UiSfxPlayer player;

    public void OnClick() => player.Play(useBigClickSound ? bigClickSound : standardClickSound);
    public void OnHover() => player.Play(hoverSound);

    public void SetAsPrimary() => useBigClickSound = true;
}
