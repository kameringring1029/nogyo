using System;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

using Information;
using General;
using System.Collections;

public class WebsocketAccessor : MonoBehaviour
{
    private WebSocket ws;
    private EnemyMgr EM;
    private GameMgr GM;

    private string roomlist; //ルームのリスト

    private List<WSStackItem> stack; //受信タスク用スタック

    private int playfirst;

    private void Start()
    {
        GM = gameObject.GetComponent<GameMgr>();
        EM = gameObject.GetComponent<EnemyMgr>();

        roomlist = "";
        stack = new List<WSStackItem>();
        playfirst = 1;

//        ws = new WebSocket("ws://localhost:8080/ws");
        ws = new WebSocket("ws://koke.link:8080/ws");

        // 接続開始時のイベント.
        ws.OnOpen += (sender, e) =>
        {
            Debug.Log("[WS]Opended");
        };
        // メッセージ受信時のイベント.
        ws.OnMessage += (sender, e) =>
        {
            Debug.Log("[WS]Received " + e.Data);

            // ルームリストの更新
            if (e.Data.IndexOf("rooms;") == 0)
            {
                Debug.Log("[WS]getRooms");
                roomlist = e.Data.Split(';')[1];
                return;
            }
            // ペア確定
            else if (e.Data.IndexOf("pair establish with;") == 0)
            {
                // タスクスタックに追加、WholeMgrのUpdateで処理
                stack.Add(new WSStackItem(WSITEMSORT.ESTROOM, e.Data.Split(';')[3]));

                playfirst = Int32.Parse( e.Data.Split(';')[2]);
            }
            // ユニット確定
            else if (e.Data.IndexOf("pairunits;") == 0)
            {
                // タスクスタックに追加、WholeMgrのUpdateで処理
                stack.Add(new WSStackItem(WSITEMSORT.ESTUNIT, e.Data.Split(';')[1]));
                
            }
            else {

                EM.enqRecvMsg(e.Data);

                /*
                switch (e.Data)
                {
                    case "A":
                        GM.pushA();
                        break;
                    case "B":
                        GM.pushB();
                        break;
                    case "U":
                        GM.pushArrow(0,1);
                        break;
                    case "D":
                        GM.pushArrow(0,-1);
                        break;
                    case "R":
                        GM.pushArrow(1,0);
                        break;
                    case "L":
                        GM.pushArrow(-1,0);
                        // gameObject.GetComponent<ControllerButtons>().onClickLeft();
                        break;

                }
                */
            }
        };
        ws.OnError += (sender, e) =>
        {
            Debug.Log("[WSERR]"+e.Message);
        };

        // 接続.
        ws.Connect();

        // メッセージ送信.
        if(ws.IsAlive)
            ws.Send("getRooms");
    }


    private void OnDestroy()
    {
        if (ws.IsAlive)
        {
            ws.Close();
            Debug.Log("[WS]close");
        }
    }

    // 受信、保管したルームリスト(String)をString[]化して渡す
    public string[] getRoomList()
    {        
        if(roomlist.Contains(","))
        {
            if(roomlist.Split(',')[0] != "")
                return roomlist.Split(',');
        }
        else if(roomlist != "")
        {
            return new string[] { roomlist };
        }

        return new string[0];
    }

    // 受信、保管した優先権をbool化して渡す
    public bool getPlayFirst()
    {
        switch (playfirst)
        {
            case 0: return false;
            default: return true;
        }
    }


    // 受信したタスクのスタックをpopして渡す
    public WSStackItem getStack()
    {
        if (stack != null)
        {
            if (stack.Count != 0)
            {
                Debug.Log("[WS]pop stack");
                WSStackItem s = stack[0];
                stack.RemoveAt(0);
                return s;
            }
        }

        return new WSStackItem();
    }


    // 任意メッセージの送信
    public void sendws(string message)
    {
        if (ws.IsAlive)
        {
            ws.Send(message);
            Debug.Log("[WS]send:"+message);
        }
        else
        {
            Debug.Log("[WS/ERR]isClosed...");
        }

    }
}

