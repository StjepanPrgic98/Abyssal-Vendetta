using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] Rigidbody2D rBody;
    [SerializeField] Transform playerTransform;
    [SerializeField] Animator animator;

    [Header("Varbiables")]
    [SerializeField] float moveSpeed;
    [SerializeField] float jumpHeight;


    Vector2 moveInput;
    Vector2 flipedSprite = new Vector2(-1f, 1f);
    Vector2 regularSprite = new Vector2(1f, 1f);

    //Used when in battle
    bool stopPlayerMovement = false;


    //Player gets set as a child of the moving platform because he would fall off othervise
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("MovingPlatform"))
        {
            playerTransform.parent = collision.transform;
        }
    }

    //When he jumps of the moving platform he is no longer a chield
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("MovingPlatform"))
        {
            playerTransform.parent = null;
        }
    }



    void Update()
    {
        if (stopPlayerMovement) { return; }

        Run();
        FlipSprite();
    }

    void Run()
    {
        MovePlayer();
    }

    void OnMove(InputValue value)               
    {
        moveInput = value.Get<Vector2>();
    }

    void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            rBody.velocity += new Vector2(0f, jumpHeight);
        }
    }

    void MovePlayer()
    {
        Vector2 playerVelocity = new Vector2(moveInput.x * moveSpeed, rBody.velocity.y);
        rBody.velocity = playerVelocity;
        RunAnimation();
    }
    void FlipSprite()                           
    {
        if(rBody.velocity.x < 0)
        {
            playerTransform.localScale = flipedSprite;
        }
        else
        {
            playerTransform.localScale = regularSprite;
        }
    }

    void RunAnimation()
    {
        if(rBody.velocity.x == 0)
        {
            animator.SetBool("isRunning", false);
        }
        else
        {
            animator.SetBool("isRunning", true);
        }
    }

    //Called when entering battle mode
    public void StopPlayerMovement()
    {
        stopPlayerMovement = true;
        animator.SetBool("isRunning", false);
    }

}
