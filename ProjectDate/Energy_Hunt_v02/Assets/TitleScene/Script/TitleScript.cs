using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScript : MonoBehaviour
{
    float maxTime = 1.5f;//スケールアップとダウンにかける時間
    float maxScale = 1.1f;//最大スケールの値
    int sceneNum = 0;//UI制御のためのシーン番号

    [SerializeField]
    [Header("スケールのカーブ制御")]
    AnimationCurve scaleCurve;

    [SerializeField]
    [Header("UI要素")]
    GameObject[] uiElements;

    float bombScaleMaxTime = 0.7f;//BombUIの最大スケールになるまでの時間
    Vector3 tagetScale;//BombのtagetScale

    [SerializeField]
    [Header("TitleImageのオブジェクト")]
    GameObject titleImage;

    float startTime = 0f;//爆弾が飛ぶまでの時間

    private Vector3 bombOriginalScale;//TitleImageの元のスケール

    float inputCooldown = 0.25f;//入力制限の時間

    private float lastInputTime = 0f;//入力制限をするための時間

    float speed = 0.02f;//フェードアウトするスピード

    float bombFadeOutSpeed = 0.07f;//爆発がフェードアウトするスピード

    [SerializeField]
    [Header("フェードするイメージ")]
    Image fadeImage;

    [SerializeField]
    [Header("Bombのレクトトランスフォーム(RectTransform型)")]
    RectTransform bombUI;

    [SerializeField]
    [Header("Bombのイメージ(Image型)")]
    Image bombImage;

    [SerializeField]
    [Header("Bombのイメージ(GameObject型)")]
    GameObject bombImageObject;

    [SerializeField]
    [Header("Bombのスプライト")]
    Sprite newSprite;

    [SerializeField]
    [Header("開始位置")]
    Vector2 startPos;

    [SerializeField]
    [Header("目標地点")]
    Vector2 targetPos;

    float launchSpeed = 1000f;//爆弾が飛ぶスピード

    float gravity = -2000f;//仮の重力

    float rotationSpeed = 360f;//爆弾の回転速度

    [SerializeField]
    [Header("音素材")]
    AudioSource SE;

    [SerializeField]
    [Header("決定音")]
    AudioClip DecisionSE;

    [SerializeField]
    [Header("選択音")]
    AudioClip selectSE;

    [SerializeField]
    [Header("タイトルBGM")]
    AudioSource titleBGM;

    bool hasPlayedSound = false;//音を鳴らすためのbool

    float timeElapsed = 0f;//爆弾が飛んでる時間

    Vector3 velocity;//速度を計算するための箱

    float red, green, blue, alfa;//FadeImageのcolorをつかむためのもの

    float red2, green2, blue2, alfa2;//BombImageのcolorをつかむためのもの

    bool Out = false;//FadeOutのbool

    BomberGentleman inputActions;//InputManagerのスクリプト
    InputAction stickAction;//Stick操作の処理

    Coroutine currentCoroutine; // 現在実行中のコルーチンを保持

    [SerializeField]
    GameObject[] players;//プレイヤーオブジェクト

    [SerializeField]
    GameObject energyCore;//energyオブジェクト

    public float energyCoreThrowTimer = 0f;

    [SerializeField] Animator animator2;//プレイヤー2のアニメーション
    [SerializeField] Animator animator3;//プレイヤー3のアニメーション
   
    [SerializeField] Rigidbody enertyCoreRB;//energyコアのリジッドボディ
    [SerializeField] bool[] startAnim;//どのアニメーションを流すか

    [SerializeField] Quaternion targetRotation; // 目標の回転角度
    bool isRotating = false;   // 回転中かどうかのフラグ

    float start = 0f;

    [SerializeField]
    [Header("batteryに指しこむエネルギー（ためたやつ）")]
    GameObject[] energy;

    [SerializeField]
    [Header("Energyを指しこむ入れ物")]
    GameObject battery;//StartAnim[1](true)の時に使うもの

    bool isReturning = false;
    float returnTimer = 0f; // 0.5秒後の処理用タイマー
    Quaternion originalRotation; // 元の回転値

    float player4StartTime = 0f;//アニメーション3を流すためのタイマー

    [SerializeField]
    [Header("takeEnergyCoreを入れるためのもの")]
    GameObject takeEnergyCore;

    float backTime = 0f;

    float setTime = 0f;

    [SerializeField] bool[] isMove;

    [SerializeField]
    [Header("コンベアのアニメーター")]
    Animator conveyorAnim;

    bool hasReachedPosition = false; // 停止したかどうか
    bool isMovingAgain = false; // 再開したかどうか

    Vector3 force;

    private void Awake()
    {
        inputActions = new BomberGentleman();
        inputActions.Enable();

        stickAction = inputActions.Player.Move; // Moveの中のLeftStickをつかむ

        scaleCurve = null; // ScaleCurveの設定を消す
    }

    void Start()
    {
        // 初期位置を設定
        bombUI.anchoredPosition = startPos;

        // ターゲットまでの距離と方向を計算
        Vector2 direction = targetPos - startPos;

        // 水平方向の時間を計算
        float timeToTarget = direction.magnitude / launchSpeed;

        // 垂直方向の速度を計算
        float verticalSpeed = (direction.y - 0.5f * gravity * Mathf.Pow(timeToTarget, 2)) / timeToTarget;

        // 初期速度を設定
        velocity = new Vector2(direction.normalized.x * launchSpeed, verticalSpeed);

        //FadeImageのcolorをつかむ
        red = fadeImage.color.r;
        green = fadeImage.color.g;
        blue = fadeImage.color.b;
        alfa = fadeImage.color.a;

        //BombImageのcolorをつかむ
        red2 = bombImage.color.r;
        green2 = bombImage.color.g;
        blue2 = bombImage.color.b;
        alfa2 = bombImage.color.a;

        bombOriginalScale = bombImage.transform.localScale;

        tagetScale = new Vector3(11, 11, 11);//爆発のマックススケール

        Application.targetFrameRate = 60;//フレームレートの固定

        // カーブが設定されていない場合、デフォルトのカーブを作成
        if (scaleCurve == null || scaleCurve.length == 0)
        {
            scaleCurve = new AnimationCurve(
                new Keyframe(0f, 0f),    // スタート
                new Keyframe(0.5f, 1f), // スケールアップ
                new Keyframe(1f, 0f)    // スケールダウン
            );
        }

        //fadeImageのImageがfalseだった場合
        if (!fadeImage.enabled)
        {
            fadeImage.enabled = true;
        }

        titleImage.SetActive(false);

        Out = true;//OutのboolをtrueにしてFadeOutを実行

        startAnim[0] = true;

        titleBGM.Play();

        // 初期化として最初のUIを動かす
        StartAnimationForScene();
    }

    void Update()
    {
        //videoPlayer.Play();//動画を再生

        if (Time.time - lastInputTime >= inputCooldown) // 入力制限
        {
            float verticalInput = stickAction.ReadValue<Vector2>().x;

            if (verticalInput > 0 && sceneNum < 2)
            {
                sceneNum++;
                lastInputTime = Time.time;
                SE.PlayOneShot(selectSE);
                StartAnimationForScene();
            }
            else if (verticalInput < 0 && sceneNum > 0)
            {
                sceneNum--;
                lastInputTime = Time.time;
                SE.PlayOneShot(selectSE); StartAnimationForScene();
            }
        }

        if (inputActions.Player.Start.triggered)
        {
            switch (sceneNum)
            {
                case 0:
                    SE.PlayOneShot(DecisionSE);//決定音を鳴らす
                    Invoke("SelectScene", 0.3f);//ゲームシーンに移動
                    break;
                case 1:
                    SE.PlayOneShot(DecisionSE);//決定音を鳴らす
                    Invoke("SelectScene", 0.3f);//説明シーンに移動
                    break;
                case 2:
                    SE.PlayOneShot(DecisionSE);//決定音を鳴らす
                    Invoke("SelectScene", 0.3f);//ゲームを終了
                    break;
            }
        }

        for (int i = 0; i < uiElements.Length; i++)
        {
            uiElements[i].SetActive(i == sceneNum);
        }

        if (Out)//Outのboolがtrueの場合フェードアウト
        {
            FadeOut();
        }

        if (startTime < 3f)//3秒よりタイマーが少なかったら
        {
            startTime += Time.deltaTime;
        }

        if (startTime > 1f)
        {
            // 前回の位置を保存
            Vector2 previousPosition = bombUI.anchoredPosition;

            // 時間経過を加算
            timeElapsed += Time.deltaTime;

            // 放物線の座標計算
            float x = velocity.x * timeElapsed;
            float y = velocity.y * timeElapsed + 0.5f * gravity * Mathf.Pow(timeElapsed, 2);

            // 新しい位置を計算
            Vector2 newPosition = new Vector2(startPos.x + x, startPos.y + y);

            // 回転を計算
            float rotationAngle = timeElapsed * rotationSpeed; // 時間に応じた回転
            bombUI.rotation = Quaternion.Euler(0, 0, rotationAngle); // 回転の更新

            // フレーム間で目標地点を超えたかを確認
            if (HasPassedTarget(previousPosition, newPosition, targetPos))
            {
                // 目標地点に位置を固定
                bombUI.anchoredPosition = targetPos;

                // 急激に回転させる
                float finalRotationAngle = 3000f; // 回転の最終角度（例えば720度に設定）

                // 回転をスムーズに補間するコルーチンを開始
                StartCoroutine(RotateBombSmoothly(finalRotationAngle));

                // 回転をリセットする場合は、少し時間をおいてから元に戻す
                if (!hasPlayedSound)
                {
                    //SE.PlayOneShot(bombSE);
                    hasPlayedSound = true;
                }

                timeElapsed = 0f; // 経過時間をリセット
                bombImage.sprite = newSprite; // 爆弾イメージから爆発イメージに変更

                StartCoroutine(BombScaleUp()); // 爆発アニメーションのコールチンを呼び出し
                if (startTime > 2.7f)
                {
                    titleImage.SetActive(true); // タイトルを表示
                }
                Invoke("BombFadeOut", 0.7f);//爆発が表示されてから0.7秒後にフェードアウトさせる
            }
            else
            {
                // 位置を更新
                bombUI.anchoredPosition = newPosition;
            }
        }

        // フレーム間で目標地点を通り過ぎたかを確認する
        bool HasPassedTarget(Vector2 previousPosition, Vector2 currentPosition, Vector2 targetPosition)
        {
            // 現在と前回の位置の間で目標地点を挟んでいるか
            return Vector2.Distance(previousPosition, targetPosition) < Vector2.Distance(currentPosition, targetPosition);
        }

        //放電で倒すアニメーション
        if (startAnim[0])
        {
            ChangeAnim1();
        }

        //Energyをチャージするアニメーション
        if (startAnim[1])
        {
            ChangeAnim2();
        }

        //エネルギーコアを奪うアニメーション
        if (startAnim[2])
        {
            ChangeAnim3();
        }
    }


    //スタート時のフェードアウトの処理
    void FadeOut()
    {
        fadeImage.enabled = true;
        alfa -= speed;
        Alpha();
        if (alfa <= 0)
        {
            Out = false;
            fadeImage.enabled = false;
        }
    }


    void Alpha()
    {
        fadeImage.color = new Color(red, green, blue, alfa);
    }


    //爆発のフェードアウト
    void BombFadeOut()
        {
            alfa2 -= bombFadeOutSpeed;
            Alpha2();
            if (alfa2 <= 0)
            {
                bombImage.enabled = fadeImage;
            }
        }


        void Alpha2()
        {
            bombImage.color = new Color(red2, green2, blue2, alfa2);
        }


        private void StartRotation()
        {
            // 目標の回転を設定（0, -90, 0 の回転）
            targetRotation = Quaternion.Euler(0, -90, 0);
            originalRotation = players[2].transform.rotation; // 元の回転を保存
            isRotating = true;
        }


        void SelectScene()
        {
            switch (sceneNum)
            {
                case 0:
                    SceneManager.LoadScene("MainGameScene");
                    break;
                case 1:
                    SceneManager.LoadScene("RuleScene");
                    break;
                case 2:
                    Application.Quit();
                    break;
            }
        }


    //startAnim[0]がtrueの時に流す
    void ChangeAnim1()
    {
        //プレイヤーが全身する処理
        if (players[0].transform.position.x > -2.05f)
        {
            players[0].transform.Translate(Vector3.back * 3 * Time.deltaTime);
            players[1].transform.Translate(Vector3.forward * 3 * Time.deltaTime);
        }

        if (players[0].transform.position.x < -2.05f)
        {
            if (energyCoreThrowTimer < 3f)
            {
                energyCoreThrowTimer += Time.deltaTime;
            }

            //プレイヤーが爆弾を投げる処理
            if (energyCoreThrowTimer > 1f)
            {
                animator3.SetBool("IsThrow", true);
                force= new Vector3(-29f, 0f, 0f);
                enertyCoreRB.AddForce(force);
                enertyCoreRB.constraints = RigidbodyConstraints.None;
            }

            //プレイヤーが前進する処理
            if (energyCoreThrowTimer > 3)
            {
                players[0].transform.Translate(Vector3.back * 3 * Time.deltaTime);
                players[1].transform.Translate(Vector3.forward * 3 * Time.deltaTime);
                conveyorAnim.speed = 1;//コンベアアニメーションをスタート
            }
            else
            {
                conveyorAnim.speed = 0;//コンベアアニメーションを止める
            }

            //すべてをリセットする
            if (players[0].transform.position.x < -18.5f)
            {
                players[0].transform.position = new Vector3(15.2f, -1.26f, 0.78f);
                players[1].transform.position = new Vector3(19.45f, -1.26f, 0.78f);
                startAnim[0] = false;
                force = Vector3.zero;
                animator2.Rebind();
                animator3.Rebind();
                energyCore.transform.position = new Vector3(19.7f, -0.42f, 0.78f);
                enertyCoreRB.constraints = RigidbodyConstraints.FreezeAll;
                enertyCoreRB.velocity = Vector3.zero;
                enertyCoreRB.transform.rotation = new Quaternion(0, 90, 0, 0);
                energyCoreThrowTimer = 0f;
                startAnim[1] = true;
            }
        }
    }

    //startAnim[1]がtrueの時に流す
    void ChangeAnim2()
    {
        if (start < 11)
        {
            start += Time.deltaTime;
        }

        if (players[2].transform.position.x > 1.86f)
        {
            //プレイヤーを前進させる
            players[2].transform.Translate(Vector3.left * 3 * Time.deltaTime);
            battery.transform.Translate(Vector3.right * 3 * Time.deltaTime);

            //バッテリーを置く
            if (players[2].transform.position.x < 1.86f)
            {
                StartRotation();

                energy[0].SetActive(false);
                energy[1].SetActive(true);
                conveyorAnim.speed = 0;//コンベアアニメーションを止める
            }
        }

        if (start > 7f)
        {
            players[2].transform.Translate(Vector3.left * 3 * Time.deltaTime);
            battery.transform.Translate(Vector3.right * 3 * Time.deltaTime);
            conveyorAnim.speed = 1;//コンベアアニメーションをスタート
        }

        //すべてをリセット
        if (players[2].transform.position.x < -13)
        {
            startAnim[1] = false;
            players[2].transform.position = new Vector3(19.79f, -1.26f, 0.78f);
            battery.transform.position = new Vector3(16.15f, -1.9f, 1.26f);
            players[2].transform.rotation = new Quaternion(0, 0, 0, 0);
            energy[0].SetActive(true);
            energy[1].SetActive(false);
            start = 0f;
            startAnim[2] = true;
        }

        //回転させるための処理
        if (isRotating)
        {
            float step = rotationSpeed * Time.deltaTime; // スムーズな回転
            players[2].transform.rotation = Quaternion.RotateTowards(players[2].transform.rotation, targetRotation, step);

            // 目標角度に十分近づいたら回転を終了
            if (Quaternion.Angle(players[2].transform.rotation, targetRotation) < 0.1f)
            {
                players[2].transform.rotation = targetRotation; // 完全に目標角度にスナップ
                isRotating = false; // 回転終了
                returnTimer = 0.5f; // 0.5秒待つためのタイマー設定
                isReturning = true; // 元に戻す準備
            }
        }

        //回転を元に戻す処理
        if (isReturning)
        {
            if (returnTimer > 0)
            {
                returnTimer -= Time.deltaTime; // 0.5秒の待機時間をカウントダウン
            }
            else
            {
                float step = rotationSpeed * Time.deltaTime; // 元の回転へのスムーズな回転
                players[2].transform.rotation = Quaternion.RotateTowards(players[2].transform.rotation, originalRotation, step);

                // 元の回転に戻ったら処理を終了
                if (Quaternion.Angle(players[2].transform.rotation, originalRotation) < 0.1f)
                {
                    players[2].transform.rotation = originalRotation; // 完全に元の角度にスナップ
                    isReturning = false; // 元に戻す処理終了
                }
            }
        }
    }

    //startAnim[2]がtrueの時に流す
    void ChangeAnim3()
    {
        if (players[3].transform.position.x > -2.05f && backTime == 0)
        {
            //プレイヤーを前進させる
            players[3].transform.Translate(Vector3.forward * 3 * Time.deltaTime);
            players[4].transform.Translate(Vector3.forward * 3 * Time.deltaTime);
            takeEnergyCore.transform.Translate(Vector3.left * 3 * Time.deltaTime);

            hasReachedPosition = false; // 停止状態をリセット
            isMovingAgain = false; // 再開もリセット
        }

        if (players[3].transform.position.x <= -2.05f && backTime == 0 && !hasReachedPosition)
        {
            conveyorAnim.speed = 0; // コンベアを停止
            hasReachedPosition = true; // 停止フラグを設定
        }

        // タイマーを動かす
        if (player4StartTime < 8f)
        {
            player4StartTime += Time.deltaTime;
        }

        // プレイヤー4の動き
        if (players[4].transform.position.x > -0.28f && player4StartTime > 6.1f && backTime == 0)
        {
            //エネルギーコアをとるプレイヤーだけを前進させる
            players[4].transform.Translate(Vector3.forward * 3 * Time.deltaTime);
        }

        // エネルギーコアの位置を調整
        if (players[4].transform.position.x > -0.7f && player4StartTime > 7f)
        {
            if (setTime == 0)
            {
                //エネルギーコアをplayer[4]の上に移動させる
                takeEnergyCore.transform.position = new Vector3(-0.017f, -0.42f, 0.78f);
            }
            if (setTime < 1f && startAnim[2])
            {
                setTime += Time.deltaTime;
                isMove[0] = true;
            }
            backTime += Time.deltaTime;
        }

        if (backTime > 0.5f && players[4].transform.position.x < 1.5f　&& isMove[0])
        {
            //プレイヤーが爆弾をとったら後ろに下がる
            players[4].transform.Translate(Vector3.back * 3 * Time.deltaTime);
            takeEnergyCore.transform.Translate(Vector3.right * 3 * Time.deltaTime);
        }

        // 次のアニメーションの準備
        if (backTime > 2)
        {
            isMove[0] = false;
            isMove[1] = true;
        }


        if (backTime > 2.5f && players[3].transform.position.x > -18 && isMove[1])
        {
            // プレイヤーの動きを再開
            players[3].transform.Translate(Vector3.forward * 3 * Time.deltaTime);
            players[4].transform.Translate(Vector3.forward * 3 * Time.deltaTime);
            takeEnergyCore.transform.Translate(Vector3.left * 3 * Time.deltaTime);

            if (!isMovingAgain)
            {
                conveyorAnim.speed = 1; // コンベアを再始動
                isMovingAgain = true; // 再開フラグを設定
            }
        }

        //すべてをリセット
        if (players[3].transform.position.x < -17.6f)
        {
            startAnim[2] = false;
            backTime = 0f;
            player4StartTime = 0f;
            setTime = 0f;
            players[3].transform.position = new Vector3(14.46f, -1.26f, 0.78f);
            players[4].transform.position = new Vector3(18f, -1.26f, 0.78f);
            takeEnergyCore.transform.position = new Vector3(14.75f, -0.42f, 0.78f);
            isMove[1] = false;
            startAnim[0] = true;
            hasReachedPosition = false;
            isMovingAgain = false;
        }
    }

        //コールチンを切り替える処理
        void StartAnimationForScene()
        {
            // 現在のコルーチンを停止
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);

                // 現在のUIのスケールを元に戻す
                GameObject currentUI = uiElements[sceneNum];
                currentUI.transform.localScale = Vector3.one;  // 元のスケールにリセット
            }

            // 選択された UI の初期スケールを取得
            GameObject targetUI = uiElements[sceneNum];
            Vector3 originalScale = targetUI.transform.localScale;

            // 新しいコルーチンを開始
            currentCoroutine = StartCoroutine(SelectUIScaleLoop(targetUI, originalScale));
        }


        //ScaleのUpとDownの処理
        IEnumerator SelectUIScaleLoop(GameObject targetUI, Vector3 originalScale)
        {
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
                    float scaleFactor = scaleCurve.Evaluate(t);
                    targetUI.transform.localScale = Vector3.Lerp(originalScale, targetScale, scaleFactor);

                    yield return null;
                }

                elapsedTime = 0f; // 経過時間をリセット
            }
        }


        //BombImageのスケールアップ
        IEnumerator BombScaleUp()
        {
            float timer = 0f;

            while (timer < bombScaleMaxTime)
            {
                timer += Time.deltaTime;
                float scaleTime = timer / bombScaleMaxTime;
                bombImageObject.transform.localScale = Vector3.Lerp(bombOriginalScale, tagetScale, scaleTime);

                yield return null;
            }

            bombImageObject.transform.localScale = tagetScale;
        }


    //爆発の回転処理
    IEnumerator RotateBombSmoothly(float targetAngle)
    {
        float rotationDuration = 2f; // 回転の所要時間
        float startAngle = bombUI.rotation.eulerAngles.z; // 現在の回転角度
        float timeElapsed = 0f;

        while (timeElapsed < rotationDuration)
        {
            // 補間で回転を計算
            float angle = Mathf.Lerp(startAngle, targetAngle, timeElapsed / rotationDuration);
            bombUI.rotation = Quaternion.Euler(0, 0, angle);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        bombUI.rotation = Quaternion.Euler(0, 0, targetAngle); // 最終的な回転角度を設定
    }
}
