using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager_Test : MonoBehaviour
{
    float[] scoreTimer = new float[4];
    public int[] playerScore = new int[4];
    public bool[] bomer = new bool[4];
    public Text[] scoreText;
    public RectTransform[] playerImages; // プレイヤーのイメージ（RectTransform）

    void Start()
    {

    }

    void Update()
    {
        // 爆弾所持時のスコア増加処理
        UpdatePlayerScore(0); // 1P
        UpdatePlayerScore(1); // 2P
        UpdatePlayerScore(2); // 3P
        UpdatePlayerScore(3); // 4P

        // スコアに基づいてイメージの位置を更新
        UpdatePlayerImagePositions();
    }

    void UpdatePlayerScore(int playerNum)
    {
        if (bomer[playerNum])
        {
            scoreTimer[playerNum] += Time.deltaTime;
            if (scoreTimer[playerNum] >= 1)
            {
                scoreTimer[playerNum] = 0;

                playerScore[playerNum]++;
                scoreText[playerNum].text = "" + playerScore[playerNum];
            }
        }
        else
        {
            scoreTimer[playerNum] = 0;
        }
    }

    void UpdatePlayerImagePositions()
    {
        // スコアとプレイヤーのインデックスをペアにしてソート
        List<(int score, int playerIndex)> scoreRankings = new List<(int score, int playerIndex)>();
        for (int i = 0; i < playerScore.Length; i++)
        {
            scoreRankings.Add((playerScore[i], i));
        }

        // スコアの降順にソート
        scoreRankings.Sort((a, b) => b.score.CompareTo(a.score));

        // 順位に応じてイメージの位置を更新
        int[] yPositions = { -100, -300, -500, -700 }; // 各順位のY座標
        for (int rank = 0; rank < scoreRankings.Count; rank++)
        {
            int playerIndex = scoreRankings[rank].playerIndex;
            RectTransform playerImage = playerImages[playerIndex];

            // XとYの座標を更新
            playerImage.anchoredPosition = new Vector2(-120, yPositions[rank]);
        }
    }

    // 他プレイヤーに爆弾の爆発を当てたときのスコア増加
    public void IncreaseByBomb(int playerNum)
    {
        playerScore[playerNum] += 10;
        scoreText[playerNum].text = "" + playerScore[playerNum];
    }

    // 爆弾の爆発に巻き込まれたときのスコア半減
    public void DecreaseByBomb(int playerNum)
    {
        playerScore[playerNum] -= playerScore[playerNum] / 3;
        scoreText[playerNum].text = "" + playerScore[playerNum];
    }

    // 爆弾を持っているとき
    public void HaveBomb(int playerNum)
    {
        bomer[playerNum] = true;
    }

    // 爆弾を手放したとき
    public void MissingBomb(int playerNum)
    {
        bomer[playerNum] = false;
    }
}
