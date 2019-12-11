using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//マウスオーバーイベント制御
using UnityEngine.EventSystems;
//Javascript呼び出し制御
using System.Runtime.InteropServices;

//マウスオーバーインタフェースを追加
public class TweetButtonController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    //Javascriptの呼び出し
    [DllImport("__Internal")]
    private static extern void WindowOpenEventOn(string url);
    [DllImport("__Internal")]
    private static extern void WindowPopUpEventOn(string url);
    [DllImport("__Internal")]
    private static extern void WindowEventOff();

    //Twitter投稿内容管理
    private string twitterURL;
    private string tweetText;

    //初回実行
    void Start()
    {

        //TwitterのツイートインテントURLを設定
        this.twitterURL = "https://twitter.com/intent/tweet?text=";
        //ツイート内容の初期化
        this.tweetText = null;
        //デバッグ用
        InputTweetText("てすと #Unity");

    }
    //ツイート内容を登録
    public void InputTweetText(string text)
    {
        this.tweetText = text;
        Debug.Log(this.tweetText);
    }

    //カーソルが入った
    public void OnPointerEnter(PointerEventData eventData)
    {

        Debug.Log("MouseEnter!");

#if UNITY_WEBGL
        //WebGLPlayer以外ではビルドしない
        //WebGLPlayerでのみ実行(Unity上で実行するとエラーになるため)
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            //別のタブで投稿するクリックイベント登録
            // WindowOpenEventOn(this.twitterURL + WWW.EscapeURL(this.tweetText));
            //別の窓で投稿するクリックイベント登録
            WindowPopUpEventOn(this.twitterURL + WWW.EscapeURL(this.tweetText));
        }
#endif
    }

    //カーソルが離れた
    public void OnPointerExit(PointerEventData eventData)
    {

        Debug.Log("MouseExit!");
#if UNITY_WEBGL
        //WebGLPlayer以外ではビルドしない
        //WebGLPlayerでのみ実行(Unity上で実行するとエラーになるため)
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            //クリックイベントを削除
            WindowEventOff();
        }
#endif
    }

}