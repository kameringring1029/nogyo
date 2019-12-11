using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using General;

public class cursor : MonoBehaviour {

    private GameObject Camera;
    private GameMgr GM;
    private EditMapMgr EM;
    private Map map;

    public int[] nowPosition = new int[2];
    
    private Vector3 endCamPosForMove; //カーソル位置にシームレスにカメラを移動するための移動先指定


    // Use this for initialization
    void Start () {
        Camera = GameObject.Find("Main Camera");
        GM = Camera.GetComponent<GameMgr>();
        EM = Camera.GetComponent<EditMapMgr>();
        map = Camera.GetComponent<Map>();

        nowPosition[0] = 0;
        nowPosition[1] = 0;
        
        endCamPosForMove = new Vector3(0, 0, 0);
    }
	
	// Update is called once per frame
	void Update () {

        // 指定の位置（カーソルの位置）にシームレスにカメラを移動する
        Vector3 startCamPosForMove = Camera.GetComponent<Transform>().position;
        if(startCamPosForMove != endCamPosForMove)
        {
            Camera.GetComponent<Transform>().position = Vector3.Lerp(startCamPosForMove, endCamPosForMove, 0.5f);
        }

    }

    // 相対移動（現在の座標から）
    public void moveCursor(int x, int y)
    {
        // Map外への移動は禁止
        if (nowPosition[0] + x < 0 || nowPosition[1] + y < 0 ||
            nowPosition[0] + x > map.x_mass * 2 - 1 || nowPosition[1] + y > map.y_mass * 2 - 1)
            return;

        // マップ上カーソルの相対移動（現在の座標から）
        gameObject.GetComponent<Transform>().position = gameObject.GetComponent<Transform>().position + new Vector3((x - y)/2.0f, -(x / 4.0f + y / 4.0f), 0);

        // カメラ位置の移動先をカーソル位置に指定
        endCamPosForMove = gameObject.GetComponent<Transform>().position + new Vector3(0, 0, -10);

        nowPosition[0] = nowPosition[0] + x;
        nowPosition[1] = nowPosition[1] + y;

        if(GM.enabled == true && GM.gameScene !=SCENE.STORY)
         Camera.GetComponent<GameMgr>().changeInfoWindow();
        else if(EM.enabled == true)
         Camera.GetComponent<EditMapMgr>().changeInfoWindow();
        
        //Debug.Log(nowPosition[0] + "/" + nowPosition[1]);
    }

    // 絶対座標移動
    public void moveCursorToAbs(int X, int Y)
    {
        gameObject.GetComponent<Transform>().position = 
            Camera.GetComponent<Map>().FieldBlocks[X, Y].GetComponent<Transform>().position;

        // カメラ位置の移動先をカーソル位置に指定
        endCamPosForMove = gameObject.GetComponent<Transform>().position + new Vector3(0, 0, -10);

        nowPosition[0] = X;
        nowPosition[1] = Y;

        if (GM.enabled == true && GM.gameScene != SCENE.STORY)
            Camera.GetComponent<GameMgr>().changeInfoWindow();
    }


    //--- カーソルをユニットの位置に移動 ---//
    public void moveCursolToUnit(GameObject unit)
    {
        moveCursorToAbs(
            unit.GetComponent<Unit>().nowPosition[0],
            unit.GetComponent<Unit>().nowPosition[1]);
        
    }



    private int abs(int a)
    {
        if (a < 0) a = a * (-1);
        return a;
    }
}
