using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class DropEnergyTank : MonoBehaviour
{
    [SerializeField]
    EnergyTank[] energyTank;

    [SerializeField]
    GameObject[] explotion;

    [SerializeField]
    GameObject[] text;

    [SerializeField, Header("メーターオブジェクト")]
    GameObject meter;

    //タンクのエネルギー量
    [SerializeField]
    private int value;

    [SerializeField, Header("マックス容量")]
    int maxValue;

    [SerializeField, Header("コアオブジェクト")]
    GameObject[] core;
    [SerializeField, Header("トレイルオブジェクト")]
    GameObject[] trall;
    [SerializeField, Header("トレイルの移動スピード")]
    float speed;
    //トレイルオブジェクトの初期位置
    private Vector3 startPos;
    //イベントの回数
    private int eventNum;
    //イベント中の判定
    private bool isEvent0 = false;
    private bool isEvent1 = false;

    void Start()
    {
        startPos = trall[0].transform.localPosition;
    }

    void Update()
    {
        //イベントが発生したらコアに向かってエネルギーを飛ばす
        if (isEvent0)
        {
            trall[0].transform.position = Vector3.MoveTowards(trall[0].transform.position, core[0].transform.position, speed);

            if(trall[0].transform.position== core[0].transform.position)
            {
                isEvent0 = false;

                trall[0].SetActive(false);
                trall[0].transform.localPosition = startPos;
            }
        }

        if (isEvent1)
        {
            trall[1].transform.position = Vector3.MoveTowards(trall[1].transform.position, core[1].transform.position, speed);

            if (trall[1].transform.position == core[1].transform.position)
            {
                isEvent1 = false;

                trall[1].SetActive(false);
                trall[1].transform.localPosition = startPos;
            }
        }
    }

    public void ScaleChange(int energy)
    {
        if (value == 0)
        {
            meter.SetActive(true);
        }

        value += energy / (eventNum + 1) * 2;
        if (value > maxValue)
        {
            value = maxValue;
        }

        meter.transform.localScale = new Vector3(meter.transform.localScale.x, meter.transform.localScale.y, value * 0.009f);
        meter.transform.localPosition = new Vector3(meter.transform.localPosition.x, meter.transform.localPosition.y, 4.03f - (value * 0.0046f));

        if (value == maxValue && eventNum <= 2)
        {
            HappenEvent();
        }
    }

    private void HappenEvent()
    {
        StartCoroutine(SetText(0));

        isEvent0 = true;
        isEvent1 = true;

        trall[0].SetActive(true);
        trall[1].SetActive(true);

        for (int i = 0; i < energyTank.Length; i++)
        {
            //タンクのチャージスピードを速くする
            energyTank[i].chargeSpeed *= 2;
        }

        //放電範囲を広くする
        //explotion[0].GetComponent<ExplosionScript>().ChangeRange();
        //explotion[1].GetComponent<ExplosionScript>().ChangeRange();
    }

    private IEnumerator SetText(int i)
    {
        text[i].SetActive(true);

        //エネルギーリセット
        value = 0;
        ScaleChange(0);

        yield return new WaitForSeconds(2);

        text[i].SetActive(false);

        eventNum++;
    }
}
