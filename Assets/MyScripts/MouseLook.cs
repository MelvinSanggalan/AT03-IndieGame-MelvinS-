using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseLook : MonoBehaviour
{
    public float sensitivity = 2.5f;
    public float drag = 1.5f;

    private Vector2 mouseDir;
    private Vector2 smoothing;
    private Vector2 result;
    private Transform character;

    //mine: crosshair reference
    public Image crosshair;

    //mine: cooldown bool
    public bool onCooldown = false;



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

        if(Physics.Raycast(transform.position, transform.forward, out hitInteractable, 6))
        {
            if(hitInteractable.collider.gameObject.TryGetComponent<IInteraction>(out IInteraction inter) == true)
            {
                crosshair.color = Color.green;
            }
            else
            {
                if (crosshair != null && crosshair.color != Color.white)
                {
                    crosshair.color = Color.white;
                }
            }
        }


        //mine: button that sends raycast for interaction
        if(Input.GetMouseButton(0))
        {
            Interact();
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
                if(onCooldown == false)
                {
                    onCooldown = true;
                    StartCoroutine(cooldownTime());
                    inter.Activate();
                }
            }
        }

        IEnumerator cooldownTime()
        {
            //put the player in a "stunned" animation
            Debug.Log("Interaction cooldown start.");

            //10 seconds wait time
            yield return new WaitForSeconds(10.5f);

            //bool turn false and cooldown end
            Debug.Log("Interaction cooldown end!!!!!!!!!");
            onCooldown = false;

        }

    }    


}

//mine: finitestatemachine for interaction
public interface IInteraction
{
    public void Activate();
}