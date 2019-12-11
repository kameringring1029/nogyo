using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using System;

/*
 * メッセージウィンドウのテキストをゆっくり表示するやつ
 */

public class MessageManager : MonoBehaviour
{
    string talkername;
    string messageText = "";
    int nowchar = 9999;

    public float timeOut = 0.02f; // 文字表示周期[s]
    private float timeElapsed;
    
    void Start()
    {
        
    }

    // timeOut秒ごとにmessageTextから一文字ずつ表示する
    void Update()
    {
        timeElapsed += Time.deltaTime;

        if (nowchar == 9999) return;
        
        if (nowchar == 0)
        {

            gameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = talkername;
            messageText = talkername + messageText;
            nowchar = nowchar + talkername.Length;
        }
        else if (timeElapsed >= timeOut && messageText != "" && messageText.Length > nowchar)
        {
            nowchar++;
            gameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = SubstringAtCount(messageText, nowchar)[0];

            timeElapsed = 0.0f;
        }
        else if (messageText.Length == nowchar)
        {
            nowchar++;

            // メッセージ終わったよ表示
            gameObject.transform.GetChild(2).gameObject.SetActive(true);
        }
    }

    // 表示する文字列の設定&各種初期化
    public void setText(string talkername, string message)
    {
        messageText = message + "";
        this.talkername = talkername;
        
        nowchar = 0;
        timeElapsed = 0;
        gameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "";
        gameObject.transform.GetChild(2).gameObject.SetActive(false);

    }


    // stringを任意の文字数で分割する
    // http://baba-s.hatenablog.com/entry/2015/03/19/140748
    public static string[] SubstringAtCount(string self, int count)
    {
        var result = new List<string>();
        var length = (int)Math.Ceiling((double)self.Length / count);

        for (int i = 0; i < length; i++)
        {
            int start = count * i;
            if (self.Length <= start)
            {
                break;
            }
            if (self.Length < start + count)
            {
                result.Add(self.Substring(start));
            }
            else
            {
                result.Add(self.Substring(start, count));
            }
        }

        return result.ToArray();
    }
}
