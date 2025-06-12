using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CountdownScript : MonoBehaviour
{
    public float timer = 3;
    [SerializeField] GameObject countdownImageObject;
    [SerializeField] Image countdownImage;
    [SerializeField] Sprite[] newSprite;
    [SerializeField] GameObject startImage;
    AnimationCurve animationCurve;
    float scaleChangeTime = 1f;
    float startScaleChageTime = 0.3f;
    Vector3 originalScale;
    Vector3 targetScale;
    bool isScaling = false; //重複してコルーチンを呼ばないようにする
    bool isFalse = false;//スタートイメージを消すためのbool

    void Start()
    {
        animationCurve = new AnimationCurve(
            new Keyframe(0f, 0f),
            new Keyframe(0.5f, 1f),
            new Keyframe(1f, 0f)
            );       

        countdownImageObject.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);//スケールアップのアニメーションをするためにスケールを小さくする
        startImage.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);//スケールアップのアニメーションをするためにスケールを小さくする
        originalScale = countdownImageObject.transform.localScale;//最初のスケールを保存
        targetScale = new Vector3(1, 1, 1);//このスケールに向かって大きくする

        StartCoroutine(NumberScaleChange());//カウントダウンをスタート
    }

    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }

        //カウントダウンの処理
        if (timer < 2)
        {
            countdownImage.sprite = newSprite[0];
            if (!isScaling) StartCoroutine(NumberScaleChange());
        }
        if (timer < 1)
        {
            countdownImage.sprite = newSprite[1];
            if (!isScaling) StartCoroutine(NumberScaleChange());
        }
        if (timer < 0 && !isFalse)
        {
            countdownImageObject.SetActive(false);
            startImage.SetActive(true);
            StartCoroutine(StartScaleUp());
        }
        else
        {
            startImage.SetActive(false);
        }
    }

    //カウントダウンのスケールチェンジ
    IEnumerator NumberScaleChange()
    {
        isScaling = true; // コルーチンの重複実行を防ぐ
        float time = 0f;

        while (time < scaleChangeTime)
        {
            time += Time.deltaTime;
            float t = time / scaleChangeTime;
            float scaleFactor = animationCurve.Evaluate(t);//アニメーションcurveに値を代入
            countdownImageObject.transform.localScale = Vector3.Lerp(originalScale, targetScale, scaleFactor);
            yield return null;
        }

        countdownImageObject.transform.localScale = originalScale;
        isScaling = false;
    }

    IEnumerator StartScaleUp()
    {
        float timer = 0f;
        while(timer < startScaleChageTime)
        {
            timer += Time.deltaTime;
            float scaleChangeTime = timer / startScaleChageTime;
            startImage.transform.localScale = Vector3.Lerp(originalScale, targetScale, scaleChangeTime);

            yield return null;
        }
        startImage.transform.localScale = targetScale;//スケールを保存

        yield return new WaitForSeconds(0.5f);//0.5秒待って下の処理を実行

        isFalse = true;//trueにしてスタートイメージをfalseにする
    }
}
