using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Information;
using General;
using UnityEngine.UI;
using UnityEngine.Networking;

/*
 * Map情報を取得してリスト表示するためのUtilityクラス
 *
 */

public class MapListUtil : MonoBehaviour
{

    private List<mapinfo> mapinfos; //取得したマップ情報を格納するリスト

    private List<GameObject> mapBtnList;

    int nowCursorPosition;
    GameObject cursorObj;

    void Start()
    {
        cursorObj = Instantiate(Resources.Load<GameObject>("Prefab/wholecursor"), GameObject.Find("Canvas").transform);
        cursorObj.GetComponent<RectTransform>().localScale = cursorObj.GetComponent<RectTransform>().localScale / 2;

        getMapsFromServer();
    }

    public void getMapsFromServer()
    {
        StartCoroutine("getMapsFromServerProcess");
    }

    // Map情報をサーバから取得
    IEnumerator getMapsFromServerProcess()
    {

        mapinfos = new List<mapinfo>();

        Debug.Log("request maps from server");

        UnityWebRequest request = UnityWebRequest.Get("http://koke.link:3000/llsrpg/map/getjson/all");
        // 下記でも可
        // UnityWebRequest request = new UnityWebRequest("http://example.com");
        // methodプロパティにメソッドを渡すことで任意のメソッドを利用できるようになった
        // request.method = UnityWebRequest.kHttpVerbGET;

        // リクエスト送信
        yield return request.Send();

        // 通信エラーチェック
        if (request.isNetworkError)
        {
            Debug.Log(request.error);

            getMapsFromLocal();
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
                mapinfo[] maparray;
                maparray = JsonUtilityHelper.MapFromJson<mapinfo>(text);

                for (int i = 0; i < maparray.Length; i++)
                {
                    mapinfos.Add(maparray[i]);
                }

                // UIのMapリストを設定
                setMapList();
            }
        }
        
    }


    // Map情報をローカルファイルから取得
    public void getMapsFromLocal()
    {
        Debug.Log("getmapfromlocal");

        mapinfos = new List<mapinfo>();

        // JSONフォルダからの読み込み
        TextAsset[] json = Resources.LoadAll<TextAsset>("JSON/");

        foreach (TextAsset mapjson in json)
        {
            string maptext = mapjson.text;
            mapinfo map = JsonUtility.FromJson<mapinfo>(maptext);
            mapinfos.Add(map);
        }

        // UIのMapリストを設定
        setMapList();
    }


    
    // Mapリストを取得してリストUIに反映
    //
    public void setMapList()
    {
        mapBtnList = new List<GameObject>();

        GameObject btnPref = Resources.Load<GameObject>("Prefab/ScrollViewButtonPrefab");

        //Content取得(ボタンを並べる場所)
        RectTransform content = GameObject.Find("MapListContent").GetComponent<RectTransform>();

        //Contentの高さ決定
        //(ボタンの高さ+ボタン同士の間隔)*ボタン数
        float btnSpace = content.GetComponent<VerticalLayoutGroup>().spacing;
        float btnHeight = btnPref.GetComponent<LayoutElement>().preferredHeight;
        content.sizeDelta = new Vector2(0, (btnHeight + btnSpace) * mapinfos.Count);

        for (int no = 0; no < mapinfos.Count; no++)
        {

            //ボタン生成
            GameObject btn = (GameObject)Instantiate(btnPref);
            btn.name = no + "_" + btn.name;

            //ボタンをContentの子に設定
            btn.transform.SetParent(content, false);

            //ボタンのテキスト変更
            btn.transform.GetComponentInChildren<Text>().text = no + ": " + mapinfos[no].name.ToString();
            btn.GetComponent<Image>().color = new Color(192 / 255f, 192 / 255f, 228 / 255f, 192 / 255f);

            //ボタンのクリックイベント登録
            int tempno = no;
            btn.transform.GetComponent<Button>().onClick.AddListener(() => selectMap(tempno));

            mapBtnList.Add(btn);
        }

        nowCursorPosition = -1;
        moveCursor(0, 1);
    }


    //--- Menu中のカーソルを移動 ---//
    public void moveCursor(int horizon, int vertical)
    {
        if(vertical != 0)
        {
            nowCursorPosition += vertical;

            // カーソル位置がオーバーフローしたとき
            if (nowCursorPosition < 0) nowCursorPosition = mapBtnList.Count - 1;
            if (nowCursorPosition > mapBtnList.Count - 1) nowCursorPosition = 0;

            Debug.Log("nowcursor:" + nowCursorPosition);

            foreach (GameObject btn in mapBtnList)
            {
                if (btn == mapBtnList[nowCursorPosition])
                {
                    btn.GetComponent<Image>().color = new Color(192 / 255f, 192 / 255f, 228 / 255f, 255 / 255f);
                    cursorObj.GetComponent<RectTransform>().position =
                        btn.GetComponent<RectTransform>().position + new Vector3(0 - btn.GetComponent<RectTransform>().sizeDelta[0] / 5, 0, 0);

                }
                else
                {
                    btn.GetComponent<Image>().color = new Color(192 / 255f, 192 / 255f, 228 / 255f, 192 / 255f);
                }
            }

        }
        else if (horizon != 0) //ページ送り//
        {
            switch (horizon)
            {
                case 1:
                    scrollToNext();
                    moveCursor(0, 1);
                    break;
                case -1:
                    scrollToPrev();
                    moveCursor(0, -1);
                    break;
            }
        }
    }

    public void selectMap(int no = -1)
    {
        if (no == -1) no = nowCursorPosition;
        Destroy(cursorObj);
        GameObject.Find("Main Camera").GetComponent<WholeMgr>().selectMap(mapinfos[no]);
    }


    //-- --//
    public void scrollToNext()
    {
        RectTransform content = GameObject.Find("MapListContent").GetComponent<RectTransform>();
        for (int i = 0; i < 5; i++)
            content.GetChild(0).SetAsLastSibling();
    }

    //-- --//
    public void scrollToPrev()
    {
        RectTransform content = GameObject.Find("MapListContent").GetComponent<RectTransform>();
        for (int i = 0; i < 5; i++)
            content.GetChild(content.childCount - 1).SetAsFirstSibling();
    }

}
