using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour
{
    public int[] playerScore = new int[4];

    [SerializeField] Text[] scoreText;
    
    void Start()
    {
        for(int i = 0; i < 4; i++)
        {
            Debug.Log(playerScore[i]);
            scoreText[i].text = "" + playerScore[i];
        }
    }

    
    void Update()
    {
        
    }
}
