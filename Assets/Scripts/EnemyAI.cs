using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    
    //Gravity/Jumping Velocity
    Vector3 verticalVelocity;
    public float jumpHeight = 2f;
    public float gravity = -9.81f; //9.81 m/s�

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
    public float alertWalkingSpeed;
    Vector3 movementVector;

    //health and death
    public int health;
    bool isDead=false;
    int deathType;

    public enum AlertLevel 
    {
    Guard,
    Suspicious,
    Alert,
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
    public Weapon weapon;

    CharacterController enemyController;

    // Detection attributes
    public bool isPlayerFound=false;
    public bool isHeaing;
    public bool isObjFound;

    public Transform eyeTransform;
    public LineRenderer lr;
    public Material safeObjMaterial;
    public Material dangerObjMaterial;
    public Vector3 objPosition;



    //weapon
    public int smgMaxAmmo;
    public int smgCurrentAmmo;
    public int smgMagCapacity;
    public int smgCurrentMag;
    public float smgFiringRate;
    public float smgReloadTime = 1f;
    public int bulletsPerBurst = 6;
    float burstInterval;
    bool isBurstFiring;

    public int rifleMaxAmmo;
    public int rifleCurrentAmmo;
    public int rifleMagCapacity;
    public int rifleCurrentMag;
    public float rifleFiringRate;
    public float rifleReloadTime = 1f;

    public GameObject SniperScope;
    public Transform mainCamera;
    public float scopeDistance=.08f;
    public float scopeAdjustmentHeight = 30f;

    bool isFiring = false;
    bool isReloading = false;
    float timeTillNextAction;

    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.material = safeObjMaterial;
        enemyController = GetComponent<CharacterController>();
        SniperScope.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, 5f, groundLayer);
        
        if (alertLevel == AlertLevel.Guard) anim.SetInteger("AlertLevel",0);
        else if (alertLevel == AlertLevel.Suspicious) anim.SetInteger("AlertLevel", 1);
        else if (alertLevel == AlertLevel.Combat) anim.SetInteger("AlertLevel", 2);
        
        if (isObjFound)
        {
            lr.positionCount = 2;
            lr.startWidth = 2f;
            lr.endWidth = 10;
            lr.SetPosition(0, eyeTransform.position);
            //lr.SetPosition(1,objPosition);

            if (isPlayerFound)
            {
                alertLevel = AlertLevel.Combat;
                lr.material = dangerObjMaterial;
                GameObject target = new GameObject();
                target.transform.position = new Vector3(objPosition.x, transform.position.y, objPosition.z);
                transform.LookAt(target.transform);
                if (enemyType == EnemyType.Sniper)
                {
                    Vector3 scopeTarget = new Vector3(target.transform.position.x, target.transform.position.y + scopeAdjustmentHeight, target.transform.position.z);
                    Vector3 direction = mainCamera.position -scopeTarget;
                    SniperScope.SetActive(true);
                    //SniperScope.transform.position = direction *(Vector3.Distance(mainCamera.position,target.transform.position)/ 40);
                    SniperScope.transform.position =scopeTarget+ direction * scopeDistance;
                    SniperScope.transform.rotation = mainCamera.rotation;
                    SniperScope.SetActive(true);
                }
                lr.SetPosition(1, new Vector3(objPosition.x,objPosition.y+ 35,objPosition.z));
                Destroy(target);

                //shoot;
                //ShootRifle();
                InvokeRepeating("ShootRifle",rifleFiringRate,rifleFiringRate+1f);
            }
            else
            {
                lr.SetPosition(1, objPosition);
                //alertLevel = AlertLevel.Suspicious;
                lr.material = safeObjMaterial;
                SniperScope.SetActive(false);
                CancelInvoke();
               
            }

        }
        else 
        {
            SniperScope.SetActive(false);
            CancelInvoke();
            lr.positionCount = 0;
            lr.material = safeObjMaterial;
        }
        //Shooting
        if (isFiring || isReloading)
        {
            movementSpeed = 0f;
            anim.SetFloat("Speed", 0f);

            if (isFiring)
            {
                if (weapon == Weapon.Rifle)
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
    public void ShootRifle()
    {
        if (!isFiring  && !isReloading && weapon==Weapon.Rifle)
        {
            if (rifleCurrentMag > 0)
            {
                isFiring = true;
                timeTillNextAction = 1 / rifleFiringRate;
                anim.SetTrigger("ShootRifle");
                rifleCurrentMag -= 1;
            }
            else
            {
                ReloadRifle();
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
}
