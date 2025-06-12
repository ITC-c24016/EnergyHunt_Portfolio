using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class BombEffectScript : MonoBehaviour
{
    public float maxTime = 2f;
    public float maxScale = 2f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(BomeEffect());
    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator BomeEffect()
    {
        float timer = 0f;
        Vector3 originalScale = transform.localScale;
        Vector3 tagetScale = transform.localScale * maxScale;

        while(timer < maxTime)
        {
            float ScaleChangeTime = timer / maxTime;
            timer += Time.deltaTime;
            transform.localScale = Vector3.Lerp(originalScale, tagetScale, ScaleChangeTime);
            yield return null;
        }
        Destroy(gameObject);

    }
}
