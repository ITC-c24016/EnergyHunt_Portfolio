using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SavingTank : MonoBehaviour
{
    [SerializeField]
    GameObject focusCamera;

    [SerializeField]
    GameObject[] player;
    [SerializeField]
    int playerNum;

    [SerializeField, Header("プレイヤーを移動させるポジション")]
    Vector3 playerPos;

    [SerializeField, Header("移動した後の向き")]
    Quaternion playerAngle;

    [SerializeField, Header("背負っているタンク")]
    GameObject playerTank;

    [Header("タンクオブジェクト"), SerializeField]
    GameObject[] tank;

    [Header("タンクのゲージオブジェクト"), SerializeField]
    GameObject[] tankGauge;

    [SerializeField, Header("貯蓄エネルギーの個数")]
    Text energyAmountText;

    //貯蓄用タンク内のエネルギー量
    public int energyAmount;

    [SerializeField, Header("大砲の玉")]
    GameObject bullet;

    

    void Start()
    {
        
    }

    void Update()
    {
        
    }


    //貯蓄用タンクにエネルギーをチャージ
    public void Charge()
    {
        energyAmount++;

        //タンクを表示
        tank[energyAmount - 1].SetActive(true);
        //ゲージを表示
        tankGauge[energyAmount - 1].SetActive(true);

        //次のタンクを非表示
        if (energyAmount != 4)
        {
            tank[energyAmount].SetActive(false);
        }
      
        //ゲージ満タンのタンク数がマックスになったとき
        if (energyAmount == 4)
        {
            //全プレイヤーを動けなくする
            for (int i = 0; i < player.Length; i++)
            {
                player[i].GetComponent<PlayerController>().PlayerDead();
            }

            //所持タンクを非表示にする
            playerTank.SetActive(false);

            //プレイヤーを砲台の横に移動
            StartCoroutine(PlayerMove());

            var bulletScript = bullet.GetComponent<ShootBullet>();
            StartCoroutine(bulletScript.ShootBomb(2.0f));
        }
    }

    private IEnumerator PlayerMove()
    {
        player[playerNum].GetComponent<Rigidbody>().isKinematic = true;

        Vector3 startPos = player[playerNum].transform.localPosition;
        Quaternion startAngle = player[playerNum].transform.localRotation;

        float time = 0;

        while (time < 1)
        {
            time += Time.deltaTime;

            float px = Mathf.Lerp(startPos.x, playerPos.x, time / 1);
            float py = Mathf.Sin(Mathf.Lerp(0, 180, time / 1) * 3.14f / 180) - 0.8f * time * time / 1;
            //float py = Mathf.Lerp(startPos.y, playerPos.y, time / 1) - 9.8f * time * time / 1;
            float pz = Mathf.Lerp(startPos.z, playerPos.z, time / 1);

            player[playerNum].transform.localPosition = new Vector3(px, py, pz);
            player[playerNum].transform.localRotation = Quaternion.Lerp(startAngle, playerAngle, time / 1);

            yield return null;
        }

        StartCoroutine(focusCamera.GetComponent<CameraScript>().CameraFocus(player[playerNum], 2.0f));
        StartCoroutine(focusCamera.GetComponent<CameraScript>().CameraRotate(2.0f));

        //StartCoroutine(player[playerNum].GetComponent<PlayerController>().WinnerAnim(2.0f));
    }
    
    //貯蓄用タンク内エネルギー消費
    public void UseEnergy(int amount)
    {
        for (int i = 0; i < amount && energyAmount > 0; i++)
        {
            energyAmount -= 1;

            //ゲージを非表示
            tankGauge[energyAmount].SetActive(false);
        }    
    }
}
