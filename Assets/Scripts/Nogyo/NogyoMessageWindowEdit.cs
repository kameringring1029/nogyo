using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using Information;

public class NogyoMessageWindowEdit : MonoBehaviour
{
    NogyoEventScenario scenario;

    NogyoEventEditMgr.editend func;
    GameObject inputfield;
    GameObject textfield;
    GameObject talkersprite;
    GameObject talkerselecter;

    string talkernamestr;

    private void Start()
    {
        inputfield = GameObject.Find("InputField");
        inputfield.SetActive(false);
        talkerselecter = GameObject.Find("TalkerSelecter");
        talkerselecter.SetActive(false);
    }

    private void Update()
    {
        // TMProのテキストを更新
        if(textfield != null)
        {
            textfield.GetComponent<TextMeshProUGUI>().text = talkernamestr + scenario.message;
        }

    }

    public void init(NogyoEventScenario scenario, NogyoEventEditMgr.editend func = null)
    {
        this.scenario = scenario;
        if(func != null)
            this.func = func;

        talkersprite = gameObject.transform.GetChild(1).transform.GetChild(0).gameObject; //あほっぽいから修正したい
        textfield = gameObject.transform.GetChild(0).gameObject;


        renewTalker();
    }


    // 話者spriteをクリックしたとき
    public void onClickTalker()
    {
        Debug.Log("onClickTalker");
        if(talkerselecter.active == true)
        {
            talkerselecter.SetActive(false);
        }
        else
        {
            talkerselecter.SetActive(true);
        }
    }
    // 話者リストから選択したとき
    public void selectTalker(int id)
    {
        scenario.unitno = id;
        renewTalker();
    }

    // TMProフィールドをクリックしたとき
    public void onClickText()
    {
        Debug.Log("onClinkText");
        if (inputfield.active == true) // inputfieldが出ていたら消す
        {
            inputfield.SetActive(false);
        }
        else // inputfieldが出ていなかったら出現させてinit
        {
            inputfield.SetActive(true);
            inputfield.GetComponent<WebGLNativeInputField>().text = scenario.message;
            inputfield.GetComponent<WebGLNativeInputField>().ActivateInputField();
        }

    }

    // TMProフィールドのテキストを更新
    public void renewText(string text)
    {
        scenario.message = inputfield.GetComponent<WebGLNativeInputField>().text;
    }

    // 話者情報更新
    public void renewTalker()
    {
        // Sprite
        talkersprite.GetComponent<Image>().sprite =  GameObject.Find("player" + scenario.unitno).GetComponent<SpriteRenderer>().sprite;

        // メッセージ中の話者表示名
        talkernamestr = NogyoProfileUtil.getProfile(scenario.unitno).name();
        Color color = NogyoProfileUtil.getProfile(scenario.unitno).color();
        talkernamestr = "<color=#" + ColorUtility.ToHtmlStringRGB(color) + ">" + talkernamestr + "</color>: ";
    }


    // 変更の反映
    public void onClickEditEnd()
    {
        Debug.Log("onClickEditEnd");
        func(scenario);
        Destroy(gameObject);
    }
}
