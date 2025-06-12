using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static UnityEngine.GraphicsBuffer;

public class GameController : MonoBehaviour
{
    private int winnerPlayerNum;

    private int imageNum;

    [SerializeField]
    [Header("スケールがmaxになるまでの時間")]
    float maxTime = 1.5f;

    [SerializeField]
    [Header("SelectUIのスケールのマックス値")]
    float maxScale = 1.1f;

    private AnimationCurve animationCurve;

    [SerializeField, Header("リザルトUI")]
    GameObject[] resultImage;

    [SerializeField, Header("勝者のImage")]
    GameObject[] winnerImage;

    [SerializeField, Header("プレイヤーオブジェクト")]
    GameObject[] player;

    //UI選択の入力
    private InputAction stickAction, buttonAction;


    // 初期化
    private void Awake()
    {
        // デバイスの変更を監視
        InputSystem.onDeviceChange += OnDeviceChange;

        // 最初に接続されているデバイス一覧をログ出力
        PrintAllDevices();
    }

    void Start()
    {
        Application.targetFrameRate = 60;

        // カーブが設定されていない場合、デフォルトのカーブを作成
        if (animationCurve == null || animationCurve.length == 0)
        {
            animationCurve = new AnimationCurve(
                new Keyframe(0f, 0f),    // スタート
                new Keyframe(0.5f, 1f), // スケールアップ
                new Keyframe(1f, 0f)   // スケールダウン
            );
        }

        StartCoroutine(ScaleLoop(resultImage[imageNum], resultImage[imageNum].transform.localScale));
    }

    private void Update()
    {
        if (stickAction != null && buttonAction != null)
        {
            //Image切り替え
            var stickAct = stickAction.ReadValue<Vector2>().x;

            if (stickAct < -0.2f)
            {
                imageNum = 0;

                resultImage[1].SetActive(false);

                StartCoroutine(ScaleLoop(resultImage[imageNum], resultImage[imageNum].transform.localScale));
            }

            if (stickAct > 0.2f)
            {
                imageNum = 1;

                resultImage[0].SetActive(false);

                StartCoroutine(ScaleLoop(resultImage[imageNum], resultImage[imageNum].transform.localScale));
            }

            //ボタン入力
            var buttonAct = buttonAction.triggered;

            if (buttonAct)//Aボタン　or 〇ボタンを押したら
            {
                switch (imageNum)
                {
                    case 0:
                        SceneManager.LoadScene("TitleScene");
                        break;
                    case 1:
                        SceneManager.LoadScene("MainGameScene");
                        break;
                }
            }
        }
    }

    //勝ったプレイヤーの番号を取得
    public void SetWinnerNum(int playerNum)
    {
        winnerPlayerNum = playerNum;

        GetPlayerInput();

        //リザルトのUIを表示する
        winnerImage[winnerPlayerNum].SetActive(true);
        resultImage[2].SetActive(true);
        resultImage[3].SetActive(true);
    }

    //勝ったプレイヤーのPlayerInputを取得
    private void GetPlayerInput()
    {
        var input = player[winnerPlayerNum].GetComponent<PlayerInput>();
        var actionMap = input.currentActionMap;
        stickAction = actionMap["Move"];
        buttonAction = actionMap["PickUp"];
    }

    //スケールの拡大、縮小させるための処理
    IEnumerator ScaleLoop(GameObject tagetUI, Vector3 originalScale)
    {
        resultImage[imageNum].SetActive(true);

        float elapsedTime = 0f;
        Vector3 targetScale = originalScale * maxScale;

        //無限ループ
        while (true)
        {
            while (elapsedTime < maxTime)
            {
                elapsedTime += Time.deltaTime;

                // カーブに従ったスケール値を計算
                float t = elapsedTime / maxTime;
                float scaleFactor = animationCurve.Evaluate(t);
                tagetUI.transform.localScale = Vector3.Lerp(originalScale, targetScale, scaleFactor);

                yield return null;
            }

            elapsedTime = 0f; // 経過時間をリセット
        }
    }

    // プレイヤー入室時に受け取る通知
    public void OnPlayerJoined(PlayerInput playerInput)
    {
        print($"プレイヤー#{playerInput.user.index}が入室！");
    }

    // プレイヤー退室時に受け取る通知
    public void OnPlayerLeft(PlayerInput playerInput)
    {
        print($"プレイヤー#{playerInput.user.index}が退室！");
    }
    
    // 終了処理
    private void OnDestroy()
    {
        // デバイス変更の監視解除
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    // 全てのデバイスをログ出力
    private void PrintAllDevices()
    {
        // デバイス一覧を取得
        var devices = InputSystem.devices;

        // 現在のデバイス一覧をログ出力
        var sb = new StringBuilder();
        sb.AppendLine("現在接続されているデバイス一覧");

        for (var i = 0; i < devices.Count; i++)
        {
            sb.AppendLine($" - {devices[i]}");
        }

        print(sb);
    }

    // デバイスの変更を検知した時の処理
    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        switch (change)
        {
            case InputDeviceChange.Added:
                print($"デバイス {device} が接続されました。");
                break;

            case InputDeviceChange.Disconnected:
                print($"デバイス {device} が切断されました。");
                break;

            case InputDeviceChange.Reconnected:
                print($"デバイス {device} が再接続されました。");
                break;

            default:
                // 接続や切断以外の変更は無視
                return;
        }

        // 接続や切断があった場合は、全てのデバイスを再びログ出力
        PrintAllDevices();
    }
}


