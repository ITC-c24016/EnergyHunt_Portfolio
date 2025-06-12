using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BarrierScript : MonoBehaviour
{
    [SerializeField]
    PlayerController playerController;

    [SerializeField, Header("バリアエフェクト")]
    GameObject barrierEffect;

    [SerializeField, Header("バリア効果時間")]
    float barrierTime;
    //バリアのタイマー
    private float barrierTimer;
    //バリア中かの判定
    public bool isBarrier = false;

    [SerializeField, Header("クールタイム")]
    float coolTime;
    //クールタイムのタイマー
    private float coolTimer;
    //クールタイム中かの判定
    private bool isCoolTime = false;


    private bool canBarrier = true;

    [SerializeField] GameObject player;

    private InputAction barrierAction;


    void Start()
    {
        var input = player.GetComponent<PlayerInput>();
        var actionMap = input.currentActionMap;
        barrierAction = actionMap["Barrier"];
    }

    void Update()
    {
        var barrierAct = barrierAction.triggered;

        if (barrierAct && canBarrier && !playerController.haveBomb[0] && !playerController.haveBomb[1] && !playerController.isDead)
        {
            isBarrier = true;
            canBarrier = false;

            barrierEffect.SetActive(true);
            GetComponent<SphereCollider>().enabled = true;
        }

        //バリアの効果時間
        if (isBarrier)
        {
            barrierTimer += Time.deltaTime;

            if (barrierTimer > barrierTime)
            {
                barrierTimer = 0;

                GetComponent<SphereCollider>().enabled = false;
                barrierEffect.SetActive(false);

                isBarrier = false;

                isCoolTime = true;
            }
        }

        //次に使えるまでのクールタイム
        if (isCoolTime)
        {
            coolTimer += Time.deltaTime;

            if (coolTimer > coolTime)
            {
                coolTimer = 0;

                isCoolTime = false;
                canBarrier = true;
            }
        }
    }

    public void BarrierCoolTime()
    {
        GetComponent<SphereCollider>().enabled = false;
        barrierEffect.SetActive(false);

        isBarrier = false;
        barrierTimer = 0;
        canBarrier = false;
        
        isCoolTime = true;
    }

    public void CoolTimeReset()
    {
        barrierEffect.SetActive(false);

        isBarrier = false;
        barrierTimer = 0;
        canBarrier = true;

        isCoolTime = false;
    }
}
