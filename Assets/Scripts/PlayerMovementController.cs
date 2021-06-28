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
    public float smgReloadTime=1f;
    public int bulletsPerBurst = 6;
    float burstInterval;
    bool isBurstFiring;

    public int rifleMaxAmmo;
    public int rifleCurrentAmmo;
    public int rifleMagCapacity;
    public int rifleCurrentMag;
    public float rifleFiringRate;
    public float rifleReloadTime=1f;

    bool isFiring=false;
    bool isStabbing=false;
    bool isReloading=false;

    public float stabbingInterval;
    float timeTillNextAction;

    void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
        isGrounded = false;
        transform.position = spawnTransform.position;
        currentWeapon = CurrentWeapon.Knife;
        
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, 5f, groundLayer);

        float x = joystick.Horizontal;
        float z = joystick.Vertical;

        

        //Only change the movement Vector if grounded
        if (isGrounded)
        {

            movementVector = new Vector3(x,0f, z);
            movementVector = movementVector.normalized;

            // rotates the player according to joystick input

            if (Mathf.Abs(z) >= .1f || Mathf.Abs(x) > .1f)
            {
                transform.forward = movementVector;

            }

           

            if (movementVector.magnitude > 1f)
            {
                movementVector = movementVector.normalized;

            }
            //stab, fire interval
            if (isFiring || isStabbing|| isReloading)
            {
                movementSpeed = 0f;
                anim.SetFloat("Speed", 0f);
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
                    if (currentWeapon == CurrentWeapon.SMG)
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
                    else if(currentWeapon== CurrentWeapon.Rifle)
                    {
                        if (timeTillNextAction >= 0)
                        {
                            timeTillNextAction -= Time.deltaTime;
                        }
                        else
                        {
                            isFiring = false;
                            if (rifleCurrentMag <= 0) ReloadRifle();
                        }
                    }
                }
                else if (isReloading) 
                {
                    if (timeTillNextAction >= 0)
                    {
                        timeTillNextAction -= Time.deltaTime;
                    }
                    else
                    {
                        isReloading = false;
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
        if (!isStabbing && !isFiring && !isReloading && currentWeapon==CurrentWeapon.Knife )
        {
            timeTillNextAction = stabbingInterval+.3f;
            isStabbing = true;
            anim.SetTrigger("Stab");


            RaycastHit hit;
            if (Physics.SphereCast(stabOrigin.position, stabRadius, stabOrigin.forward, out hit, stabMaxDistance, enemyLayer))
            {
                hit.collider.GetComponent<EnemyAI>().TakeDamage(knifeDamage);
            }
        }
    }
    public void ShootSMG()
    {
        if (!isFiring && !isStabbing && !isReloading && currentWeapon==CurrentWeapon.SMG)
        {
            if (smgCurrentMag > 0)
            {
                isFiring = true;
                timeTillNextAction =bulletsPerBurst*(1/smgFiringRate)+.7f ;
            }
            else
            {
                ReloadSMG();
            }
        }
    }
    public void ShootRifle()
    {
        if (!isFiring && !isStabbing && !isReloading && currentWeapon==CurrentWeapon.Rifle)
        {
            if (rifleCurrentMag > 0)
            {
                isFiring = true;
                timeTillNextAction = 1/rifleFiringRate;
                anim.SetTrigger("RifleShoot");
                rifleCurrentMag -= 1;
            }
            else
            {
                ReloadRifle();
            }
        }
    }

    public void BurstFire() 
    {
        if (!isBurstFiring && !isReloading)
        {
            if (smgCurrentMag > 0)
            {
                isBurstFiring = true;
                burstInterval = (1 / smgFiringRate);
                anim.SetTrigger("SMGShoot");
                smgCurrentMag -= 1;
            }
            else 
            {
                ReloadSMG();
            }
        }
    
    }

    public void ReloadSMG()
    {
        if (smgCurrentMag < smgMagCapacity)
        {
            if (smgCurrentAmmo > 0)
            {
                if (!isReloading)
                {
                    isReloading = true;
                    timeTillNextAction = smgReloadTime;
                    anim.SetTrigger("ReloadSMG");
                    if (smgCurrentAmmo > smgMagCapacity)
                    {
                        smgCurrentAmmo -= (smgMagCapacity - smgCurrentMag);
                        smgCurrentMag += (smgMagCapacity - smgCurrentMag);
                    }
                    else
                    {
                        smgCurrentMag = smgCurrentAmmo;
                        smgCurrentAmmo = 0;
                    }
                }

            }
            else
            {
                //dry fire or switch weapon
            }
        }
       
    }

    public void ReloadRifle()
    {
        if (rifleCurrentMag < rifleMagCapacity)
        {
            if (rifleCurrentAmmo > 0)
            {
                if (!isReloading)
                {
                    isReloading = true;
                    timeTillNextAction = rifleReloadTime;
                    anim.SetTrigger("ReloadRifle");
                    if (rifleCurrentAmmo > rifleMagCapacity)
                    {
                        rifleCurrentAmmo -= (rifleMagCapacity - rifleCurrentMag);
                        rifleCurrentMag += (rifleMagCapacity - rifleCurrentMag);
                    }
                    else
                    {
                        rifleCurrentMag = rifleCurrentAmmo;
                        rifleCurrentAmmo = 0;
                    }
                }

            }
            else
            {
                //dry fire or switch weapon
            }
        }

    }
    public void SelectMachineGun() 
    {
        if (currentWeapon != CurrentWeapon.SMG)
        {
            isReloading = true; // only for creating a pause
            timeTillNextAction = .8f;
            currentWeapon = CurrentWeapon.SMG;

            smgHolster.SetActive(false);
            knife.SetActive(false);
            rifle.SetActive(false);
            anim.SetTrigger("DrawSMG");
            smg.SetActive(true);
            knifeHolster.SetActive(true);
            rifleHolster.SetActive(true);
        }
    }
    public void SelectRifle() 
    {
        if (currentWeapon != CurrentWeapon.Rifle)
        {
            isReloading = true; // only for creating a pause
            timeTillNextAction = .8f;
            currentWeapon = CurrentWeapon.Rifle;

            knifeHolster.SetActive(true);
            smg.SetActive(false);
            rifleHolster.SetActive(false);
            anim.SetTrigger("DrawRifle");
            rifle.SetActive(true);
            knife.SetActive(false);
            smgHolster.SetActive(true);
        }
        
    }
    public void SelectKnife() 
    {
        if (currentWeapon != CurrentWeapon.Knife)
        {
            isReloading = true; // only for creating a pause
            timeTillNextAction = .8f;
            currentWeapon = CurrentWeapon.Knife;

            knifeHolster.SetActive(false);
            smg.SetActive(false);
            rifle.SetActive(false);
            anim.SetTrigger("DrawKnife");
            knife.SetActive(true);
            smgHolster.SetActive(true);
            rifleHolster.SetActive(true);
        }
    }

   

   
}
