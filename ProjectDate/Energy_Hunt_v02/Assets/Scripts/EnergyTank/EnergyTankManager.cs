using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerInput))]
public class EnergyTankManager : MonoBehaviour
{
    float energyTimer;

    //現在貯めている最中のタンクのエネルギー量
    public int energyAmount;

    //タンクひとつ当たりの容量
    [SerializeField] int maxEnergy;

    //所持中のタンクの個数
    public int energyTankAmount;

    //エネルギー満タン状態のタンクの個数
    [SerializeField] int fullTankAmount;

    //所持可能なタンクの上限
    [SerializeField] int tankAmountLimit;

    //エネルギーコアを所持しているかどうかの判定
    public bool charge;

    public Text scoreText;

    //プレイヤーが所持するタンクオブジェクト(３つ)
    [SerializeField] GameObject[] energyTank;

    //貯蓄用のタンクオブジェクト
    [SerializeField] GameObject savingTank;

    //貯蓄用のタンクオブジェクトのスクリプト
    SavingTank savingTankSC;

    //プレイヤーが所持するタンクごとのスクリプト
    [SerializeField] EnergyTank[] energyTankSC;

    //プレイヤーコントローラースクリプト
    [SerializeField] PlayerController playerController;

    //タンクに預けられるかの判定
    private bool canSave = false;

    //タンクから取り出せるかの判定
    private bool canTake = false;
    //エネルギーの取り出し、預入れに使うボタンからの入力を受け取る変数
    private InputAction putInAction, takeOutAction;

    void Start()
    {
        //PlayerInputコンポーネントを取得
        var input = GetComponent<PlayerInput>();

        //取得したPlayerInputのActionMapを取得
        var actionMap = input.currentActionMap;

        //ActionMap内の取り出し、預入れアクションを取得
        putInAction = actionMap["PutIn"];
        takeOutAction = actionMap["TakeOut"];

        savingTankSC = savingTank.GetComponent<SavingTank>();
    }

    void Update()
    {
        //取り出し、預け入れのボタンが押されたかどうかをチェック
        var putIn = putInAction.triggered;
        var takeOut = takeOutAction.triggered;

        //エネルギーコア所持時のエネルギー増加
        if (charge)//1P
        {

            if (fullTankAmount < tankAmountLimit)
            {
                energyTimer += Time.deltaTime * 4;

                //1秒ごとにエネルギー増加
                if (energyTimer >= 1)
                {
                    energyTimer = 0;

                    energyAmount++;

                    //現在充電中のタンクのエネルギー増加
                    //Debug.Log(energyTankAmount);
                    energyTankSC[energyTankAmount].ChargeEnergy(1);
                    scoreText.text = "" + energyAmount;

                    //所持エネルギーがタンク容量を超えた際のタンク追加
                    if (energyAmount >= maxEnergy)
                    {
                        //エネルギーリセット
                        energyAmount = 0;

                        //満タン状態のタンクのカウント増加
                        fullTankAmount++;
                        PowerUpAndDown();

                        //タンク追加
                        AddTank(false);
                    }
                }
            }
        }
        else
        {
            energyTimer = 0;
        }


        //エネルギーの預け入れができる状態だったら預け入れる
        if (putIn && canSave && !charge)
        {
            TankCharging();
        }

        //エネルギーの取り出しができる状態だったら取り出す
        if (takeOut && canTake && !charge)
        {
            TakeOutEnergy();
        }
    }


    //エネルギーコアの爆発に巻き込まれたときの所持エネルギードロップ
    public void LostEnergy()
    {
        //エネルギーコア所持状態の解除
        MissingCore();

        //所持していたエネルギーリセット
        energyAmount = 0;

        //満タン状態のタンクの個数リセット
        fullTankAmount = 0;
        PowerUpAndDown();

        //所持していたタンクごとのエネルギーに応じてドロップ
        for (int i = 0; i < energyTankAmount + 1; i++)
        {
            //energyTankSC[i].DropEnergy();
        }

        //タンクの数をリセット
        energyTankAmount = 0;
    }



    //ドロップしているエネルギーを拾った際のエネルギー増加
    private void OnTriggerEnter(Collider other)
    {
        //ドロップオブジェクトのスクリプト
        DropEnergy dropEnergy;

        //エネルギー量
        int energy;

        //衝突したオブジェクトがドロップオブジェクトだった場合
        if (other.tag == "DropEnergy")
        {
            if (energyAmount == 0)
            {
                energyTank[energyTankAmount].SetActive(true);
            }
            //ドロップオブジェクトのスクリプト取得
            dropEnergy = other.GetComponent<DropEnergy>();

            //ドロップオブジェクトの位置をリセット
            dropEnergy.PosReset();

            //ドロップオブジェクトのエネルギー量を取得
            energy = dropEnergy.energyAmount;

            //エネルギー増加の処理呼びだし
            PickUpEnergy(energy);
        }

        if (other.gameObject.tag == $"TankArea{playerController.controllerNum}")
        {
            canSave = true;
            canTake = true;
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == $"TankArea{playerController.controllerNum}")
        {
            canSave = false;
            canTake = false;
        }
    }


    //エネルギー増加
    public void PickUpEnergy(int amount)
    {
        //ドロップオブジェクトのエネルギー量がタンク容量以上だった場合
        if (amount >= maxEnergy)
        {
            //現在エネルギーを貯めているタンクを満タンにする
            energyTank[energyTankAmount].SetActive(true);
            //energyTankSC[energyTankAmount].SetEnergy(maxEnergy);

            //満タン状態のタンクの個数増加
            if (fullTankAmount < tankAmountLimit)
            {
                fullTankAmount++;
                PowerUpAndDown();
            }

            //所持しているタンクの個数が所持上限に達していない場合にタンクの追加
            if (energyTankAmount < tankAmountLimit - 1)
            {
                //タンク追加
                AddTank(true);
            }
        }

        //ドロップオブジェクトのエネルギーと現在貯めているタンクのエネルギー量の総量がタンク容量以上だった場合
        else if (amount + energyAmount >= maxEnergy)
        {
            //現在エネルギーを貯めているタンクを満タンにする
            energyTank[energyTankAmount].SetActive(true);
            //energyTankSC[energyTankAmount].SetEnergy(maxEnergy);

            //満タン状態のタンクの個数増加
            if (fullTankAmount < tankAmountLimit)
            {
                fullTankAmount++;
                PowerUpAndDown();
            }

            //所持しているタンクの個数が所持上限に達していない場合にタンクの追加
            if (energyTankAmount < tankAmountLimit - 1)
            {
                //タンクを追加
                AddTank(false);

                //溢れた分のエネルギーを追加したタンクに引継ぎ
                //energyTankSC[energyTankAmount].SetEnergy((amount + energyAmount) - maxEnergy);
                energyAmount = (amount + energyAmount) - maxEnergy;

            }
        }
        //ドロップオブジェクトのエネルギーと現在貯めているタンクのエネルギー量の総量がタンク容量を超えない場合
        else
        {
            //増加後のエネルギーを代入
            //energyTankSC[energyTankAmount].SetEnergy(amount + energyAmount);
            energyAmount += amount;

        }

    }


    //エネルギーコアを持っているとき
    public void HaveCore()
    {
        energyTank[0].SetActive(true);

        PowerUpAndDown();

        charge = true;
    }


    //エネルギーコアを手放したとき
    public void MissingCore()
    {
        charge = false;
    }


    //エネルギータンク追加
    public void AddTank(bool takeOut)
    {
        //タンクが所持上限に達していなかったらタンクを追加
        if (energyTankAmount < tankAmountLimit - 1)
        {
            //タンク追加
            energyTankAmount++;

            //追加分のタンクを表示
            Debug.Log("タンク追加");
            energyTank[energyTankAmount].SetActive(true);
        }

        //貯蓄用のタンクから取り出した場合、エネルギーを拾った場合のエネルギー引継ぎ
        if (takeOut)
        {
            if (energyAmount != 0)
            {
                //energyTankSC[energyTankAmount].SetEnergy(energyAmount);
            }
        }
    }


    //貯蓄用のタンクにエネルギータンクを移す
    public void TankCharging()
    {
        //１つ以上満タンのタンクがある場合
        if (fullTankAmount > 0 && savingTankSC.energyAmount < 9)
        {
            //移した分のタンク非表示
            energyTank[energyTankAmount].SetActive(false);

            if (energyTankAmount > 0)
            {
                //タンクを減らす
                energyTankAmount--;
            }

            if (energyTankAmount == 0)
            {
                energyTank[energyTankAmount].SetActive(true);
            }

            //満タンのタンクの数を減らす
            fullTankAmount--;
            PowerUpAndDown();

            if (energyAmount != 0)
            {
                //貯めている途中のエネルギーを残ったタンクに代入
                //energyTankSC[energyTankAmount].SetEnergy(energyAmount);
            }
            else if (energyAmount == 0 && energyTankAmount == 1)
            {
                //energyTankSC[energyTankAmount].SetEnergy(maxEnergy);
            }

            //タンクのエネルギーを初期化
            if (energyTankAmount == 1 && fullTankAmount == 1)
            {
                //energyTankSC[energyTankAmount].SetEnergy(0);
            }
            else
            {
                //energyTankSC[energyTankAmount + 1].SetEnergy(0);
            }

            //貯蓄用のタンクのエネルギー増加
            savingTankSC.Charge();
        }

        if (fullTankAmount == 0 && energyAmount == 0)
        {
            //energyTankSC[energyTankAmount].SetEnergy(0);
        }


    }


    //貯蓄用のタンクからエネルギーを取り出す
    public void TakeOutEnergy()
    {
        //満タン状態のタンクの個数が所持上限より少ない場合
        if (savingTankSC.energyAmount > 0 && fullTankAmount < tankAmountLimit)
        {
            //現在エネルギーを貯めているタンクを満タンにする
            if (energyTankAmount + 1 == fullTankAmount)
            {
                //energyTankSC[energyTankAmount + 1].SetEnergy(maxEnergy);
            }
            else
            {
                //energyTankSC[energyTankAmount].SetEnergy(maxEnergy);
            }

            //満タン状態のタンク数を増やす
            fullTankAmount++;
            PowerUpAndDown();

            //所持しているタンクの個数が所持上限に達していない場合にタンクの追加
            if (energyTankAmount < tankAmountLimit - 1)
            {
                AddTank(true);
            }

            //貯蓄用のタンク内のエネルギー消費
            savingTankSC.UseEnergy(1);
        }

        if (fullTankAmount >= tankAmountLimit)
        {
            energyAmount = 0;
        }
    }

    //満タン状態のタンクの個数に応じてプレイヤーの身体強化
    public void PowerUpAndDown()
    {
        playerController.FullTank(fullTankAmount);
    }

}
