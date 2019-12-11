using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using General;
using Information;

/*
 * アプリケーション全体のマネージャ
 * スタートメニューから選択されて動作
 * Game:ユニット選択→ゲームマネージャ呼び出し
 * Room：ルームマネージャ呼び出し
 */

public class WholeMgr : MonoBehaviour {

    public WHOLEMODE wholemode = WHOLEMODE.SELECTMODE;

    private int wholecursor = 1;

    private GameObject wholecursorObj; 
    private GameObject startMenuPanel;
    private GameObject selectUnitPanel;

    public UnitSelect unitSelect;
    
    private GameObject mapList;

    private GameObject GameRoomPanel;

    private void Start()
    {
        gameObject.GetComponent<Camera>().orthographicSize = 1.5f;

        startMenuPanel = Instantiate(Resources.Load<GameObject>("Prefab/UI/StartMenuPanel"), GameObject.Find("Canvas").transform);

        wholecursorObj = Instantiate(Resources.Load<GameObject>("Prefab/wholecursor"), GameObject.Find("Canvas").transform);
        wholecursorObj.GetComponent<RectTransform>().position =
            GameObject.Find("StartGameButton").GetComponent<RectTransform>().position + new Vector3(0-GameObject.Find("StartGameButton").GetComponent<RectTransform>().sizeDelta[0]/5,0,0);


    }

    private void Update()
    {
        // Websocketのタスクスタックを確認して処理
        if (gameObject.GetComponent<WebsocketAccessor>().enabled)
        {
            WSStackItem s = gameObject.GetComponent<WebsocketAccessor>().getStack();

            switch (s.sort)
            {
                case WSITEMSORT.NONE:
                    break;
                case WSITEMSORT.ESTROOM:
                    Destroy(GameObject.Find("LoadingPanel(Clone)"));
                    Destroy(GameRoomPanel);
                    gameObject.GetComponent<GameMgr>().setMapInfo(s.map);
                    selectUnits();
                    break;
                case WSITEMSORT.ESTUNIT:
                    Destroy(GameObject.Find("LoadingPanel(Clone)"));
                    gameObject.GetComponent<GameMgr>().setPairUnitIdArray(s.units);
                    startGame();
                    break;
            }
        }

    }


    // モード選択ボタンから呼び出し
    public void selectMode(WHOLEMODE mode)
    {
        Destroy(startMenuPanel);
        Destroy(wholecursorObj);

        wholemode = mode;

        switch (mode)
        {
            case WHOLEMODE.GAME:
                gameObject.GetComponent<GameMgr>().enabled = true;
                startSelectMap();
                break;

            case WHOLEMODE.ROOM:
                initRoom();
                break;

            case WHOLEMODE.MAPEDIT:
                gameObject.GetComponent<EditMapMgr>().enabled = true;
                startSelectMap();
                break;

            case WHOLEMODE.SELECT_GAMEROOM:
                gameObject.GetComponent<GameMgr>().enabled = true;
                gameObject.GetComponent<WebsocketAccessor>().enabled = true;
                startSelectGameRoom();
                break;
        }

    }

    // マップ選択を開始（MAPリストパネルをインスタンス化）
    public void startSelectMap()
    {
        mapList = Instantiate(Resources.Load<GameObject>("Prefab/UI/MapList"), GameObject.Find("Canvas").transform);
        wholemode = WHOLEMODE.SELECTMAP;
    }


    // 通信対戦用のルーム選択を開始
    public void startSelectGameRoom()
    {
        GameRoomPanel = Instantiate(Resources.Load<GameObject>("Prefab/UI/GameRoomPanel"), GameObject.Find("Canvas").transform);
        StartCoroutine(initGameRoom());
    }

    IEnumerator initGameRoom()
    {
        yield return new WaitForSeconds(0.5f);
        GameRoomPanel.GetComponent<GameRoomPartMgr>().init();
    }
    

    // MapListから呼び出し
    public void selectMap(mapinfo map)
    {
        Debug.Log("MapSelected");

        // SRPGのときとMapエディタモードのときで分岐
        if (GameObject.Find("CreateRoomButton"))
        {
            GameRoomPanel.GetComponent<GameRoomPartMgr>().createRoom(map);
        }
        else if(gameObject.GetComponent<GameMgr>().enabled == true)
        {
                gameObject.GetComponent<GameMgr>().setMapInfo(map);
                selectUnits();
        }
        else if (gameObject.GetComponent<EditMapMgr>().enabled == true)
        {
            wholemode = WHOLEMODE.MAPEDIT;
            gameObject.GetComponent<EditMapMgr>().startEditMap(map);
            Destroy(wholecursorObj);
        }

        Destroy(mapList);
    }

    //--- SRPGスタート ---//
    private void selectUnits()
    {
        selectUnitPanel = Instantiate(Resources.Load<GameObject>("Prefab/UI/UnitSelectPanel"), GameObject.Find("Canvas").transform);
        wholemode = WHOLEMODE.SELECTUNIT;
        unitSelect = selectUnitPanel.GetComponent<UnitSelect>();
        unitSelect.init();

   }

    // ユニット選択画面を消去、選択されたユニットをGameMgrに渡す
    public void startGame()
    {
        Destroy(selectUnitPanel);
        gameObject.GetComponent<GameMgr>().setUnitIdArray(unitSelect.selectedUnits.ToArray());

        Destroy(wholecursorObj);

        wholemode = WHOLEMODE.GAME;
        gameObject.GetComponent<GameMgr>().startGame();
    }



    //---Roomスタート ---//
    private void initRoom()
    {
        Destroy(wholecursorObj);
        wholemode = WHOLEMODE.ROOM;
        StartCoroutine("startRoom");
    }

    IEnumerator startRoom()
    {
        gameObject.GetComponent<RoomMgr>().enabled = true;
        yield return new WaitForSeconds(0.1f);
        gameObject.GetComponent<RoomMgr>().init();
    }

    

    //++++++++++++++++++++++//
    //+++ 以下ボタン処理 +++//
    //++++++++++++++++++++++//

    public void pushArrow(int horizon, int vertical)
    {
        wholecursor += horizon + vertical;

        switch (wholemode)
        {
            case WHOLEMODE.SELECTMODE:

                // カーソルのオーバーフロー処理
                if(wholecursor < 1)
                {
                    wholecursor = startMenuPanel.GetComponent<Transform>().childCount;
                }else if(wholecursor > startMenuPanel.GetComponent<Transform>().childCount)
                {
                    wholecursor = 1;
                }

                // カーソルの移動
                // 全ボタンの色を暗く
                for (int i = 0; i < startMenuPanel.GetComponent<Transform>().childCount; i++)
                {
               //     startMenuPanel.GetComponent<Transform>().GetChild(i).gameObject.GetComponent<Image>().color
               //         = new Color(155.0f / 255.0f, 155.0f / 255.0f, 155.0f / 255.0f, 255f);
                }

                // オンカーソルのボタンを明るく、カーソルの位置をボタンへ移動
                GameObject nowbutton = startMenuPanel.GetComponent<Transform>().GetChild(wholecursor - 1).gameObject;
             //   nowbutton.GetComponent<Image>().color = new Color(255f, 255f, 255f, 255f);
                wholecursorObj.GetComponent<RectTransform>().position =
                    nowbutton.GetComponent<RectTransform>().position + new Vector3(0 - nowbutton.GetComponent<RectTransform>().sizeDelta[0] / 5, 0, 0);
                               
                break;


            case WHOLEMODE.SELECTMAP:

                mapList.GetComponent<MapListUtil>().moveCursor(horizon, vertical);
                break;

            case WHOLEMODE.SELECT_GAMEROOM:

                GameRoomPanel.GetComponent<GameRoomPartMgr>().moveCursor(horizon + vertical);
                break;


            case WHOLEMODE.GAME:
                // カーソルのオーバーフロー処理

                // カーソルの移動

                break;

        }
    }

    public void pushA()
    {

        switch (wholemode)
        {
            case WHOLEMODE.SELECTMODE:
                // カーソルの決定
                if (wholecursor == 1)
                {
                    selectMode(WHOLEMODE.GAME);
                }
                else if (wholecursor == 2)
                {
                    selectMode(WHOLEMODE.ROOM);
                }
                else if (wholecursor == 3)
                {
                    selectMode(WHOLEMODE.MAPEDIT);
                }
                else if (wholecursor == 4)
                {
                    selectMode(WHOLEMODE.SELECT_GAMEROOM);
                }

                break;


            case WHOLEMODE.SELECTMAP:
                mapList.GetComponent<MapListUtil>().selectMap();
                break;

            case WHOLEMODE.SELECT_GAMEROOM:
                GameRoomPanel.GetComponent<GameRoomPartMgr>().selectByCursor();
                    break;

            case WHOLEMODE.GAME:
                

                break;

        }
    }

    public void pushB()
    {

    }

    public void pushR()
    {

    }

    public void pushL()
    {

    }

}
