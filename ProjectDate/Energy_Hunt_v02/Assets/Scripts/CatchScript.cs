using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatchScript : MonoBehaviour
{
    [SerializeField] PlayerController playerController;

    [SerializeField] TakeBomb takeBomb;

    //クールタイム
    private float timer;
    //クールタイム判定
    private bool isTimer = false;

    void Start()
    {
        
    }

    void Update()
    {
        //投げた直後、自信でキャッチできないようにする
        if (isTimer)
        {
            timer += Time.deltaTime;

            if (timer > 0.2f)
            {
                isTimer = false;

                timer = 0;
            }
        }
    }

    public void TimerStart()
    {
        isTimer = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Bomb0" && !playerController.haveBomb[0] && !playerController.haveBomb[1] && !isTimer)
        {
            if (other.GetComponent<BombScript>().isThrow)
            {
                Debug.Log("キャッチ");
                takeBomb.CanTakeChange(0, true);
            }
        }

        if (other.gameObject.tag == "Bomb1" && !playerController.haveBomb[0] && !playerController.haveBomb[1] && !isTimer)
        {
            if (other.GetComponent<BombScript>().isThrow)
            {
                Debug.Log("キャッチ");
                takeBomb.CanTakeChange(1, true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Bomb0")
        {
            takeBomb.CanTakeChange(0, false);
        }

        if (other.gameObject.tag == "Bomb1")
        {
            takeBomb.CanTakeChange(1, false);
        }
    }
}
