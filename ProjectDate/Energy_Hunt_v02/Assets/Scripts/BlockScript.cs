using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockScript : MonoBehaviour
{
    private MeshRenderer material;

    [SerializeField]
    Material[] blockMat; 

    [SerializeField, Header("復活するまでの時間")]
    float setTime;

    private int blockHp = 2;

    private float timer;

    private bool isTimer = false;

    //ブロックが壊れているかの判定
    public bool isBroken = false;
    

    void Start()
    {
        material= gameObject.GetComponent<MeshRenderer>();
    }

    void Update()
    {
        //復活までのクールタイム
        if (isTimer)
        {
            timer += Time.deltaTime;

            if (timer > setTime)
            {
                timer = 0;

                blockHp = 2;

                //ブロックの表示切替
                material.material = blockMat[0];
                GetComponent<MeshRenderer>().enabled = true;
                GetComponent<BoxCollider>().isTrigger = false;

                isBroken = false;

                //タイマー切り替え
                isTimer = false;
            }
        }
    }

    
    public void SetBlock(bool blockBool)
    {
        blockHp--;
        if (blockHp == 1)
        {
            material.material = blockMat[1];
        }
       
        if (blockHp == 0)
        {
            //ブロックの表示切替
            GetComponent<MeshRenderer>().enabled = blockBool;
            GetComponent<BoxCollider>().isTrigger = !blockBool;

            isBroken = !blockBool;

            //タイマー切り替え
            isTimer = !blockBool;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag=="P1"|| other.gameObject.tag == "P2" || other.gameObject.tag == "P3" || other.gameObject.tag == "P4")
        {
            //Debug.Log("hoge");
            isTimer = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "P1" || other.tag == "P2" || other.tag == "P3" || other.tag == "P4")
        {
            isTimer = true;
        }
    }
}
