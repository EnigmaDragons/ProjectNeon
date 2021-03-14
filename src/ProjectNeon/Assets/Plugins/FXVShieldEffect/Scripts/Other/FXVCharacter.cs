using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FXV
{
    public class FXVCharacter : MonoBehaviour
    {
        public float moveSpeed = 1.0f;

        Animator animator;
        Vector3 startPosition;

        void Start()
        {
            animator = GetComponent<Animator>();
            startPosition = transform.position;
        }

        void Update()
        {
            animator.SetFloat("Forward", moveSpeed, 0.1f, Time.deltaTime);
            animator.SetFloat("Turn", 0.0f, 0.1f, Time.deltaTime);
            animator.SetBool("Crouch", false);
            animator.SetBool("OnGround", true);
            animator.SetFloat("Jump", 0.0f);
        }

        public void ResetPosition()
        {
            transform.position = startPosition;
        }

    }
}
