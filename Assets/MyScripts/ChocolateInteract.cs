using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Script created by Melvin Jr Sanggalan
 * Last updated 20/05/2023
 * Script makes the enemy AI into an infinite ChaseState as well as enable the Barricades and WindowEscape gameObjects.
 */

public class ChocolateInteract : MonoBehaviour, IInteraction
{
    //reference to enemy
    public GameObject enemy;
    //reference to enemy's EnemyPathfinding script
    private EnemyPathfinding enemyScript;
    //reference to barricades
    public GameObject barricades;
    //reference to windowescape
    public GameObject windowEscape;
    //reference to chasewarningtext
    public GameObject chaseWarningText;


    // Start is called before the first frame update
    void Start()
    {
        //set enemyscript to EnemyPathfinding script
        enemyScript = enemy.GetComponent<EnemyPathfinding>();
    }

    public void Activate()
    {
        Debug.Log("Chocolate collected.");

        //detection distance to 200 so the player chases enemy no matter the distance
        enemyScript.detectionDistance = 200;

        //make cookie model and spotlight invisible
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);

        //make barricades, windowescape, and chasewarningtext visible
        barricades.SetActive(true);
        windowEscape.SetActive(true);
        chaseWarningText.SetActive(true);

        //set enemy state to chase
        enemyScript.StateMachine.SetState(new EnemyPathfinding.ChaseState(enemyScript));
    }

}

