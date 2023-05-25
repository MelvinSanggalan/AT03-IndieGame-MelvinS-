using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* Script created by Melvin Jr Sanggalan
 * Last updated 22/05/2023
 * Script for the player's movement and mouselook.
 */

public class MouseLook : MonoBehaviour
{
    public float sensitivity = 2.5f;
    public float drag = 1.5f;

    private Vector2 mouseDir;
    private Vector2 smoothing;
    private Vector2 result;
    private Transform character;

    //crosshair references
    public Image crosshair;
    public Image crosshair2;

    public Text cooldownText;

    //cooldown bool for object interact
    public bool onCooldownObject = false;

    //cooldown bool for enemy interact
    public bool onCooldownEnemy = false;

    //reference to slash sfx
    public GameObject slashSFX;






    private void Awake()
    {
        character = transform.parent;

        Cursor.lockState = CursorLockMode.Locked;   // keep confined to center of screen
    }


    // Update is called once per frame
    void Update()
    {
        //camera movement
        mouseDir = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * sensitivity;
        smoothing = Vector2.Lerp(smoothing, mouseDir, 1 / drag);
        result += smoothing;
        result.y = Mathf.Clamp(result.y, -70, 70);

        //transform.localRotation = Quaternion.AngleAxis(-result.y, Vector3.right);
        character.rotation = Quaternion.AngleAxis(result.x, character.up);


        //crosshair change colour if its an interactable object
        RaycastHit hitInteractable;

        //use raycast to check for interactable objects
        if(Physics.Raycast(transform.position, transform.forward, out hitInteractable, 6))
        {
            //check if object has IInteraction
            if(hitInteractable.collider.gameObject.TryGetComponent<IInteraction>(out IInteraction inter) == true)
            {
                //check if object has Enemy tag and make the crosshair a slash symbol. If not, then make crosshair green.
                if(hitInteractable.collider.gameObject.tag == "Enemy")
                {
                    crosshair2.gameObject.SetActive(true);
                    crosshair.gameObject.SetActive(false);
                }
                else
                {
                    crosshair.color = Color.green;
                    crosshair.gameObject.SetActive(true);
                    crosshair2.gameObject.SetActive(false);
                }
            }
            else
            {
                if (crosshair != null && crosshair.color != Color.white)
                {
                    crosshair.color = Color.white;
                    crosshair.gameObject.SetActive(true);
                    crosshair2.gameObject.SetActive(false);
                }
            }
        }
        //if nothing is detected by raycast then make crosshair white.
        else
        {
            crosshair.color = Color.white;
            crosshair.gameObject.SetActive(true);
            crosshair2.gameObject.SetActive(false);
        }


        //if player left clicks or xbox controller rt
        if (Mathf.Round(Input.GetAxisRaw("Fire1")) > 0 || Input.GetButton("Fire1"))
        {
            Interact();
        }

        //if player presses escape it closes their game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Player quit game.");
            Application.Quit();
        }

    }


    //mine: raycast for interaction
    private void Interact()
    {
        RaycastHit hit;

        if(Physics.Raycast(transform.position, transform.forward, out hit, 6))
        {
            if(hit.collider.gameObject.TryGetComponent<IInteraction>(out IInteraction inter))
            {
                if(onCooldownObject == false)
                {
                    if(hit.collider.gameObject.tag == "Object")
                    {
                        onCooldownObject = true;
                        StartCoroutine(cooldownTimeObject());
                        inter.Activate();
                    }
                }
                if(onCooldownEnemy == false)
                    if (hit.collider.gameObject.tag == "Enemy")
                    {
                        onCooldownEnemy = true;
                        StartCoroutine(cooldownTimeEnemy());
                        inter.Activate();
                    }
            }
        }

        //cooldown for object interaction
        IEnumerator cooldownTimeObject()
        {
            Debug.Log("Object interaction cooldown start.");

            //1 second wait time
            yield return new WaitForSeconds(1.5f);

            //bool turn false and cooldown end
            Debug.Log("Object interaction cooldown end!!!!!!!!!");
            onCooldownObject = false;

        }

        //cooldown for enemy interaction
        IEnumerator cooldownTimeEnemy()
        {
            Debug.Log("Enemy interaction cooldown start.");

            //play slash sfx
            slashSFX.SetActive(true);

            //ui cooldown timer
            cooldownText.text = "10";
            yield return new WaitForSeconds(1);
            cooldownText.text = "9";
            yield return new WaitForSeconds(1);
            cooldownText.text = "8";
            yield return new WaitForSeconds(1);
            cooldownText.text = "7";
            yield return new WaitForSeconds(1);
            cooldownText.text = "6";
            yield return new WaitForSeconds(1);
            cooldownText.text = "5";
            yield return new WaitForSeconds(1);
            cooldownText.text = "4";
            yield return new WaitForSeconds(1);
            cooldownText.text = "3";
            yield return new WaitForSeconds(1);
            cooldownText.text = "2";
            yield return new WaitForSeconds(1);
            cooldownText.text = "1";
            yield return new WaitForSeconds(1);
            cooldownText.text = " ";

            //bool turn false and cooldown end
            Debug.Log("Enemy interaction cooldown end!!!!!!!!!");
            slashSFX.SetActive(false);
            onCooldownEnemy = false;

        }


    }    


}

//mine: finitestatemachine for interaction
public interface IInteraction
{
    public void Activate();
}