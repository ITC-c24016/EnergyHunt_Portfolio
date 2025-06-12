using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankScript : MonoBehaviour
{
    [SerializeField] float timer = 0f;
    public GameObject[] tentativeTank;
    public bool haveBomb;

    public int currentIndex = 0; // 現在操作中のオブジェクトのインデックス
    private float maxScaleY = 0.7f; // 最大スケール値
    private float duration = 3f;   // スケールが最大になるまでの時間

    public int count = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (haveBomb && currentIndex < tentativeTank.Length)
        {
            // 現在のタンクをアクティブにする（最初の1回だけ実行）
            if (!tentativeTank[currentIndex].activeSelf)
            {
                tentativeTank[currentIndex].SetActive(true);
            }

            // タイマーの進行度に応じてスケールを補間
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / duration);
            Vector3 currentScale = tentativeTank[currentIndex].transform.localScale;
            tentativeTank[currentIndex].transform.localScale = new Vector3(
                currentScale.x,
                Mathf.Lerp(0f, maxScaleY, progress),
                currentScale.z
            );

            if (timer >= duration)
            {
                // タイマーが3秒に達したら次のオブジェクトをアクティブにする
                currentIndex++;

                if (currentIndex < tentativeTank.Length)
                {
                    tentativeTank[currentIndex].SetActive(true);
                }

                // カウントは次のオブジェクトをアクティブにした後に1増やす
                if (currentIndex > 0) // 最初のタンクがアクティブ化された後からカウントを増やす
                {
                    count++;
                }

                // タイマーをリセット
                timer = 0f;
            }
        }
    }
}
