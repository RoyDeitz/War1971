using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    
    //Gravity/Jumping Velocity
    Vector3 verticalVelocity;
    public float jumpHeight = 2f;
    public float gravity = -9.81f; //9.81 m/s²

    //Check if grounded
    public Transform groundCheck;
    public LayerMask groundLayer;
    bool isGrounded;


    //animation
    public Animator anim;

    //movement
    public float movementSpeed = 12f;
    public float runningSpeed;
    public float walkingSpeed;
    public float crouchungSpeed;
    Vector3 movementVector;

    //health and death
    public int health;
    bool isDead=false;
    int deathType;

    public enum AlertLevel 
    {
    Guard,
    Suspicious,
    Combat
    }
    public AlertLevel alertLevel;

    public enum EnemyType 
    {
    Soldier,
    Sniper,
    Officer
    }
    public EnemyType enemyType;

    public enum Weapon 
    {
    Rifle,
    SMG,
    Pistol
    }

    CharacterController enemyController;

    public bool isPlayerFound=false;
    public bool isHeaing;
    public bool isObjFound;
    public Transform eyeTransform;
    public LineRenderer lr;
    public Vector3 objPosition;




    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        enemyController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, 5f, groundLayer);

        if (isObjFound)
        {
            lr.positionCount = 2;
            lr.startWidth = 5f;
            lr.endWidth = 10;
            lr.SetPosition(0, eyeTransform.position);
            lr.SetPosition(1, objPosition);


        }
        else 
        {
            lr.positionCount = 0;
        }

        //movement and gravity

        enemyController.Move(movementVector * movementSpeed * Time.deltaTime);

        if (isGrounded == true && verticalVelocity.y < 0f)
        {
            verticalVelocity.y = -1f;
        }
        else
        {
            //Apply gravity
            verticalVelocity.y += gravity * Time.deltaTime;
        }

        enemyController.Move(verticalVelocity * Time.deltaTime);
    }

    public void TakeDamage(int damage) 
    {
        if (!isDead)
        {
            health -= damage;
            if (health <= 0) 
            {
                isDead = true;
            }
        }
    }

    public void TakeDamageWithDeathtype(int damage, int deathType)
    {
        if (!isDead)
        {
            health -= damage;
            if (health <= 0)
            {
                isDead = true;
            }
        }
    }
}
