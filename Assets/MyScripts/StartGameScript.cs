using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/* Script created by Melvin Jr Sanggalan
 * Last updated 22/05/2023
 * Script moves player to another scene.
 */

public class StartGameScript : MonoBehaviour
{

    //Build numer of scene to start when start button is pressed
    public int gameStartScene;

    public void StartGame()
    {
        SceneManager.LoadScene(gameStartScene);

        Debug.Log("Player moved to a new scene.");
    }

}
