using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BomeEffectInstansScript : MonoBehaviour
{
    public GameObject bomeEffect;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Instantiate(bomeEffect, new Vector3(0, 0, 0), Quaternion.identity);
        }
    }
}
