using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ResultScript : MonoBehaviour
{
    [SerializeField]
    [Header("スケールがmaxになるまでの時間")]
    float maxTime = 1.5f;

    [SerializeField]
    [Header("SelectUIのスケールのマックス値")]
    float maxScale = 1.1f;

    [SerializeField]
    [Header("シーンの番号(0 or 1)")]
    int sceneNum = 0;
    
    //スケールのcurve
    private AnimationCurve animationCurve;

    [SerializeField]
    [Header("スケールをいじるUI")]
    GameObject[] ui;

    [SerializeField]
    [Header("勝利プレイヤーUI")]
    GameObject[] playerUI;

    [SerializeField]
    GameObject[] text;
    
    private Vector3 playerUIMaxScale;

    private PlayerControll inputActions;
    private InputAction stickAction;//スティック入力
    private InputAction buttonAction;

    private Vector3 playerUIOriginalScale;

    private float scaleUpMaxTime = 0.8f;//PlayerUIが拡大するmaxTime

    private Coroutine coroutine;//コールチンを入れるためのもの


    private void Awake()
    {
        

        inputActions = new PlayerControll();
        inputActions.Enable();

        stickAction = inputActions.Player.Move;
    }


    void Start()
    {
        //text[GameController.winnerPlayerNum].SetActive(true);
        //Debug.Log(GameController.winnerPlayerNum);
        //animationCurveが設定されてない場合、animationCurveをセットする
        if (animationCurve == null || animationCurve.length == 0)
        {
            animationCurve = new AnimationCurve(
                new Keyframe(0f, 0f),
                new Keyframe(0.5f, 1f),
                new Keyframe(1f, 0f)
            );
        }

        playerUIMaxScale = new Vector3(1,1,1);

        StartAnim();//最初のコールチンをスタート
    }


    void Update()
    {
        float horizontalStick = stickAction.ReadValue<Vector2>().x;//横方向のスティック入力を取得

        if (sceneNum < 1)
        {
            if (horizontalStick > 0)
            {
                sceneNum++;
                StartAnim();//コールチンを切り替え
            }
        }
        if (sceneNum > 0)
        {
            if (horizontalStick < 0)
            {
                sceneNum--;
                StartAnim();//コールチンを切り替え
            }
        }

        if (inputActions.Player.Throw.triggered)//Aボタン　or 〇ボタンを押したら
        {
            switch (sceneNum)
            {
                case 0:
                    SceneManager.LoadScene("TitleScene");
                    break;
                case 1:
                    SceneManager.LoadScene("MainGameScene");
                    break;
            }
        }

        //拡大、縮小するUIのSetActive処理
        for (int i = 0; i < ui.Length; i++)
        {
            if (i == sceneNum)
            {
                ui[i].SetActive(true);
            }
            else
            {
                ui[i].SetActive(false);
            }
        }

        if(scaleUpMaxTime < 2)
        {
            scaleUpMaxTime += Time.deltaTime;
        }

        if(scaleUpMaxTime > 2)
        {
            SetUI();
        }
    }

    private void SetUI()
    {
        //勝者のUIを表示
        for (int i = 0; i < playerUI.Length; i++)
        {
            //if (GameController.winnerPlayerNum == i + 1)
            {
                //Debug.Log(GameController.winnerPlayerNum);
                playerUI[i].SetActive(true);
                playerUIOriginalScale = playerUI[i].transform.localScale;
                StartCoroutine(PlayerUIScaleUp());
            }
        }
    }

    //コールチンの切り替え処理
    void StartAnim()
    {
        if(coroutine != null)
        {
            StopCoroutine(coroutine);//今のコールチンを止める

            GameObject currentUI = ui[sceneNum];
            currentUI.transform.localScale = Vector3.one;//UIの大きさをリセットする
        }

        GameObject tagetUI = ui[sceneNum];//ScaleLoopのtagetUIを取得
        Vector3 originalScale = tagetUI.transform.localScale;//ScaleLoopのoriginalScaleを取得

        coroutine = StartCoroutine(ScaleLoop(tagetUI, originalScale));//コールチンをセット
    }


    //スケールの拡大、縮小させるための処理
    IEnumerator ScaleLoop(GameObject tagetUI,Vector3 originalScale)
    {
        float timer = 0f;//maxTimeまでの時間を計算するタイマー
        Vector3 tagetScale = originalScale * maxScale;

        //無限ループ
        while (true)
        {
            while(timer < maxTime)
            {
                timer += Time.deltaTime;
                float t = timer / maxTime;
                float scaleCurve = animationCurve.Evaluate(t);//animationCurveの時間の計算
                tagetUI.transform.localScale = Vector3.Lerp(originalScale, tagetScale, scaleCurve);//スケールの拡大、縮小処理

                yield return null;
            }
            timer = 0f;
        }
    }

    //勝ったプレイヤーのUIをスケールアップする
    IEnumerator PlayerUIScaleUp()
    {
        float timer = 0f;
        Vector3 tagetScale = new Vector3(1, 1, 1);

        while(timer < scaleUpMaxTime)
        {
            timer += Time.deltaTime;
            float t = timer / scaleUpMaxTime;
            for(int i = 0;i < playerUI.Length; i++)
            {
                playerUI[i].transform.localScale = Vector3.Lerp(playerUIOriginalScale, tagetScale, t);
            }

            yield return null;
        }
        for (int i = 0; i < playerUI.Length; i++)
        {
            playerUI[i].transform.localScale = tagetScale;
        }
    }
}
