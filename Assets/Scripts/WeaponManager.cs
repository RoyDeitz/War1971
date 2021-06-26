using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public struct Gun
    {
        public bool available;
        public string name;
        public int firingRate;
        public int magCapacity;
        public int currentMag;
        public int currentAmmo;
        public int maxAmmo;
    }

    public Gun[] guns;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
