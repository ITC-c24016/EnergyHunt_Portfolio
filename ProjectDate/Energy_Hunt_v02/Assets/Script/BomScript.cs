using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BomScript : MonoBehaviour
{
    private Rigidbody rb;
    public float power = 10f;
    public GameObject destroyBlock;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 force = new Vector3(0.0f, 0.0f, power);
            rb.AddForce(force);
            rb.useGravity = true;
        }

        if(transform.position.y < -10.0f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Floor")
        {
            destroyBlock.SetActive(true);
        }
    }
}

