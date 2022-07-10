using UnityEngine;

public class SetCharacterCreatorIdle : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private string paramName;
    
    private void Start() => animator.SetBool(paramName, true);
}
