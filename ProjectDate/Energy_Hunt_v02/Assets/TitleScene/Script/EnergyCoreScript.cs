using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyCoreScript : MonoBehaviour
{
    [SerializeField] GameObject bombEffect;

    //[SerializeField] float timer = 0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor" || collision.gameObject.tag == "Player2")
        {
            bombEffect.SetActive(true);
            StartCoroutine(DeactivateEffectAfterDelay(1f)); // 0.5•bŒã‚É–³Œø‰»
        }
    }
    private IEnumerator DeactivateEffectAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Žw’è‚µ‚½ŽžŠÔ‘Ò‹@
        bombEffect.SetActive(false);
    }
}

