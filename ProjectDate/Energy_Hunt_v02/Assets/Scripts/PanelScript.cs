using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerInput))]

public class PanelScript : MonoBehaviour
{
    [SerializeField] GameObject[] panel;

    int panelNum = 0;

    InputAction nextAction, returnAction, cancelAction, stickAction;

    private float timer;

    private bool isTimer = false;

    // Start is called before the first frame update
    void Start()
    {
        var input = GetComponent<PlayerInput>();

        var actionMap = input.currentActionMap;

        //nextAction = actionMap["Next"];
        //returnAction = actionMap["Return"];
        cancelAction = actionMap["Barrier"];
        stickAction = actionMap["Move"];
    }

    // Update is called once per frame
    void Update()
    {
        var stickAct = stickAction.ReadValue<Vector2>().x;

        if (stickAct > 0.2 && !isTimer)
        {
            isTimer = true;

            PanelChange(true);
        }
        if (stickAct < -0.2 && !isTimer)
        {
            isTimer = true;

            PanelChange(false);
        }
        if (cancelAction.triggered)
        {
            SceneManager.LoadScene("RuleScene");
        }

        if (isTimer)
        {
            timer += Time.deltaTime;

            if (timer > 0.4)
            {
                isTimer = false;

                timer = 0;
            }
        }
    }

    void PanelChange(bool next)
    {
        panel[panelNum].SetActive(false);

        if (next)
        {
            if (panelNum < panel.Length - 1)
            {
                panelNum++;
            }
            else
            {
                panelNum = 0;
            }
        }
        else
        {
            if (panelNum > 0)
            {
                panelNum--;
            }
            else
            {
                panelNum = panel.Length - 1;
            }
        }

        panel[panelNum].SetActive(true);
    }
}
