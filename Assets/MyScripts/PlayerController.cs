using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float gravity = 30f;
    public float jumpForce = 20f;
    public float doubleJumpForce = 10f;
    public float moveSpeed = 3.5f;

    //Value-type variable
    private float velocity = 0f;
    private float currentSpeed = 0f;
    private bool jumpedTwice = false;
    public Vector3 motionStep;

    //Reference-type variable
    public CharacterController controller;



    //Awake is called before Start
    private void Awake()
    {
        TryGetComponent(out controller);
    }


    // Start is called before the first frame update
    void Start()
    {
        currentSpeed = moveSpeed;
    }


    //FixedUpdate may run more than once per frame
    private void FixedUpdate()
    {
        if (controller.isGrounded == true)
        {
            velocity = -gravity * Time.deltaTime;
        }
        else
        {
            velocity -= gravity * Time.deltaTime;
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (controller.isGrounded == true)
        {
            if (jumpedTwice == true)
            {
                jumpedTwice = false;
            }

            if (Input.GetButtonDown("Jump") == true)
            {
                velocity = jumpForce;
            }
        }

        else if (jumpedTwice == false)
        {
            if (Input.GetButtonDown("Jump") == true)
            {
                jumpedTwice = true;
                velocity = doubleJumpForce;
            }
        }

        ApplyMovement();
    }

    private void ApplyMovement()
    {
        motionStep = Vector3.zero;
        motionStep += transform.forward * Input.GetAxisRaw("Vertical");
        motionStep += transform.right * Input.GetAxisRaw("Horizontal");
        motionStep = currentSpeed * motionStep;
        motionStep.y += velocity;
        controller.Move(motionStep  * Time.deltaTime);

    }
}
