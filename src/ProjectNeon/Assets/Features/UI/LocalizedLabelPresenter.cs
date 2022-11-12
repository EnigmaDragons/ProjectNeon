using I2.Loc;
using UnityEngine;

public class LocalizedLabelPresenter : MonoBehaviour
{
    [SerializeField] private Localize label;

    public Localize Label => label;

    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);
}
