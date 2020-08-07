using System.Collections;
using DG.Tweening;
using UnityEngine;

public sealed class ResolutionZoneOffset : MonoBehaviour
{
    [SerializeField] private float yOffset;
    [SerializeField] private CardsVisualizer cards;
    
    private Vector3 _cardsPosition;

    private void Awake() => _cardsPosition = cards.transform.position;
    
    public IEnumerator BeginResolutionPhase()
    {
        cards.transform.DOMove(_cardsPosition + new Vector3(0, yOffset, 0), 0.5f);
        yield return new WaitForSeconds(0.5f);
        cards.RefreshPositions();
    }

    public void EndResolutionPhase()
    {
        cards.transform.position = _cardsPosition;
        cards.RefreshPositions();
    }
}
