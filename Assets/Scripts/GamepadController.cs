using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * ゲームパッド対応用のクラス
 */
public class GamepadController : MonoBehaviour
{

    float vertical, horizonal;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Debug.Log("Fire1");
            gameObject.GetComponent<ControllerButtons>().onClickB();
        }
        else if (Input.GetButtonDown("Fire2"))
        {
            Debug.Log("Fire2");
            gameObject.GetComponent<ControllerButtons>().onClickA();
        }
        else if (Input.GetButtonDown("Fire3"))
        {
            Debug.Log("Fire3");
        }
        else if (Input.GetButtonDown("Jump"))
        {
            Debug.Log("Jump");
        }
        else if (Input.GetAxisRaw("Horizontal") > 0)
        {
            if(horizonal == 0)
            {
                Debug.Log("h1");
                gameObject.GetComponent<ControllerButtons>().onClickLeft();
                horizonal = 1;
            }
        }
        else if (Input.GetAxisRaw("Horizontal") < 0)
        {
            if (horizonal == 0)
            {
                Debug.Log("h-1");
                gameObject.GetComponent<ControllerButtons>().onClickRight();
                horizonal = -1;
            }
        }
        else if (Input.GetAxisRaw("Vertical") > 0)
        {
            if (vertical == 0)
            {
                Debug.Log("v1");
                gameObject.GetComponent<ControllerButtons>().onClickUp();
                vertical = 1;
            }
        }
        else if (Input.GetAxisRaw("Vertical") < 0)
        {
            if (vertical == 0)
            {
                Debug.Log("v-1");
                gameObject.GetComponent<ControllerButtons>().onClickDown();
                vertical = -1;
            }
            else if(Input.GetButton("Fire1") && Input.GetButton("Fire2"))
            {
                    gameObject.GetComponent<ControllerButtons>().onClickStart();
            }
        }
        else
        {
            // ボタンが離されたら初期化（おしっぱだと連続判定されちゃうから変数で状態管理）
            horizonal = 0;vertical = 0;
        }

    }
}