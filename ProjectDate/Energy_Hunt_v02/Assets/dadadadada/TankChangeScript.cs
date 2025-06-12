using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankChangeScript : MonoBehaviour
{
    public TankScript tankScript;
    [SerializeField] GameObject[] energy;
    public bool isChange = false;

    void Update()
    {
        if (isChange && Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = 0; i < tankScript.count && i < energy.Length; i++)
            {
                energy[i].SetActive(true);
            }

            for (int i = 0; i < tankScript.tentativeTank.Length; i++)
            {
                tankScript.tentativeTank[i].SetActive(false);
            }

            tankScript.currentIndex = 0;
        }

        if (isChange && !tankScript.tentativeTank[2].activeSelf && Input.GetKeyDown(KeyCode.F))
        {
            for (int i = energy.Length - 1; i >= 0; i--)
            {
                if (energy[i].activeSelf)
                {
                    energy[i].SetActive(false);

                    // 現在のインデックスの tentativeTank をアクティブ化
                    if (tankScript.currentIndex < tankScript.tentativeTank.Length)
                    {
                        // tentativeTank[currentIndex] をアクティブ化
                        tankScript.tentativeTank[tankScript.currentIndex].SetActive(true);

                        // 次のインデックスに進む
                        tankScript.currentIndex++;
                    }

                    // 処理が完了したらループを抜ける
                    break;
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Tank"))
        {
            isChange = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Tank"))
        {
            isChange = false;
        }
    }
}
