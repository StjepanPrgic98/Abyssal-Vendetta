using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBattleSystem : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] protected Transform bossTransform;
    [SerializeField] protected Animator animator;

    [Header("Variables")]
    [SerializeField] float moveSpeedDuringBattle;
    [SerializeField] Vector2 leftSpriteScale;
    [SerializeField] Vector2 rightSpriteScale;
    [SerializeField] public float playerMeleeAttackDistance;
    [SerializeField] public float bossMeleeAttackDistance;
    [SerializeField] int timesHitBeforeSpecialAttack;

    public BattleSystem BattleSystem { get; set; }
    public Vector3 BossMeleePosition { get; set; }
    protected int TimesHit { get; set; }


    Vector3 initalPosition;

    bool moveDuringBattle;
    bool moveToMeleePosition;
    bool moveToInitalPosition;


    void Update()
    {
        //If an attack involves moving the boss during battle, moveDuringBattle will be set to true
        if (!moveDuringBattle) { return; }

        //Depending on the attack for the boss, different moving methods are called
        if (moveToMeleePosition)
        {
            MoveToMeleePostion();
        }
        if (moveToInitalPosition)
        {
            MoveToInitalPositon();
        }
    }

    public void Attack()
    {
        //If the boss was hit enough times to do a special attack, then the regular attack wont be executed
        if (CheckHowManyTimesBossWasHit()) { return; }

        //Sets the boss initial position, also allows him to move during battle
        initalPosition = bossTransform.position;
        moveDuringBattle = true;
        moveToMeleePosition = true;

        animator.SetBool("isRunning", true);
    }

    public void Hurt()
    {
        //Plays hurt animation and resets it after specific time
        animator.SetBool("isHurt", true);
        StartCoroutine(WaitAndTurnOffAnimation("isHurt", 0.7f));

        //Everytime Hurt method is called, means the boss was hit, so the TimesHit increases
        TimesHit++;
    }


    IEnumerator WaitAndTurnOffAnimation(string animation, float delay)
    {
        yield return new WaitForSeconds(delay);
        animator.SetBool(animation, false);
    }

    IEnumerator WaitAndHurtPlayer(float delay)
    {
        yield return new WaitForSeconds(delay);
        BattleSystem.HurtPlayer();
    }

    IEnumerator WaitAndMoveToInitalPositon(float delay)
    {
        yield return new WaitForSeconds(delay);

        //Waits certian time, allows boss to move, and flips his sprite in correct direction
        moveToInitalPosition = true;
        animator.SetBool("isRunning", true);
        FlipSprite("left");
    }

    IEnumerator WaitAndTurnOnPlayerAttackButtons(float delay)
    {
        //After boss did his attack, it waits certian time and turns on player attack buttons
        yield return new WaitForSeconds(delay);
        BattleSystem.TogglePlayerAttackButtons(true);
    }

    void MoveToMeleePostion()
    {
        //Moves the boss to the specific position to perform his melee attack
        bossTransform.position = Vector3.MoveTowards(bossTransform.position, BossMeleePosition, moveSpeedDuringBattle);

        if (bossTransform.position == BossMeleePosition)
        {
            //When the boss is in position, movement is stoped
            moveToMeleePosition = false;

            //Attack animation is triggered and turned of after specific time
            animator.SetBool("isRunning", false);
            animator.SetBool("isAttacking", true);
            StartCoroutine(WaitAndTurnOffAnimation("isAttacking", 1f));

            //Hurts player after specific time
            StartCoroutine(WaitAndHurtPlayer(0.7f));

            //Waits after attack and allows boss to move to inital position
            StartCoroutine(WaitAndMoveToInitalPositon(1.2f));
        }
    }

    void MoveToInitalPositon()
    {
        //Moves the boss to his inital position after the attack
        bossTransform.position = Vector3.MoveTowards(bossTransform.position, initalPosition, moveSpeedDuringBattle);

        if (bossTransform.position == initalPosition)
        {
            //When the boss is in his inital position, movement is stoped
            moveDuringBattle = false;
            moveToInitalPosition = false;

            //Idle animation plays, and sprite is fliped in correct direction
            animator.SetBool("isRunning", false);
            FlipSprite("right");

            //After the boss is in inital position, its considered as end of his turn, so it waits for certian
            //time and gives player his attack buttons
            StartCoroutine(WaitAndTurnOnPlayerAttackButtons(1));
        }
    }

    void FlipSprite(string direction)
    {
        if (direction == "left")
        {
            bossTransform.localScale = leftSpriteScale;
        }
        else if (direction == "right")
        {
            bossTransform.localScale = rightSpriteScale;
        }
    }

    bool CheckHowManyTimesBossWasHit()
    {
        //Checks how many times the boss was hit compared to how many times he has to be hit for his special attack
        if(TimesHit == timesHitBeforeSpecialAttack)
        {
            SpecialAttack();
            return true;
        }

        return false;
    }


    //Special Attack Method made to be overriden by other battle systems
    public virtual void SpecialAttack()
    {
        
    }
}
