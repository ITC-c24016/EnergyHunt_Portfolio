using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerInput))]

public class RuleSceneScript : MonoBehaviour
{
    bool first;

    [SerializeField] GameObject[] image;

    int select = 0;

    InputAction decideAction, cancelAction, upAction, downAction, stickAction;

    private float timer;

    private bool isTimer = false;

    private AnimationCurve animationCurve;

    private Coroutine currentCoroutine;

    float maxScale = 1.1f;

    float maxTime = 1f;

    // Start is called before the first frame update
    void Start()
    {
        var input = GetComponent<PlayerInput>();

        var actionMap = input.currentActionMap;

        decideAction = actionMap["PickUp"];
        cancelAction = actionMap["Barrier"];
        //upAction = actionMap["Up"];
        //downAction = actionMap["Down"];
        stickAction = actionMap["Move"];

        animationCurve = new AnimationCurve(
            new Keyframe(0f,0f),
            new Keyframe(0.5f,1f),
            new Keyframe(1f,0f));
        
        image[select].SetActive(true);
        StartAnimationForScene();
    }

    // Update is called once per frame
    void Update()
    {
        var stickAct = stickAction.ReadValue<Vector2>().y;

        if (stickAct > 0.2 /*&& isTimer*/)
        {
            //isTimer = true;

            SelectChange(true);
        }
        else if (stickAct < -0.2 /*&& !isTimer*/)
        {
            //isTimer = true;

            SelectChange(false);
        }

        if (decideAction.triggered)
        {
            Debug.Log("シーン");
            switch (select)
            {
                case 0:
                    SceneManager.LoadScene("RuleScene_Main");

                    break;

                case 1:
                    SceneManager.LoadScene("RuleScene_Action");

                    break;
            }
        }
        
        if (cancelAction.triggered)
        {
            SceneManager.LoadScene("TitleScene");
        }

        if (isTimer)
        {
            timer += Time.deltaTime;

            if (timer > 0.2)
            {
                isTimer = false;

                timer = 0;
            }
        }
    }


    void SelectChange(bool up)
    {
        image[select].SetActive(false);

        if (up)
        {
            if (select > 0)
            {
                select--;
                StartAnimationForScene();
            }
        }
        else
        {
            if (select < image.Length - 1)
            {
                select++;
                StartAnimationForScene();
            }
        }
        image[select].SetActive(true);
    }

    void StartAnimationForScene()
    {
        // 現在のコルーチンを停止
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);

            // 現在のUIのスケールを元に戻す
            GameObject currentUI = image[select];
            currentUI.transform.localScale = Vector3.one;  // 元のスケールにリセット
        }

        // 選択された UI の初期スケールを取得
        GameObject targetUI = image[select];
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
                float scaleFactor = animationCurve.Evaluate(t);
                targetUI.transform.localScale = Vector3.Lerp(originalScale, targetScale, scaleFactor);

                yield return null;
            }

            elapsedTime = 0f; // 経過時間をリセット
        }
    }
}
