using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DropEnergy_copy : MonoBehaviour
{
    //エネルギー量
    public int energyAmount;

    [SerializeField, Header("プレイヤーのポジション")]
    Vector3[] playerPos;

    [SerializeField, Header("EnergyTankスクリプト")]
    EnergyTank_copy[] energytankSC;

    [SerializeField, Header("スタートのポジション")]
    Vector3 startPos;

    int player;

    bool target;

    [SerializeField] float speed = 1;
    private void Start()
    {
        
    }
    public void Update()
    {
        if (target)
        {
            transform.position = Vector3.MoveTowards(transform.position, playerPos[player], speed);
            if(transform.position == playerPos[player])
            {
                energytankSC[player].ChargeEnergy(energyAmount);
                target = false;
                PosReset();
            }
        }
    }
    //所持タンクオブジェクトからエネルギーを引継ぎ
    public void SetEnergyAmount(int amount)
    {
        energyAmount = amount;
    }

    public void PosReset()
    {
        transform.localPosition = startPos;
    }

    public void SelectPos(int playerNum)
    {
        player = playerNum;
        target = true;
    }
    
}
