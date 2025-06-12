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

    //���������̔���
    public bool isExplosion = false;

    //�_�Œ����̔���
    public bool isBlink = false;

    //�������Ă��邩�̔���
    public bool isThrow = false;

    //�R�A�_�ł̃R���[�`��
    Coroutine _blinkBomb;

    [SerializeField,Header("�R�A��_�ł�����Ƃ��̐F")]
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
                Debug.Log("�_��");
                isBlink = true;
                _blinkBomb = StartCoroutine(BlinkBomb());
            }
        }
    }

    public void IsTimerTrue()
    {
        //�������Ԃ������_���őI��
        int i = Random.Range(0, 4);
        //�f�o�b�O�p
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

        //�^�C�}�[�X�^�[�g
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

    //���e�������āA�����G�t�F�N�g��\������
    public void Explosion()
    {
        //�_�Œ�~
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

    //�����G�t�F�N�g���\��
    public IEnumerator ExplosionScaleReset(float waitSecond)
    {
        yield return new WaitForSeconds(waitSecond);

        explosionRange.SetActive(false);

        isExplosion = false;


        ExplosionRange(1);
    }

    //���e��_�ł�����
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

    //�{���̃|�W�V���������Z�b�g
    public IEnumerator PosReset(float waitSecond)
    {
        //�_�Œ�~
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

        //���o��
        mesh.enabled = true;
        sphere.enabled = true;

        bombRb.isKinematic = false;
    }
    

    private void OnCollisionEnter(Collision collision)
    {
        //�^�C�}�[���i��ł��鎞�A���̃I�u�W�F�N�g�ɓ��������甚��������
        if (isTimer && (collision.gameObject.tag == "Floor" || collision.gameObject.tag == "Ground"
            || collision.gameObject.tag == "P1" || collision.gameObject.tag == "P2" || collision.gameObject.tag == "P3" || collision.gameObject.tag == "P4"))
        {
            SetIsThrow(false);

            

            Explosion();
        }

        //�X�e�[�W�O�ɗ������ꍇ�A�X�|�[���|�W�V�����ɖ߂�
        if (collision.gameObject.tag == "Reset" || collision.gameObject.tag == "OutSide")
        {
            isTimer = false;
            timer = 0;

            SetIsThrow(false);

            StartCoroutine(PosReset(0));
        }
        
        //�o���A�ɓ���������
        if (collision.gameObject.tag == "Barrier")
        {
            //�o���A������
            //collision.gameObject.GetComponent<BarrierScript>().BarrierCoolTime();

            //���˕Ԃ�
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
