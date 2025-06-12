using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionScript : MonoBehaviour
{
    [SerializeField] BombScript bombScript;
    [SerializeField] PlayerController[] playerController;
    //private EnergyTank energyTank;

    [SerializeField, Header("バリアオブジェクト")]
    GameObject[] barrier;

    [SerializeField] Vector3 tagetScale;

    [SerializeField, Header("エフェクトの円")]
    GameObject circle;

    [SerializeField, Header("エフェクトの中心")]
    GameObject center;

    [SerializeField, Header("エフェクトのビリビリ1")]
    GameObject thander1;

    [SerializeField, Header("エフェクトのビリビリ2")]
    GameObject thander2;


    //爆発範囲
    [SerializeField] float explosionRange = 2;

    [SerializeField] int bomberNum;

    public float maxTime = 2f;

    [SerializeField]
    AudioSource audioSource;

    //自爆判定
    public bool isSelf = false;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    /*public IEnumerator BomeEffect()
    {
        float timer = 0f;
        Vector3 originalScale = transform.localScale;
    
        while (timer < maxTime)
        {
            float ScaleChangeTime = timer / maxTime;
            timer += Time.deltaTime;
            transform.localScale = Vector3.Lerp(originalScale, tagetScale * explosionRange, ScaleChangeTime);
            yield return null;
        }        
    }*/

    public void Owner(int playerNum)
    {
        bomberNum = playerNum;
    }

    public void SetIsSelf(int playerNum)
    {
        if (playerNum == bomberNum)
        {
            isSelf = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "P1" || other.gameObject.tag == "P2" || other.gameObject.tag == "P3" || other.gameObject.tag == "P4")
        {
            audioSource.Play();

            var playerNum = other.GetComponent<PlayerController>().controllerNum;

            if (!barrier[playerNum - 1].GetComponent<BarrierScript>().isBarrier)
            {
                var energyTank = other.gameObject.GetComponent<EnergyTank>();

                if (playerNum == bomberNum)
                {
                    isSelf = true;                    
                }

                energyTank.DropEnergy(bomberNum);
            }
        }

        if (other.gameObject.tag == "Floor")
        {
            audioSource.Play();

            //当たったブロックのスクリプトを取得
            var blockScript = other.GetComponent<BlockScript>();

            //壊れている時は通らないようにする
            if (!blockScript.isBroken)
            {
                blockScript.SetBlock(false);
            }
        }
    }

    //強化タンクによる範囲変更
    public void ChangeRange()
    {
        gameObject.transform.localScale *= 1.1f;
        circle.transform.localScale *= 1.1f;
        center.transform.localScale *= 1.1f;
        thander1.transform.localScale *= 1.1f;
        thander2.transform.localScale *= 1.1f;
    }
}
