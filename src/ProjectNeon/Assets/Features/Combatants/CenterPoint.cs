using UnityEngine;

public class CenterPoint : MonoBehaviour
{
    [SerializeField] private GameObject centerPoint;

    public Vector3 Position => centerPoint.transform.position;
}