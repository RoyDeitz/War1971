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
  
    Vector3 movementVector;

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

    //health,attack, and damage
    public LayerMask enemyLayer;
    public Transform stabOrigin;
    public float stabRadius;
    public float stabMaxDistance;

    public int knifeDamage;
    public int pistolDamage;
    public int smgDamage;
    public int rifleDamage;
    public int assaultRifleDamage;

    //weapon
    public GameObject rifleHolster;
    public GameObject smgHolster;
    public GameObject knifeHolster;

    public GameObject knife;
    public GameObject smg;
    public GameObject rifle;

    public enum CurrentWeapon
    {
        Knife,
        SMG,
        Rifle
    }
    public CurrentWeapon currentWeapon;

    public int smgMaxAmmo;
    public int smgCurrentAmmo;
    public int smgMagCapacity;
    public int smgCurrentMag;
    public float smgFiringRate;
    float burstInterval;
    bool isBurstFiring;

    public int rifleMaxAmmo;
    public int rifleCurrentAmmo;
    public int rifleMagCapacity;
    public int rifleCurrentMag;
    public float rifleFiringRate;

    bool isFiring;
    bool isStabbing;

    public float stabbingInterval;
    float timeTillNextAction;

    void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
        isGrounded = false;
        transform.position = spawnTransform.position;
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, 5f, groundLayer);

        float x = joystick.Horizontal;
        float z = joystick.Vertical;

        

        //Only change the movement Vector if grounded
        if (isGrounded)
        {

            movementVector = new Vector3(x, 0f, z);
            movementVector = movementVector.normalized;

            if (Mathf.Abs(z) >= .1f || Mathf.Abs(x) > .1f)
            {
                transform.forward = movementVector;

            }

           

            if (movementVector.magnitude > 1f)
            {
                movementVector = movementVector.normalized;

            }
            //stab, fire interval
            if (isFiring || isStabbing)
            {
                movementSpeed = 0f;
                if (isStabbing)
                {
                    if (timeTillNextAction >= 0)
                    {
                        timeTillNextAction -= Time.deltaTime;
                    }
                    else
                    {
                        isStabbing = false;
                    }
                }
                else if (isFiring) 
                {
                    if (timeTillNextAction >= 0)
                    {
                        timeTillNextAction -= Time.deltaTime;

                        BurstFire();

                        if (burstInterval >= 0)
                        {
                            burstInterval -= Time.deltaTime;
                        }
                        else
                        {
                            isBurstFiring = false;
                        }
                       
                    }
                    else
                    {
                        isFiring = false;
                    }

                }
            }
            else
            {
                if (Mathf.Abs(x) > .7f || Mathf.Abs(z) > .7f) movementSpeed = runningSpeed;
                else movementSpeed = walkingSpeed;

                if (Mathf.Abs(x) > Mathf.Abs(z)) anim.SetFloat("Speed", Mathf.Abs(x));
                else anim.SetFloat("Speed", Mathf.Abs(z));
            }

        }

       

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

    public void Stab()
    {
        if (!isStabbing && !isFiring)
        {
            timeTillNextAction = stabbingInterval;
            isStabbing = true;
            anim.SetTrigger("Stab");


            RaycastHit hit;
            if (Physics.SphereCast(stabOrigin.position, stabRadius, stabOrigin.forward, out hit, stabMaxDistance, enemyLayer))
            {
                hit.collider.GetComponent<EnemyAI>().TakeDamage(knifeDamage);
            }
        }
    }
    public void Shoot()
    {
        if (!isFiring && !isStabbing)
        {
            isFiring = true;
            timeTillNextAction= smgFiringRate/10;

        }
    }

    public void BurstFire() 
    {
        if (!isBurstFiring) 
        {
            isBurstFiring = true;
            burstInterval = (1 / smgFiringRate);
            anim.SetTrigger("SMGShoot");
        }
    
    }
    public void SelectMachineGun() 
    {
        smgHolster.SetActive(false);
        knife.SetActive(false);
        rifle.SetActive(false);
        anim.SetTrigger("DrawSMG");
        smg.SetActive(true);
        knifeHolster.SetActive(true);
        rifleHolster.SetActive(true);
    }
    public void SelectRifle() 
    {
    
    }
    public void SelectKnife() 
    {
        knifeHolster.SetActive(false);
        smg.SetActive(false);
        rifle.SetActive(false);
        anim.SetTrigger("DrawKnife");
        knife.SetActive(true);
        smgHolster.SetActive(true);
        rifleHolster.SetActive(true);
    }

   

   
}
