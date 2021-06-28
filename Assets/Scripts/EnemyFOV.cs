using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFOV : MonoBehaviour
{

    public float detectionTime;
    public EnemyAI enemyAI;

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            if (enemyAI.alertLevel == EnemyAI.AlertLevel.Guard)
            {
                enemyAI.isObjFound = true;
                enemyAI.objPosition = other.transform.position;
                StartCoroutine(Recognise(detectionTime, other));

            }
            else
            {
                //shoot
            }
        }
        else 
        {
        
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player") 
        {
            //set memory time
            // search routine
            enemyAI.isObjFound = false;
        }
    }

    IEnumerator Recognise(float time, Collider other)
    {
        yield return new WaitForSeconds(time);

    }
}
