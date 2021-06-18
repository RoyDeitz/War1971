using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    //Reference to the Character controller
    CharacterController controller;
    public Joystick joystick;
    public Transform cameraTransform;
    public Transform spawnTransform;
    public RectTransform center;
    public RectTransform joystickDirection;

    //Player Movement Speed
    public float movementSpeed = 12f;
    public float runningSpeed;
    public float walkingSpeed;
    public float crouchungSpeed;
    public float sprintFactor = 2f;
    Vector3 movementVector;

    //Gravity/Jumping Velocity
    Vector3 verticalVelocity;
    public float jumpHeight = 2f;
    public float gravity = -9.81f; //9.81 m/s²

    //Check if grounded
    public Transform groundCheck;
    public LayerMask groundLayer;
    bool isGrounded;

    void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
        isGrounded = false;
        transform.position = spawnTransform.position;
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, 5f, groundLayer);

        float x = joystick.Horizontal * movementSpeed;
        float z = joystick.Vertical * movementSpeed;


        //Only change the movement Vector if grounded
        if (isGrounded)
        {

            movementVector = new Vector3(x, 0f, z);
            movementVector= movementVector.normalized;
            
            if (Mathf.Abs(z) >= .2f || Mathf.Abs(x) > .2f)
            {
                transform.forward = movementVector;
                
            }

            if (Mathf.Abs(x) >= .99f || Mathf.Abs(z) >= .99f) movementSpeed = runningSpeed;
            else movementSpeed = walkingSpeed;

            if (movementVector.magnitude >1f)
            {
                movementVector = movementVector.normalized;
                
            }
            
        }

        //Double the speed on sprint button

        //if (Input.GetKey(KeyCode.LeftShift) && z > 0 && isGrounded == true) movementVector = movementVector * sprintFactor;

        controller.Move(movementVector * movementSpeed * Time.deltaTime);
        

        //Jump
        /*
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded == true)
        {
            verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        */

        //Reset the vertical velocity to avoid it being to strong
        if (isGrounded == true && verticalVelocity.y < 0f)
        {
            verticalVelocity.y = -1f;
        }
        else
        {
            //Apply gravity
            verticalVelocity.y += gravity * Time.deltaTime;
        }

        controller.Move(verticalVelocity * Time.deltaTime);
    }

    public void ResetPos() 
    {
        transform.position = spawnTransform.position;
    }
}
