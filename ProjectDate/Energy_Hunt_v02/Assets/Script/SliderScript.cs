using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderScript : MonoBehaviour
{
    [SerializeField] bool habBomb;
    [SerializeField] bool attackChack;
    [SerializeField] Slider sliders;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (habBomb)
        {
            sliders.value += 2 * Time.deltaTime;
        }
        else if (sliders.value > 0)
        {
            sliders.value -= Time.deltaTime;
        }
        if(sliders.value == 100 && attackChack)
        {
            sliders.value = 0;
            attackChack = false;
        }
    }
}
