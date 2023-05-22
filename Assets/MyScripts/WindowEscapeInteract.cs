using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Script created by Melvin Jr Sanggalan
 * Last updated 22/05/2023
 * Script puts the player in the WinGameScreen when the object it is attached to has been interacted
 */

public class WindowEscapeInteract : MonoBehaviour, IInteraction
{
    //reference to enemy
    public GameObject enemy;
    //reference to enemy's EnemyPathfinding script
    private EnemyPathfinding enemyScript;
    //reference to WinGameScreen
    public GameObject winGameScreen;


    // Start is called before the first frame update
    void Start()
    {
        //set enemyscript to EnemyPathfinding script
        enemyScript = enemy.GetComponent<EnemyPathfinding>();
    }

    public void Activate()
    {
        Debug.Log("Player escapes.");
        //open WinGameScreen
        winGameScreen.SetActive(true);
        //set enemy state to end
        enemyScript.StateMachine.SetState(new EnemyPathfinding.EndState(enemyScript));

        //make it so the player can see and move their mouse
        Cursor.lockState = CursorLockMode.None;
    }
}
