using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System;
[RequireComponent(typeof(PlayerInput))]

public class PlayerController : MonoBehaviour
{
    public TakeBomb takeBomb;

    [SerializeField, Header("EnergyTankManagerスクリプト")]
    EnergyTank energyTank;

    [SerializeField, Header("SavingTankスクリプト")]
    SavingTank savingTank;

    [SerializeField,Header("Bombスクリプト")] 
    BombScript[] bombScript;

    [SerializeField,Header("爆弾オブジェクト")] GameObject[] bomb;
    [SerializeField, Header("放電エフェクト")] GameObject[] effect; 
    [SerializeField,Header("ボムを取れる範囲")] GameObject passRange;

    [SerializeField, Header("ボムをキャッチできる範囲")] GameObject catchRange;

    [SerializeField, Header("背負っているタンクオブジェクト")]
    GameObject playerTank;

    [Header("プレイヤーのモデル")]
    [SerializeField] GameObject body;
    //[SerializeField] GameObject head;
    [SerializeField] GameObject hand;

    [SerializeField,Header("プレイヤーのコライダー")] 
    CapsuleCollider capsule;

    [SerializeField,Header("プレイヤーのRigidbody")] 
    Rigidbody playerRb;

    [SerializeField, Header("爆弾のRigidbody")] 
    Rigidbody[] bombRb;

    [SerializeField, Header("バリアオブジェクト")]
    GameObject barrierObj;

    //プレイヤーのスタート位置
    private Vector3 startPos;
    //プレイヤーのスタートの向き
    private Quaternion startAngle;

    //プレイヤーの番号
    public int controllerNum;

    [SerializeField,Header("プレイヤーの移動スピード")] 
    float speed;

    //プレイヤーの移動、投げるアクション
    public InputAction moveAction, throwAction;

    //投げた後、次にボタンを押せるまでの時間
    private float timer = 0;
    //入力クールタイム
    public float inputTimer;
    //入力クールタイムの判定
    public bool isInputTimer = false;

    //プレイヤーの向き
    private float angle;
    //スティックの角度を保存するためのfloat
    private float lastAngle;

    //爆弾を投げる力
    private float power = 1;

    //爆弾を持っている判定
    public bool[] haveBomb;

    //死亡判定
    public bool isDead = true;

    //落下判定
    private bool isFall = false;

    //点滅判定
    private bool isBlink = false;

    private Animator animator;

    private bool isBarrier = false;

    void Start()
    {
        //ActionMapを取得
        var input = GetComponent<PlayerInput>();
        //var input = PlayerInput.GetPlayerByIndex(controllerNum - 1);
        var actionMap = input.currentActionMap;

        moveAction = actionMap["Move"];
        throwAction = actionMap["PickUp"];


        haveBomb[0] = false;
        haveBomb[1] = false;


        bombRb[0] = bomb[0].GetComponent<Rigidbody>();
        bombRb[1] = bomb[1].GetComponent<Rigidbody>();


        //最初のポジションを設定
        startPos = transform.position;
        startAngle = transform.localRotation;

        animator = GetComponent<Animator>();

        //バリアスクリプトのバリア判定を取得
        isBarrier = barrierObj.GetComponent<BarrierScript>().isBarrier;

        //スタートした後に動けるようにする
        Invoke("GameStart", 3.5f);
    }

    private void GameStart()
    {
        isDead = false;
    }

    void Update()
    {
        if (!isDead && !isFall)
        {
            //プレイヤーの移動
            Vector2 move = moveAction.ReadValue<Vector2>();

            if (move.x < 0.1 && move.x > -0.1)
            {
                move.x = 0;
            }
            if (move.y < 0.1 && move.y > -0.1)
            {
                move.y = 0;
            }

            transform.position += new Vector3(move.x, 0, move.y) * speed * Time.deltaTime;         


            //プレイヤーの回転
            if (move.x > 0.1 || move.x < -0.1 || move.y > 0.1 || move.y < -0.1)
            {
                angle = Mathf.Atan2(move.x, move.y) * Mathf.Rad2Deg;

                if (angle < 0)
                {
                    angle += 360;
                }
                //スティックの角度を保存
                lastAngle = angle;

                transform.rotation = Quaternion.Euler(0, angle, 0);
            }
            else
            {
                //スティック入力が0の時保存していた角度を代入
                transform.rotation = Quaternion.Euler(0, lastAngle, 0);
            }
        }

        //爆弾を奪える範囲の追従
        passRange.transform.position = transform.position;
        passRange.transform.rotation = transform.rotation;

        //キャッチできる範囲の追従
        catchRange.transform.position = transform.position;
        catchRange.transform.rotation = transform.rotation;

        //バリアオブジェクトの追従
        barrierObj.transform.position = transform.position;


        //投げる動作の判定
        var throwAct = throwAction.triggered;
        /*
        if (throwAct)
        {
            SceneManager.LoadScene("TestScene");
        }
        */
        //爆弾を投げる
        if (throwAct && haveBomb[0] && !isInputTimer)
        {
            //すぐにキャッチできないようにする
            catchRange.GetComponent<CatchScript>().TimerStart();
            takeBomb.IsTimerTrue();

            //コアを投げられた状態にする
            bombScript[0].SetIsThrow(true);

            ThrowBomb(0);
            
        }
        if (throwAct && haveBomb[1] && !isInputTimer)
        {
            //すぐにキャッチできないようにする
            catchRange.GetComponent<CatchScript>().TimerStart();
            takeBomb.IsTimerTrue();

            //コアを投げられた状態にする
            bombScript[1].SetIsThrow(true);

            ThrowBomb(1);
        }


        //投げてすぐにキャッチできないようにするための入力クールタイム
        if (isInputTimer)
        {
            timer += Time.deltaTime;

            if (timer > 0.4)
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

    public void IsTimerTrue()
    {
        isInputTimer = true;
    }

    public void HaveBombChange(int i, bool a)
    {
        haveBomb[i] = a;
    }

    //腕を回転させる
    public void SetArm(int armAngle)
    {
        //Debug.Log("腕");
        hand.transform.localRotation = Quaternion.Euler(armAngle, 0, 0);
    }

    public void PlayerDead()
    {
        //誰かが勝利したとき、全員動けなくする
        isDead = true;
    }

    //爆弾を投げる関数
    private void ThrowBomb(int bombNum)
    {
        //animator.SetBool("isThrow", true);
        //Invoke("ThroeAnimFalse", 0.1f);
        

        //入力クールタイムスタート
        isInputTimer = true;

        //AddForceで投げるので、isKinematicをfalseする
        bombRb[bombNum].isKinematic = false;

        //コライダー復活
        bombScript[bombNum].ColliderEnabled(true);

        //爆弾を持っている判定をfalseにする
        energyTank.MissingCore();
        haveBomb[bombNum] = false;

        //腕を下げる
        SetArm(0);

        //爆弾を飛ばす
        bombRb[bombNum].AddForce(bomb[bombNum].transform.forward * 500 * power);
    }

    private void ThroeAnimFalse()
    {
        animator.SetBool("isThrow", false);
    }

    public IEnumerator WinnerAnim(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        animator.SetBool("isWin", true);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //ステージ外に落ちた時、haveBombをfalseにする
        if (collision.gameObject.tag == "Reset" && !isDead)
        {
            //マグマに沈ませるためにコライダーオフ
            capsule.enabled = false;

            if (haveBomb[0])
            {
                Debug.Log("reset");
                //ボムを持っている判定をfalseにする
                haveBomb[0] = false;

                //腕を下げる
                SetArm(0);

                //bombRb[0].isKinematic = false;

                StartCoroutine(bombScript[0].PosReset(0));
            }
            else if (haveBomb[1])
            {
                Debug.Log("reset");
                //ボムを持っている判定をfalseにする
                haveBomb[1] = false;

                //腕を下げる
                SetArm(0);

                //bombRb[1].isKinematic = false;

                StartCoroutine(bombScript[1].PosReset(0));
            }

            

            energyTank.DropEnergy(5);

            PlayerDead(2);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //ステージの穴に落ちたら動けなくなる
        if (other.gameObject.tag == "FallArea")
        {
            isFall = true;
        }




        //爆発に当たったら死亡
        if (other.gameObject.tag == "Explosion0" && !isDead && !barrierObj.GetComponent<BarrierScript>().isBarrier && !isBlink)
        {
            if (haveBomb[1])
            {
                //ボムを持っている判定をfalseにする
                haveBomb[1] = false;
                energyTank.MissingCore();

                //腕を下げる
                SetArm(0);

                bombScript[1].Explosion();
            }

            //energyTank.DropEnergy(5);

            PlayerDead(1);
        }
        if (other.gameObject.tag == "Explosion1" && !isDead && !barrierObj.GetComponent<BarrierScript>().isBarrier && !isBlink) 
        {
            if (haveBomb[0])
            {
                //ボムを持っている判定をfalseにする
                haveBomb[0] = false;
                energyTank.MissingCore();

                //腕を下げる
                SetArm(0);

                bombScript[0].Explosion();

            }

            

            PlayerDead(1);
        }
    }


    
    private void PlayerDead(int lostEnergy)
    {
        //バリアを消す
        barrierObj.GetComponent<BarrierScript>().BarrierCoolTime();

        if (lostEnergy == 1)
        {
            animator.SetBool("isBreke", true);
        }

        //プレイヤー死亡
        isDead = true;

        haveBomb[0] = false;
        haveBomb[1] = false;

        //腕を下げる
        SetArm(0);

        //タンクを非表示
        playerTank.SetActive(false);

        //プレイヤーのモデルを非表示
        //body.SetActive(false);
        //head.SetActive(false);
        //hand.SetActive(false);

        //落ちないようにisKinematicをfalse
        //playerRb.isKinematic = true;

        //墓のオブジェクトが当たらないようにコライダーをオフ
        //capsule.enabled = false;

        //爆弾を取れなくする
        takeBomb.CanTakeChange(0, false);
        takeBomb.CanTakeChange(1, false);

        //StartCoroutine(craneScript.RotateToTarget());
        StartCoroutine(posReset(lostEnergy));
    }

    private IEnumerator posReset(int lostEnergy)
    {
        

        yield return new WaitForSeconds(2.0f);

        animator.SetBool("isBreke", false);

        //プレイヤーを点滅させる
        gameObject.GetComponent<CharacterBlinkingScript>().BlinkStart(true);
        isBlink = true;

        //エネルギーリセット
        effect[0].GetComponent<ExplosionScript>().SetIsSelf(controllerNum);
        effect[1].GetComponent<ExplosionScript>().SetIsSelf(controllerNum);
        //energyTank.DropEnergy(5);

        //スタート位置にリスポーン
        transform.position = startPos;

        //スタートの向きに向く
        transform.localRotation = startAngle;

        //プレイヤーのモデルを表示
        body.SetActive(true);
        hand.SetActive(true);

        //タンクを表示
        playerTank.SetActive(true);

        //コライダー復活
        capsule.enabled = true;

        playerRb.isKinematic = false;

        //バリア復活
        barrierObj.GetComponent<BarrierScript>().CoolTimeReset();

        //プレイヤー復活
        isDead = false;
        isFall = false;

        yield return new WaitForSeconds(1.0f);

        //点滅終了
        gameObject.GetComponent<CharacterBlinkingScript>().BlinkStart(false);
        isBlink = false;

        //プレイヤーのモデルを表示
        body.SetActive(true);
        hand.SetActive(true);

        //タンクを表示
        playerTank.SetActive(true);


        //落下時タンクのエネルギー2,爆発に当たったとき1消費
        //savingTank.UseEnergy(lostEnergy);
    }

    //TankManagerから満タン状態のタンクの個数を取得
    public void FullTank(int amount)
    {
        //満タン状態のタンク数に応じてプレイヤーを強化
        switch (amount)
        {
            case 0:
                speed = 4;
                power = 1;
                ExploPower(1);
                break;

            case 1:
                speed = 5.0f;
                ExploPower(1);
                break;

            case 2:
                power = 1.2f;
                ExploPower(1);
                break;

            case 3:
                ExploPower(2.0f);
                break;
        }
    }

    //爆弾の爆発範囲を変更
    public void ExploPower(float amount)
    {
        if (haveBomb[0])
        {
            bombScript[0].ExplosionRange(amount);
        }
        else if (haveBomb[1])
        {
            bombScript[1].ExplosionRange(amount);
        }
    }

    public void CanTakeTrue(int i)
    {
        takeBomb.CanTakeChange(i, true);
    }

    public void HitPlayerChange(GameObject hitplayer)
    {
        takeBomb.SetHitPlayer(hitplayer);
    }
}
