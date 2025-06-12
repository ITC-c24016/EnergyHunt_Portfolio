using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraneScript : MonoBehaviour
{
    public float rotationSpeed = 90f;
    private bool isRotating = false;
    private bool isReturning = false;
    public Transform player;
    public Transform crane;
    public Transform craneTip;
    //public Player_Test player_Test;
    private Quaternion initialRotation;
    [SerializeField] Vector3 targetEulerAngles;
    private Quaternion targetRotation;

    void Start()
    {
        initialRotation = transform.rotation;
        //targetRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y - 90, transform.rotation.eulerAngles.z);
        targetRotation = Quaternion.Euler(targetEulerAngles);
    }

    void Update()
    {
        /*
        if (player.gameObject.transform.position.y < -5)
        {
            // プレイヤーが指定位置を下回ったら位置をリセット

            player.gameObject.transform.position = playerRespawnPos;//new Vector3(8.8f, 2.5f, -8.6f);
            player.SetParent(crane);

            // 回転開始
            if (!isRotating)
            {
                StartCoroutine(RotateToTarget(targetRotation));
            }
        }*/
    }

    public IEnumerator RotateToTarget()
    {

        Debug.Log("crane");
        isRotating = true;

        //player.SetParent(crane);

        // 回転をする
        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
        {
            player.transform.position = craneTip.transform.position;

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            yield return null;
        }

        transform.rotation = targetRotation; //最終的な値を確定

        //player_Test.rb.useGravity = true;

        //player.transform.parent = null;
        // 回転が90度に達したら0.5秒待機
        yield return new WaitForSeconds(0.5f);

        // 90度回転が完了したら元の回転に戻る
        StartCoroutine(RotateBackToInitial());
    }

    private IEnumerator RotateBackToInitial()
    {
        isReturning = true;

        // 初期回転に戻す
        while (Quaternion.Angle(transform.rotation, initialRotation) > 0.1f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, initialRotation, rotationSpeed * Time.deltaTime);
            yield return null;
        }

        transform.rotation = initialRotation;
        isReturning = false;
        isRotating = false;
    }
}
