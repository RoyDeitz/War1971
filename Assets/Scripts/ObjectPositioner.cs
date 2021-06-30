using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPositioner : MonoBehaviour
{
    public Transform cubeSpawner;
    public GameObject cube;

    public void ResetPosition()
    {
        cube.transform.position = cubeSpawner.position;
    }
}
