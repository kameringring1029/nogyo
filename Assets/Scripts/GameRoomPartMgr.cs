using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System;

using General;
using Information;

public class GameRoomPartMgr : MonoBehaviour
{
    private GameObject cursorObj;
    private int nowCursorPosition = 0;
    private List<ACTION> actionList;

    private List<GameObject> roomSelectBtns;

    Text stateText;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    
    public void init()
    {
        stateText = gameObject.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.GetComponent<Text>();

        roomSelectBtns = new List<GameObject>();
        List<string> roomList = new List<string>( getRoomList());

        GameObject btnPref = Resources.Load<GameObject>("Prefab/ScrollViewButtonPrefab");
        cursorObj = Instantiate(Resources.Load<GameObject>("Prefab/wholecursor"), GameObject.Find("Canvas").transform);
        cursorObj.GetComponent<RectTransform>().localScale = cursorObj.GetComponent<RectTransform>().localScale / 2;


        //Content取得(ボタンを並べる場所)
        RectTransform content = gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).GetComponent<RectTransform>();

        //Contentの高さ決定
        //(ボタンの高さ+ボタン同士の間隔)*ボタン数
        float btnSpace = content.GetComponent<VerticalLayoutGroup>().spacing;
        float btnHeight = btnPref.GetComponent<LayoutElement>().preferredHeight;
        content.sizeDelta = new Vector2(0, (btnHeight + btnSpace) * roomList.Count);

        for (int no = 0; no < roomList.Count; no++)
        {
            //ボタン生成
            GameObject btn = (GameObject)Instantiate(btnPref, content);

            btn.GetComponent<RectTransform>().sizeDelta = new Vector2(btnPref.GetComponent<RectTransform>().sizeDelta[0]*2/3, btnPref.GetComponent<RectTransform>().sizeDelta[1] * 2);

            //ボタンのテキスト変更
            btn.transform.GetComponentInChildren<Text>().text = roomList[no];
            Debug.Log("room:"+ roomList[no]);

            //色変更
            btn.GetComponent<Image>().color = new Color(92 / 255f, 92 / 255f, 128 / 255f, 255 / 255f);

            //ボタンのクリックイベント登録
            int tempno = no;
            btn.transform.GetComponent<Button>().onClick.AddListener(() => selectRoom(tempno));

            roomSelectBtns.Add(btn);
        }

        nowCursorPosition = -1;
        moveCursor(0);

        stateText.text = "待合中のルーム：" + roomSelectBtns.Count + "件";

    }



    string[] getRoomList()
    {
        return GameObject.Find("Main Camera").GetComponent<WebsocketAccessor>().getRoomList();
    }

    

    //--- Menu中のカーソルを移動 ---//
    // 
    public void moveCursor(int vector)
    {
        
        nowCursorPosition += vector;

        // カーソル位置がオーバーフローしたとき
        if (nowCursorPosition < -1) nowCursorPosition = roomSelectBtns.Count - 1;
        if (nowCursorPosition > roomSelectBtns.Count - 1) nowCursorPosition = -1;

        Debug.Log("nowcursor:" + nowCursorPosition);

        for(int i=-1; i<roomSelectBtns.Count; i++)
        {
            GameObject btn;
            if (i == -1) // createroomボタン
            {
                btn = gameObject.transform.GetChild(1).gameObject;
            }
            else
            {
                btn = roomSelectBtns[i];
            }

            if (i == nowCursorPosition)
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

    //
    public void selectByCursor()
    {
        if(nowCursorPosition == -1)
        {
            StartCreateRoom();
        }
        else
        {
            selectRoom(nowCursorPosition);
        }
    }
    
    
    void selectRoom(int no)
    {
        GameObject.Find("Main Camera").GetComponent<WebsocketAccessor>().sendws("setPair;"+roomSelectBtns[no].transform.GetComponentInChildren<Text>().text);
    }



    public void StartCreateRoom()
    {
        GameObject.Find("Main Camera").GetComponent<WholeMgr>().startSelectMap();
    }

    public void createRoom(mapinfo map)
    {
        GameObject.Find("Main Camera").GetComponent<WebsocketAccessor>().sendws("createRoom;" + JsonUtility.ToJson(map));
        Destroy(cursorObj);

        stateText.text = "ルーム名：**" + null + "で待機中";
    }
}
