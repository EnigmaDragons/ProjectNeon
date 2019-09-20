using System.Collections;
using UnityEngine;

public class CardsSelectController : MonoBehaviour
{
    [SerializeField] ConfirmCardSelection ConfirmCardSelection;
    [SerializeField] CardPlayZones playZones;
    [SerializeField] GameObject targetPoint;
    [SerializeField] GameObject handArea;
    [SerializeField] float cardDelta;

    CardPresenter[] playerCards;
    private void Start()
    {
        ConfirmCardSelection.onConfirmation.Subscribe(FindSelectedCards, this);
    }
    public void FindSelectedCards()
    {
        playerCards = transform.GetComponentsInChildren<CardPresenter>();
        StartCoroutine(MoveToBottom());
    }
    // @todo #133: 60min Change card scale and rotation while moving down
    IEnumerator MoveToBottom()
    {
        handArea.SetActive(false);
        Vector3 targetPosition;
        int mediane = playerCards.Length / 2;
        bool stop = false;
        while (!stop)
        {
            yield return null;
            stop = true;
            for (int i = 0; i < playerCards.Length; i++)
            {
                targetPosition = targetPoint.transform.position;
                targetPosition.x += cardDelta * (i - mediane);
                CardMovement(i, targetPosition);
                if (Mathf.Abs(playerCards[i].transform.position.x - targetPosition.x) > 1) stop = false;
            }
        }
    }
    void CardMovement(int cardIndex, Vector3 targetPosition)
    {
        playerCards[cardIndex].transform.position = 
            Vector3.MoveTowards(playerCards[cardIndex].transform.position, targetPosition, 1500f * Time.deltaTime);

    }
}
