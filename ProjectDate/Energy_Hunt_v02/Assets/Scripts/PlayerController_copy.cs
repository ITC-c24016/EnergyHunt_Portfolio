using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
[RequireComponent(typeof(PlayerInput))]

public class PlayerController_copy : MonoBehaviour
{
    [SerializeField,Header("TakeBombスクリプト")] 
    TakeBomb_copy takeBomb;

    [SerializeField, Header("EnergyTankスクリプト")] 
    EnergyTank_copy energyTank;

    [SerializeField, Header("SavingTankスクリプト")]
    SavingTank savingTank;

    [SerializeField,Header("Bombスクリプト")] 
    BombScript[] bombScript;

    [SerializeField,Header("Craneスクリプト")] 
    CraneScript craneScript;

    [SerializeField,Header("爆弾オブジェクト")] GameObject[] bomb;
    [SerializeField,Header("ボムを取れる範囲")] GameObject passRange;

    [Header("プレイヤーのモデル")]
    [SerializeField] GameObject body;
    [SerializeField] GameObject head;
    [SerializeField] GameObject hand;

    [SerializeField,Header("プレイヤーのコライダー")] 
    CapsuleCollider capsule;

    [SerializeField,Header("プレイヤーのRigidbody")] 
    Rigidbody playerRb;

    [SerializeField, Header("爆弾のRigidbody")] 
    Rigidbody[] bombRb;

    //プレイヤーのスタート位置
    private Vector3 startPos;

    //プレイヤーの番号
    public int controllerNum;

    [SerializeField,Header("プレイヤーの移動スピード")] 
    float speed;

    //プレイヤーの移動、投げるアクション
    private InputAction moveAction, throwAction;

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
    public bool isDead = false;

    void Start()
    {
        //ActionMapを取得
        var input = GetComponent<PlayerInput>();
        var actionMap = input.currentActionMap;

        moveAction = actionMap["Move"];
        throwAction = actionMap["Throw"];


        haveBomb[0] = false;
        haveBomb[1] = false;


        bombRb[0] = bomb[0].GetComponent<Rigidbody>();
        bombRb[1] = bomb[1].GetComponent<Rigidbody>();


        //プレイヤー番号に応じて最初のポジションを設定
        switch (controllerNum)
        {
            case 1:
                startPos = new Vector3(5, 1.5f, -5);
                transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case 2:
                startPos = new Vector3(-5, 1.5f, -5);
                transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case 3:
                startPos = new Vector3(-5, 1.5f, 5);
                transform.rotation = Quaternion.Euler(0, 180, 0);
                break;
            case 4:
                startPos = new Vector3(5, 1.5f, 5);
                transform.rotation = Quaternion.Euler(0, 180, 0);
                break;
        }
    }

    void Update()
    {
        if (!isDead)
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

        //投げる動作の判定
        var throwAct = throwAction.triggered;

        //爆弾を投げる
        if (throwAct && haveBomb[0])
        {
            ThrowBomb(0);
            
        }
        if (throwAct && haveBomb[1])
        {
            ThrowBomb(1);
        }


        //投げてすぐにキャッチできないようにするための入力クールタイム
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

    public void HaveBombChange(int i, bool a)
    {
        haveBomb[i] = a;
    }

    //爆弾を投げる関数
    private void ThrowBomb(int bombNum)
    {
        //入力クールタイムスタート
        isInputTimer = true;

        //AddForceで投げるので、isKinematicをfalseする
        bombRb[bombNum].isKinematic = false;

        //コライダー復活
        bombScript[bombNum].ColliderEnabled(true);

        //爆弾を持っている判定をfalseにする
        energyTank.MissingCore();
        haveBomb[bombNum] = false;

        //爆弾を飛ばす
        bombRb[bombNum].AddForce(bomb[bombNum].transform.forward * 500 * power);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //ステージ外に落ちた時、haveBombをfalseにする
        if (collision.gameObject.tag == "Reset")
        {
            if (haveBomb[0])
            {
                Debug.Log("reset");
                //ボムを持っている判定をfalseにする
                haveBomb[0] = false;

                bombRb[0].isKinematic = false;

                StartCoroutine(bombScript[0].PosReset(0));
            }
            else if (haveBomb[1])
            {
                Debug.Log("reset");
                //ボムを持っている判定をfalseにする
                haveBomb[1] = false;

                bombRb[1].isKinematic = false;

                StartCoroutine(bombScript[1].PosReset(0));
            }

            
            energyTank.DropEnergy(4);

            PlayerDead(2);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //ステージの穴に落ちたら動けなくなる
        if (other.gameObject.tag == "FallArea")
        {
            isDead = true;
        }

        //爆発に当たったら死亡
        if (other.gameObject.tag == "Explosion0" && !isDead)
        {
            if (haveBomb[1])
            {
                //ボムを持っている判定をfalseにする
                haveBomb[1] = false;
                energyTank.MissingCore();

                bombScript[1].Explosion();

                energyTank.DropEnergy(4);
            }

            PlayerDead(1);
        }
        if (other.gameObject.tag == "Explosion1" && !isDead)
        {
            if (haveBomb[0])
            {
                //ボムを持っている判定をfalseにする
                haveBomb[0] = false;
                energyTank.MissingCore();

                bombScript[0].Explosion();

                energyTank.DropEnergy(4);
            }

            PlayerDead(1);
        }
    }


    
    private void PlayerDead(int lostEnergy)
    {
        //プレイヤー死亡
        isDead = true;

        haveBomb[0] = false;
        haveBomb[1] = false;

        //プレイヤーのモデルを非表示
        body.SetActive(false);
        head.SetActive(false);
        hand.SetActive(false);

        //墓のオブジェクトが当たらないようにコライダーをオフ
        capsule.enabled = false;

        //爆弾を取れなくする
        takeBomb.CanTakeChange(0, false);
        takeBomb.CanTakeChange(1, false);


        //StartCoroutine(craneScript.RotateToTarget());
        StartCoroutine(posReset(lostEnergy));
    }

    private IEnumerator posReset(int lostEnergy)
    {
        yield return new WaitForSeconds(1.0f);

        //スタート位置にリスポーン
        transform.position = startPos;

        //プレイヤーのモデルを表示
        body.SetActive(true);
        head.SetActive(true);
        hand.SetActive(true);

        //コライダー復活
        capsule.enabled = true;

        //プレイヤー復活
        isDead = false;

        //落下時タンクのエネルギー2,爆発に当たったとき1消費
        savingTank.UseEnergy(lostEnergy);
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
