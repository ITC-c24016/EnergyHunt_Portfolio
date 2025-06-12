using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.InputSystem;
//[RequireComponent(typeof(PlayerInput))]

public class TakeBomb_copy : MonoBehaviour
{
    [SerializeField, Header("EnergyTankスクリプト")] 
    EnergyTank_copy[] energyTank;

    [SerializeField, Header("PlayerControllerスクリプト")] 
    PlayerController_copy[] playerController;
    
    [SerializeField, Header("爆弾のスクリプト")] 
    BombScript[] bombScript;

    [SerializeField, Header("爆発のスクリプト")] 
    ExplosionScript_copy[] explosionScript;

    [SerializeField, Header("爆弾オブジェクト")] 
    GameObject[] bomb;

    [SerializeField, Header("プレイヤーオブジェクト")]
    GameObject player;

    //爆弾を取れる範囲にいる爆弾を持っているプレイヤー
    [SerializeField] private GameObject hitPlayer;
    //爆弾を取れる範囲にある爆弾
    private GameObject hitBomb;

    //プレイヤーの番号
    public int controllerNum;

    //爆弾を取れるかの判定
    public bool[] canTake;

    //爆弾を取るアクション
    private InputAction pickUpAction;

    [SerializeField, Header("プレイヤーのPlayerInput")] 
    PlayerInput input;

    //投げた後、次にボタンを押せるまでの時間
    private float timer = 0;
    //入力クールタイム
    //float inputTimer = 0.2f;
    //入力クールタイムの判定
    private bool isInputTimer = false;

    void Start()
    {
        //プレイヤーのアクションマップを取得
        var actionMap = input.currentActionMap;

        pickUpAction = actionMap["PickUp"];


        canTake[0] = false;
        canTake[1] = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Bomb0" && !playerController[controllerNum - 1].haveBomb[0] && !playerController[controllerNum - 1].haveBomb[1])
        {
            //Debug.Log("canTake");
            canTake[0] = true;

            hitBomb = other.gameObject;
        }
        if (other.gameObject.tag == "Bomb1" && !playerController[controllerNum - 1].haveBomb[0] && !playerController[controllerNum - 1].haveBomb[1])
        {
            //Debug.Log("canTake");
            canTake[1] = true;

            hitBomb = other.gameObject;
        }

        if (!playerController[controllerNum - 1].haveBomb[0] && !playerController[controllerNum - 1].haveBomb[1]
            && (other.gameObject.tag == "P1" || other.gameObject.tag == "P2" || other.gameObject.tag == "P3" || other.gameObject.tag == "P4"))
        {
            //当たった相手が自分ではない場合
            if (other.gameObject.tag != $"P{controllerNum}")
            {
                //当たった相手が爆弾を持っている場合
                if (other.gameObject.GetComponent<PlayerController_copy>().haveBomb[0])
                {
                    hitPlayer = other.gameObject;
                    canTake[0] = true;
                }
                else if (other.gameObject.GetComponent<PlayerController_copy>().haveBomb[1])
                {
                    hitPlayer = other.gameObject;
                    canTake[1] = true;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Bomb0")
        {            
            canTake[0] = false;
        }
        if (other.gameObject.tag == "Bomb1")
        {
            canTake[1] = false;
        }

        if(!playerController[controllerNum - 1].haveBomb[0] && !playerController[controllerNum - 1].haveBomb[1]
            && (other.gameObject.tag == "P1" || other.gameObject.tag == "P2" || other.gameObject.tag == "P3" || other.gameObject.tag == "P4"))
        {
            if(other.gameObject.tag != $"P{controllerNum - 1}")
            {
                //Exitした相手がボムを持っていたら、canTakeをfalseにする
                if (other.gameObject.GetComponent<PlayerController_copy>().haveBomb[0])
                {
                    hitPlayer = null;

                    canTake[0] = false;
                }
                else if (other.gameObject.GetComponent<PlayerController_copy>().haveBomb[1])
                {
                    hitPlayer = null;

                    canTake[1] = false;
                }
            }
        }
    }

    void Update()
    {
        //爆弾を持っている場合、プレイヤーに追従
        if (playerController[controllerNum - 1].haveBomb[0])
        {
            bomb[0].transform.position = new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z);
            bomb[0].transform.rotation = transform.rotation;
        }
        else if (playerController[controllerNum - 1].haveBomb[1])
        {
            bomb[1].transform.position = new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z);
            bomb[1].transform.rotation = transform.rotation;
        }


        //canTakeが2つtrueのとき、片方だけtrueにする
        if (canTake[0] && canTake[1])
        {
            canTake[0] = false;
        }


        //取るボタンを押しているかの判定
        var pickUp = pickUpAction.triggered;

        //爆弾を奪う
        if (canTake[0] && pickUp && !playerController[controllerNum - 1].isDead && !bombScript[0].isExplosion)
        {
            //isInputTimer = true;

            TakeBombFunction(0);
        }
        if (canTake[1] && pickUp && !playerController[controllerNum - 1].isDead && !bombScript[1].isExplosion)
        {
            //isInputTimer = true;

            TakeBombFunction(1);
        }

        if (isInputTimer)
        {
            timer += Time.deltaTime;

            if (timer > 0.2)
            {
                isInputTimer = false;

                timer = 0;
            }
        }
        else
        {
            timer = 0;
        }
    }

    //爆弾を奪う関数
    private void TakeBombFunction(int bombNum)
    {
        Debug.Log("取った");      
        //爆弾を取れないようにする
        CanTakeChange(0, false);
        CanTakeChange(1, false);

        //爆弾を持っている判定をtrueにする
        playerController[controllerNum - 1].HaveBombChange(bombNum,true);
        energyTank[controllerNum - 1].HaveCore();

        //爆弾を拾ったとき
        if (hitBomb != null)
        {
            //プレイヤーが爆弾を持っている時はほかのオブジェクトに当たらないようにするために
            //isKinematicをtrueにする
            bombScript[bombNum].IsKinematicTrue();

            //爆弾のコライダーを消す
            bombScript[bombNum].ColliderEnabled(false);

            //爆発のタイマーを開始する
            bombScript[bombNum].IsTimerTrue();

            //爆弾に持ち主を登録
            explosionScript[bombNum].Owner(controllerNum);
        }

        //自分以外のプレイヤーの爆弾を持っている判定をfalseにする
        for (int i = 0; i < 4; i++)
        {
            if (i != controllerNum - 1)
            {
                if (playerController[i].haveBomb[bombNum])
                {
                    energyTank[i].MissingCore();
                }
                playerController[i].HaveBombChange(bombNum, false);
            }         
        }
        /*
        //爆弾を奪った相手の
        if (hitPlayer != null)
        {
            Debug.Log(hitPlayer);
            //爆弾を持っている判定をfalseにする
            hitPlayer.gameObject.GetComponent<PlayerController>().HaveBombChange(bombNum, false);
            hitPlayer.gameObject.GetComponent<EnergyTankManager>().MissingCore();

            //爆弾を取れる判定をtrueにする
            //hitPlayer.gameObject.GetComponent<PlayerController>().CanTakeTrue(bombNum);

            //相手視点の爆弾を奪った相手を自分にする
            //hitPlayer.gameObject.GetComponent<PlayerController>().HitPlayerChange(player);
        }
        */
        

    }

    //爆弾を取れる判定を変更
    public void CanTakeChange(int i, bool canTakeChange)
    {
        canTake[i] = canTakeChange;
    }

    public void SetHitPlayer(GameObject gameObject)
    {
        hitPlayer = gameObject;
    }
}
