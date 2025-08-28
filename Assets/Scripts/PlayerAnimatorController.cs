using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimatorController : MonoBehaviour {
    [Header("References")] 
    [SerializeField] private Rigidbody2D rbReference;
    
    private Animator animator;
    
    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Update() {
        if (!rbReference) return;
        animator.SetBool("walking", rbReference.linearVelocity.magnitude > 0.01f);
    }
}
