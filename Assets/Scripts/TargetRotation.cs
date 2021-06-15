using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetRotation : MonoBehaviour
{

    public Transform arCameraTransform;
  
    void Update()
    {
        transform.rotation = Quaternion.Euler(0f,arCameraTransform.rotation.y,0f);
    }
}
