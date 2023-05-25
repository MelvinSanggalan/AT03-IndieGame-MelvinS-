using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/* Script created by Melvin Jr Sanggalan
 * Last updated 25/05/2023
 * Script moves player to another scene.
 */

public class StartGameScript : MonoBehaviour
{
    //reference to audio source
    private AudioSource audioSource;



    //Build numer of scene to start when start button is pressed
    public int gameStartScene;


    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void StartGame()
    {
        //play button sfx
        audioSource.Play();

        SceneManager.LoadScene(gameStartScene);

        Debug.Log("Player moved to a new scene.");
    }

    public void QuitGame()
    {
        Debug.Log("Player quit game.");
        Application.Quit();
    }

    public void Update()
    {
        //for xbox controller if they press A, it starts their game
        if(Input.GetButton("Submit"))
        {
            //play button sfx
            audioSource.Play();

            SceneManager.LoadScene(gameStartScene);

            Debug.Log("Player moved to a new scene.");
        }

        //if player presses escape it closes their game
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Player quit game.");
            Application.Quit();
        }
    }

}
