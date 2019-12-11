using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Information;
using General;
using UnityEngine.Networking;
using System.Text;
using UnityEngine.UI;

public class NogyoEventLoadSave : MonoBehaviour
{

    /* delegate用のやつ */
    public delegate void serverEndWrapper(List<NogyoEvent> x);


    // サーバからロード
    public void getEventFromServer(serverEndWrapper func)
    {
        StartCoroutine(this.getEventFromServerProcess(func));
    }
    IEnumerator getEventFromServerProcess(serverEndWrapper func)
    {
        yield return new WaitForSeconds(0.1f);

        List<NogyoEvent> nogyoevents = new List<NogyoEvent>();

        Debug.Log("reques from server");

        UnityWebRequest request = UnityWebRequest.Get("https://koke.link:3000/nogyo/scenario/getjson/all");
        // 下記でも可
        // UnityWebRequest request = new UnityWebRequest("http://example.com");
        // methodプロパティにメソッドを渡すことで任意のメソッドを利用できるようになった
        // request.method = UnityWebRequest.kHttpVerbGET;

        // リクエスト送信
        yield return request.SendWebRequest();

        // 通信エラーチェック
        if (request.isNetworkError)
        {
            Debug.Log(request.error);

            nogyoevents = getEventFromLocal();
        }
        else
        {
            if (request.responseCode == 200)
            {
                // UTF8文字列として取得する
                string text = request.downloadHandler.text;
                //
                text = text.Replace("\\", "");

                Debug.Log("success request! result:" + text);

                // jsonをパースしてListに格納
                // jsonutilityそのままだと配列をパースできないのでラッパを使用 https://qiita.com/akira-sasaki/items/71c13374698b821c4d73
                NogyoEvent[] eventarray;
                eventarray = JsonUtilityHelper.MapFromJson<NogyoEvent>(text);

                for (int i = 0; i < eventarray.Length; i++)
                {
                    nogyoevents.Add(eventarray[i]);
                }

            }
        }

        // 設定
        func(nogyoevents);

    }


    // ローカルからロード
    public static List<NogyoEvent> getEventFromLocal()
    {
        // jsonをパースしてListに格納
        // jsonutilityそのままだと配列をパースできないのでラッパを使用 https://qiita.com/akira-sasaki/items/71c13374698b821c4d73


        List<NogyoEvent> nogyoevents = new List<NogyoEvent>();

        // JSONフォルダからの読み込み
        TextAsset[] jsons = Resources.LoadAll<TextAsset>("JSON/Nogyo/Event");

        foreach (TextAsset j in jsons)
        {
            string fsontext = j.text;
            NogyoEvent e = JsonUtility.FromJson<NogyoEvent>(fsontext);
            nogyoevents.Add(e);
        }

        Debug.Log(nogyoevents[0].scenarioarrays.Length);

        return nogyoevents;

    }



    public void saveOnServer(NogyoEvent data)
    {
        StartCoroutine(saveOnServerProcess(data));
    }

    IEnumerator saveOnServerProcess(NogyoEvent data)
    {
        GameObject mapsavelogtext;

        UnityWebRequest request = new UnityWebRequest("https://koke.link:3000/nogyo/scenario/savejson", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.responseCode == 200)
        {
          //  mapsavelogtext.GetComponent<Text>().text = mapinformation.name + "がサーバに保存されました";
        }
        else
        {
          //  mapsavelogtext.GetComponent<Text>().text = "サーバに接続できませんでした\ncode:" + request.responseCode;
        }
        Debug.Log("Status Code: " + request.responseCode);


        Destroy(gameObject);
    }

}
