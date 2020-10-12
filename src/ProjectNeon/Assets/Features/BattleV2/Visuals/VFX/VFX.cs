using UnityEngine;

public class VFX : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] particles;
    [SerializeField] private GameObject[] animators;

    public void Play()
    {
        particles.ForEach(x => x.Play());
        animators.ForEach(x => x.SetActive(true));
    }
}