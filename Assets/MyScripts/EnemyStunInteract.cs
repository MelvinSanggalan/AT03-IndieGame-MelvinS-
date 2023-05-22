using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Script created by Melvin Jr Sanggalan
 * Last updated 20/05/2023
 * Allows the player to interact with an enemy to put them into the StunnedState.
 */

public class EnemyStunInteract : MonoBehaviour, IInteraction
{
   
    //reference to enemy's EnemyPathfinding script
    private EnemyPathfinding enemyScript;

    // Start is called before the first frame update
    void Start()
    {
        //set enemyscript to EnemyPathfinding script
        enemyScript = transform.GetComponent<EnemyPathfinding>();
    }

    public void Activate()
    {
        Debug.Log("Enemy stunned.");

        //set enemy state to chase
        enemyScript.StateMachine.SetState(new EnemyPathfinding.StunnedState(enemyScript));
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
