using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BattleSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] PlayerBattleSystem playerBattleSystem;
    [SerializeField] GameObject battleCanvas;
    [SerializeField] GameObject attackButton;

    BossBattleSystem bossBattleSystem;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Boss")
        {
            //When player colides with boss, this script takes the BossBattleSystem script from the collision as a reference
            bossBattleSystem = collision.GetComponent<BossBattleSystem>();

            //The BossBattleSystem gets a reference of this script
            bossBattleSystem.BattleSystem = this;

            //Stops player movement and enables the battle canvas
            playerMovement.StopPlayerMovement();
            ToggleBattleCanvas(true);

            //Calculates player intial position, distance for melee attack for player and boss
            SetBattlePositons();          
        }
    }

    public void BossTurn()
    {
        //Waits before taking boss action so the fight isnt too fast
        StartCoroutine(WaitAndDoBossAttack(1));
    }

    public void HurtBoss()
    {
        bossBattleSystem.Hurt();
    }

    public void HurtPlayer()
    {
        playerBattleSystem.HurtPlayer();
    }

    public void TogglePlayerAttackButtons(bool state)
    {
        attackButton.SetActive(state);
    }

    void ToggleBattleCanvas(bool state)
    {
        battleCanvas.SetActive(state);
    }

    void SetBattlePositons()
    {
        //Saves the player inital position
        playerBattleSystem.PlayerInitialPosition = playerBattleSystem.transform.position;

        //Calculates player melee positon based on the boss position and his distance for player melee attack based on boss size, the Y cordinate is from the player position so the doesnt have to move up or down
        playerBattleSystem.PlayerMeleePosition = new Vector3(bossBattleSystem.transform.position.x - bossBattleSystem.playerMeleeAttackDistance, playerBattleSystem.PlayerInitialPosition.y);

        //Calculates boss melee position based on the inital player position and the selected boss malee position based on boss size, the Y cordinate is from boss position so he doesnt have to move up or down
        bossBattleSystem.BossMeleePosition = new Vector3(playerBattleSystem.PlayerInitialPosition.x + bossBattleSystem.bossMeleeAttackDistance, bossBattleSystem.transform.position.y);
    }

    IEnumerator WaitAndDoBossAttack(float delay)
    {
        yield return new WaitForSeconds(delay);
        bossBattleSystem.Attack();
    }

    IEnumerator WaitAndTurnOnPlayerAttackButtons(float delay)
    {
        yield return new WaitForSeconds(delay);
        TogglePlayerAttackButtons(true);
    }
}
