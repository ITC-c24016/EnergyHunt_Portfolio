using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnergyTank_copy
    : MonoBehaviour
{
    float energyTimer;

    //エネルギーが貯まる速さ
    [SerializeField] float chageSpeed = 1;

    //タンクの容量
    [SerializeField] int maxEnergy;

    //所持タンクのエネルギー量
    public int energyAmount;

    //携帯用のタンクオブジェクト
    [SerializeField] GameObject enagytank;

    //貯蓄用のタンクオブジェクト
    [SerializeField] GameObject savingTank;

    //貯蓄用のタンクオブジェクトのスクリプト
    SavingTank savingTankSC;

    //タンクに預けられるかの判定
    private bool canSave = false;

    //エネルギーを移す時に使うボタンからの入力を受け取る変数
    private InputAction putInAction;

    //エネルギーコアを所持しているかどうかの判定
    public bool charge;

    //エネルギーがドロップしたとき用のオブジェクト
    [SerializeField] GameObject dropEnergy;

    //ドロップオブジェクトのスクリプト
    DropEnergy_copy dropEnergySC;

    //プレイヤーコントローラースクリプト
    [SerializeField] PlayerController_copy playerController;

    [SerializeField, Header("所持タンクの初期位置")]
    private Vector3 startPos;

    [SerializeField, Header("ドロップするときの勢い")]
    float dropPower;

    void Start()
    {
        //PlayerInputコンポーネントを取得
        var input = GetComponent<PlayerInput>();

        //取得したPlayerInputのActionMapを取得
        var actionMap = input.currentActionMap;

        //ActionMap内の取り出し、預入れアクションを取得
        putInAction = actionMap["PutIn"];

        //スクリプト取得
        dropEnergySC = dropEnergy.GetComponent<DropEnergy_copy>();
        savingTankSC = savingTank.GetComponent<SavingTank>();
        //スタート時のタンクの位置を取得
        //startPos = transform.localPosition;
    }

    private void Update()
    {
        //エネルギーを移すボタンが押されたかどうかをチェック
        var putIn = putInAction.triggered;

        //エネルギーコア所持時のエネルギー増加
        if (charge)
        {

            energyTimer += Time.deltaTime * chageSpeed;

            //1秒ごとにエネルギー増加
            if (energyTimer >= 1)
            {
                energyTimer = 0;

                ChargeEnergy(1);


            }
        }
        else
        {
            energyTimer = 0;
        }


        //エネルギーの預け入れができる状態だったら預け入れる
        if (putIn && canSave && !charge && energyAmount >= maxEnergy)
        {
            TankCharging();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == $"TankArea{playerController.controllerNum}")
        {
            canSave = true;
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == $"TankArea{playerController.controllerNum}")
        {
            canSave = false;
        }
    }


    //エネルギー増加とスケールアップ
    public void ChargeEnergy(int amount)
    {
        energyAmount += amount;

        //所持エネルギーがタンク容量を超えないようにする
        if (energyAmount >= maxEnergy)
        {
            energyAmount = maxEnergy;
        }

        //Debug.Log("充電中");
        //エネルギー量に応じてタンクのメーターをスケールアップし、メーターのポジションを調整
        enagytank.transform.localScale = new Vector3(transform.localScale.x, energyAmount * 0.012f, transform.localScale.z);
        enagytank.transform.localPosition = new Vector3(transform.localPosition.x, -0.04f + (energyAmount * 0.012f), transform.localPosition.z);
    }

    //エネルギーコアを持っているとき
    public void HaveCore()
    {
        charge = true;
    }


    //エネルギーコアを手放したとき
    public void MissingCore()
    {
        charge = false;
    }

    //エネルギーコアの爆発に巻き込まれたときの所持エネルギードロップ
    public void DropEnergy(int target)
    {
        //エネルギーコア所持状態の解除
        MissingCore();

        //タンクのエネルギー量が0でない場合、ドロップオブジェクトを落とす
        if (energyAmount != 0)
        {
            //ドロップオブジェクトを自身の位置に移動してドロップ

            dropEnergy.transform.localScale = new Vector3(0.3f, energyAmount * 0.024f, 0.3f);
            dropEnergy.transform.position = enagytank.transform.position;

            Rigidbody dropRb = dropEnergy.GetComponent<Rigidbody>();
        }

        //ドロップオブジェクトにエネルギーを引継ぎ
        dropEnergySC.SetEnergyAmount(energyAmount);

        //ドロップオブジェクトのターゲットを選択
        dropEnergySC.SelectPos(target);

        //エネルギーリセット
        energyAmount = 0;

        //メーターのスケールとポジションをリセット
        enagytank.transform.localScale = new Vector3(0.165f, 0, 0.165f);
        enagytank.transform.localPosition = startPos;

        //タンクの非表示
        enagytank.SetActive(false);
    }

    //貯蓄用のタンクにエネルギータンクを移す
    public void TankCharging()
    {
        //貯蓄用のタンクのエネルギー増加
        savingTankSC.Charge();
    }
}
