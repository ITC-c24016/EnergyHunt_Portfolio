using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombScript : MonoBehaviour
{
    [SerializeField] ExplosionScript explosionScript;

    public GameObject explosionRange;

    [SerializeField] MeshRenderer mesh;
    [SerializeField] SphereCollider sphere;

    private Rigidbody bombRb;

    [SerializeField] Vector3 startPos;

    [SerializeField] int bombNum;

    private float bombTimer;
    private float timer;

    public bool isTimer;

    //爆発中かの判定
    public bool isExplosion = false;

    //点滅中かの判定
    public bool isBlink = false;

    //投げられているかの判定
    public bool isThrow = false;

    //コア点滅のコルーチン
    Coroutine _blinkBomb;

    [SerializeField,Header("コアを点滅させるときの色")]
    Color[] bombColor;

    void Start()
    {
        isTimer = false;

        transform.localPosition = startPos;

        bombRb = gameObject.GetComponent<Rigidbody>();


    }

    void Update()
    {
        if (!isExplosion) 
        {
            explosionRange.transform.position = transform.position;
        }

        if (isTimer)
        {
            timer += Time.deltaTime;

            if (timer >= bombTimer)
            {
                SetIsThrow(false);

                Explosion();
            }

            if ((bombTimer - timer <= 5) && !isBlink) 
            {
                Debug.Log("点滅");
                isBlink = true;
                _blinkBomb = StartCoroutine(BlinkBomb());
            }
        }
    }

    public void IsTimerTrue()
    {
        //爆発時間をランダムで選ぶ
        int i = Random.Range(0, 4);
        //デバッグ用
        //i = 0;
        switch (i)
        {
            case 0:
                bombTimer = 5; 
                break;
            case 1: 
                bombTimer = 10; 
                break;
            case 2: 
                bombTimer = 15; 
                break;
            case 3: 
                bombTimer = 20;
                break;
            case 4:
                bombTimer = 180;
                break;
        }

        //タイマースタート
        isTimer = true;
    }

    public void IsKinematicTrue()
    {
        bombRb.isKinematic = true;
    }

    public void SetIsThrow(bool set)
    {
        isThrow = set;
    }

    //爆弾を消して、爆発エフェクトを表示する
    public void Explosion()
    {
        //点滅停止
        if (_blinkBomb != null)
        {
            isBlink = false;
            StopCoroutine(_blinkBomb);
            GetComponent<Renderer>().material.color = bombColor[1];
        }
        

        explosionRange.SetActive(true);
        //StartCoroutine(explosionScript.BomeEffect());

        isExplosion = true;

        isTimer = false;
        timer = 0;

        bombRb.isKinematic = false;
        //mesh.enabled = false;
        sphere.enabled = true;

        

        //StartCoroutine(PosReset(0.5f));
        StartCoroutine(ExplosionScaleReset(1.0f));
    }

    //爆発エフェクトを非表示
    public IEnumerator ExplosionScaleReset(float waitSecond)
    {
        yield return new WaitForSeconds(waitSecond);

        explosionRange.SetActive(false);

        isExplosion = false;


        ExplosionRange(1);
    }

    //爆弾を点滅させる
    private IEnumerator BlinkBomb()
    {

        float blinkNum = 0;
        int i = 0;

        while (blinkNum < 10)
        {
            blinkNum ++;

            i = 1 - i;
            GetComponent<Renderer>().material.color = bombColor[i];
            
            yield return new WaitForSeconds(0.5f);
        }

        GetComponent<Renderer>().material.color = bombColor[1];

        isBlink = false;

        yield return null;
    }

    //ボムのポジションをリセット
    public IEnumerator PosReset(float waitSecond)
    {
        //点滅停止
        if (_blinkBomb != null)
        {
            isBlink = false;
            StopCoroutine(_blinkBomb);
            GetComponent<Renderer>().material.color = bombColor[1];
        }

        isTimer = false;
        timer = 0;
        bombRb.isKinematic = true;

        yield return new WaitForSeconds(waitSecond * 2);

        explosionRange.SetActive(false);


        yield return new WaitForSeconds(waitSecond);

        transform.rotation = Quaternion.Euler(0, 0, 0);
        transform.localPosition = startPos;


        //yield return new WaitForSeconds(0.2f);

        //視覚化
        mesh.enabled = true;
        sphere.enabled = true;

        bombRb.isKinematic = false;
    }
    

    private void OnCollisionEnter(Collision collision)
    {
        //タイマーが進んでいる時、他のオブジェクトに当たったら爆発させる
        if (isTimer && (collision.gameObject.tag == "Floor" || collision.gameObject.tag == "Ground"
            || collision.gameObject.tag == "P1" || collision.gameObject.tag == "P2" || collision.gameObject.tag == "P3" || collision.gameObject.tag == "P4"))
        {
            SetIsThrow(false);

            

            Explosion();
        }

        //ステージ外に落ちた場合、スポーンポジションに戻す
        if (collision.gameObject.tag == "Reset" || collision.gameObject.tag == "OutSide")
        {
            isTimer = false;
            timer = 0;

            SetIsThrow(false);

            StartCoroutine(PosReset(0));
        }
        
        //バリアに当たったら
        if (collision.gameObject.tag == "Barrier")
        {
            //バリアを消す
            //collision.gameObject.GetComponent<BarrierScript>().BarrierCoolTime();

            //跳ね返る
            Vector3 reflection = bombRb.velocity - transform.up;
            bombRb.AddForce(reflection * -2.0f, ForceMode.Impulse);

            //var reflection = Vector3.Reflect(bombRb.velocity, transform.up);
            //bombRb.velocity = reflection * 100.0f;
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag=="Explosion0"|| other.gameObject.tag == "Explosion1")
        {
            SetIsThrow(false);

            Explosion();
        }
    }

    public void ExplosionRange(float range)
    {
        //explosionScript.ChangeRange(range);
    }

    public void ColliderEnabled(bool a)
    {
        sphere.enabled = a;
    }
}
