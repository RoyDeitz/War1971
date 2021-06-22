using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public int health;
    bool isDead;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
