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

    //Player Movement Speed
    public float movementSpeed = 12f;
    public float sprintFactor = 2f;
    Vector3 movementVector;

    //Gravity/Jumping Velocity
    Vector3 verticalVelocity;
    public float jumpHeight = 2f;
    public float gravity = -9.81f; //9.81 m/s�

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

        //float x = Input.GetAxis("Horizontal");
        //float z = Input.GetAxis("Vertical");

        float x = joystick.Horizontal * movementSpeed;
        float z = joystick.Vertical * movementSpeed;
      
       
        //Only change the movement Vector if grounded
        if (isGrounded)
        {
            movementVector =transform.forward* z + transform.right * x;
            
       
            if (movementVector.magnitude > 1f)
                movementVector = movementVector.normalized;
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
