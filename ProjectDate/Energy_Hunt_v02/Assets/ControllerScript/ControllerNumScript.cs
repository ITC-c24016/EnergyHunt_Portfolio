using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerNumScript : MonoBehaviour
{
    public int[] controllerNum = new int[4]; // コントローラー番号
    private bool[] isAssigned = new bool[4]; // ボタンが割り当て済みかどうか

    void Update()
    {
        // スイッチコントローラー (P1AButton, P2AButton)
        if (Input.GetButtonDown("P1AButton") && !isAssigned[0]) // P1
        {
            AssignToSwitchController(0); //割り当て
        }

        if (Input.GetButtonDown("P2AButton") && !isAssigned[1]) // P2
        {
            AssignToSwitchController(1); //割り当て
        }

        // プレステコントローラー (P3AButton, P4AButton)
        if (Input.GetButtonDown("P3AButton") && !isAssigned[2]) // P3
        {
            AssignToPlayStationController(2); //割り当て
        }

        if (Input.GetButtonDown("P4AButton") && !isAssigned[3]) // P4
        {
            AssignToPlayStationController(3); //割り当て
        }
    }

    // スイッチコントローラー
    private void AssignToSwitchController(int buttonIndex)
    {
        for (int i = 0; i < 2; i++) // スイッチ用スロットのみ (0, 1)
        {
            if (controllerNum[i] == 0) // 空いているスロットを探す
            {
                controllerNum[i] = i + 1; // スロットに番号を割り当て
                isAssigned[buttonIndex] = true; // ボタンを割り当て済みに設定
                return;
            }
        }

        Debug.Log("No available slot for Switch Controller.");
    }

    // プレステコントローラー
    private void AssignToPlayStationController(int buttonIndex)
    {
        for (int i = 2; i < 4; i++) // プレステ用スロットのみ (2, 3)
        {
            if (controllerNum[i] == 0) // 空いているスロットを探す
            {
                controllerNum[i] = i + 1; // スロットに番号を割り当て
                isAssigned[buttonIndex] = true; // ボタンを割り当て済みに設定
                return;
            }
        }
    }
}
