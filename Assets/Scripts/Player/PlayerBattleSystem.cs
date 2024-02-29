using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBattleSystem : MonoBehaviour
{
    [Header("Refrences")]
    [SerializeField] BattleSystem battleSystem;

    [Header("Components")]
    [SerializeField] Transform playerTransform;
    [SerializeField] Animator animator;

    [Header("Variables")]
    [SerializeField] float moveSpeedDuringBattle;

    public Vector3 PlayerMeleePosition { get; set; }
    public Vector3 PlayerInitialPosition { get; set; }

    bool movePlayerDuringBattle;
    bool moveToMeleePosition;
    bool moveToInitalPosition;

    void Update()
    {
        //Any action that involves player moving during battle will set this to true
        if (!movePlayerDuringBattle) { return; }

        //Specific cases
        if (moveToMeleePosition)
        {
            MovePlayerToMeleePosition();
        }
        if (moveToInitalPosition)
        {
            MovePlayerToInitalPosition();
        }     
    }

    public void Attack()
    {
        //Turns off player attack options so he cant perform multiple moves per turn
        battleSystem.TogglePlayerAttackButtons(false);

        //Allow the player to move and set animation to run
        movePlayerDuringBattle = true;
        moveToMeleePosition = true;
        animator.SetBool("isRunning", true);
        
    }

    public void HurtPlayer()
    {
        animator.SetBool("isHurt", true);
        StartCoroutine(WaitAndTurnOffPlayerAnimation("isHurt", 0.7f));
    }

    void MovePlayerToMeleePosition()
    {
        //Moves player until he reaches the position to perform melee attack
        playerTransform.position = Vector3.MoveTowards(playerTransform.position, PlayerMeleePosition, moveSpeedDuringBattle);

        if (playerTransform.position == PlayerMeleePosition)
        {
            //Stop player moving and turn of run animation
            moveToMeleePosition = false;
            animator.SetBool("isRunning", false);

            //Attacking animation, Boss damage, Player run back after attack
            AttackAndHurtBoss(0.7f, 0.3f);
        }
    }

    void MovePlayerToInitalPosition()
    {
        //Move player until he reaches his inital position before the attack
        playerTransform.position = Vector3.MoveTowards(playerTransform.position, PlayerInitialPosition, moveSpeedDuringBattle);

        if (playerTransform.position == PlayerInitialPosition)
        {
            //Stops player from moving, flips sprite to face the boss and turns of attack animation
            movePlayerDuringBattle = false;
            moveToInitalPosition = false;
            FlipSprite("right");
            animator.SetBool("isRunning", false);

            //Waits and starts boss turn
            StartCoroutine(WaitAndStartBossTurn(1));
        }
    }

    void AttackAndHurtBoss(float animtionEndTime, float hurtBossTime)
    {
        animator.SetBool("isAttacking", true);

        //Stops attack animation after certian time
        StartCoroutine(WaitAndTurnOffPlayerAnimation("isAttacking", 0.7f));

        //Hurts boss after certian time depending on the attack animation length
        StartCoroutine(WaitAndHurtBoss(0.3f));

        //Allow player to move back after the attack
        StartCoroutine(WaitAndMoveToInitialPosition(animtionEndTime + 0.5f));
    }

    void FlipSprite(string direction)
    {
        if (direction == "left")
        {
            playerTransform.localScale = new Vector2(-1f, 1f);
        }
        else if (direction == "right")
        {
            playerTransform.localScale = new Vector2(1f, 1f);
        }
    }

    //Resets player to idle at chosen time
    IEnumerator WaitAndTurnOffPlayerAnimation(string animation, float animationDelay)
    {
        yield return new WaitForSeconds(animationDelay);
        animator.SetBool(animation, false);
    }

    IEnumerator WaitAndStartBossTurn(float delay)
    {
        yield return new WaitForSeconds(delay);
        battleSystem.BossTurn();
    }

    IEnumerator WaitAndHurtBoss(float delay)
    {
        yield return new WaitForSeconds(delay);
        battleSystem.HurtBoss();
    }

    IEnumerator WaitAndMoveToInitialPosition(float delay)
    {
        //After waiting allows player to move, starts running animation and flips the sprite left
        yield return new WaitForSeconds(delay);

        moveToInitalPosition = true;
        animator.SetBool("isRunning", true);
        FlipSprite("left");
    }
}
