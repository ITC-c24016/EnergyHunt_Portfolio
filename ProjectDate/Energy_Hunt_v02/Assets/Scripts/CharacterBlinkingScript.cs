using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBlinkingScript : MonoBehaviour
{
    [SerializeField]
    [Header("点滅間隔")]
    float timeOut = 0.2f;

    float timeElapsed;//点滅間隔を計算するタイマー

    bool isVisible = true;//キャラクターのsetActiveをいじるbool

    [SerializeField] GameObject[] playerParts = new GameObject[3];//キャラクターのパーツを3つ入れる

    private bool isBlink = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isBlink)
        {
            CharacterBlinking();
        }
        /*
        else
        {
            foreach (var part in playerParts)
            {
                part.SetActive(true);
            }
            timeElapsed = 0f;
        }
        */
    }

    public void BlinkStart(bool set)
    {
        isBlink = set;
    }

    //キャラクターを点滅させる
    void CharacterBlinking()
    {
        timeElapsed += Time.deltaTime;

        if (timeElapsed >= timeOut)
        {
            isVisible = !isVisible; // ON/OFFを切り替え
            foreach (var part in playerParts)
            {
                part.SetActive(isVisible);
            }
            timeElapsed = 0f;
        }
    }
}
