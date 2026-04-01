using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;


public class playerMovement : MonoBehaviour
{

[SerializeField]private float speed = 5f;
private Rigidbody2D rb;

private Vector2 moveInput;

private Animator animator;

private bool playingFootSteps = false;
public float footstepSpeed = 0.5f;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        if (PauseController.IsGamePaused)
        {
            rb.linearVelocity = Vector2.zero;
             animator.SetBool("isWalking", false);
             return;
        }
        rb.linearVelocity = moveInput * speed;
                     animator.SetBool("isWalking", rb.linearVelocity.magnitude > 0);

                     if(rb.linearVelocity.magnitude > 0  && !playingFootSteps)
        {
            StartFootSteps();
        }
        else if (rb.linearVelocity.magnitude == 0)
        {
            StopFootSteps();
        }
        ;


    }



public void Move(InputAction.CallbackContext context)
{
    if (context.canceled)
    {
        animator.SetBool("isWalking", false);

        animator.SetFloat("LastinputX", moveInput.x);
        animator.SetFloat("LastinputY", moveInput.y);

        moveInput = Vector2.zero; 
        return;
    }

    moveInput = context.ReadValue<Vector2>();
    animator.SetFloat("InputX", moveInput.x);
    animator.SetFloat("InputY", moveInput.y);
}

void StopFootSteps()
    {
        playingFootSteps = false;
        CancelInvoke(nameof(PLayFootstep));
    }



    void StartFootSteps()
    {
        playingFootSteps = true;
        InvokeRepeating(nameof(PLayFootstep), 0f, footstepSpeed);
        
    }

    void PLayFootstep()
    {
        SoundManager.Play("Steps" , true);
    }

}
