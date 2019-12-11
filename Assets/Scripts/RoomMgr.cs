using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using General;

public class RoomMgr : MonoBehaviour {

    public GameObject cursor;
    private Map map;

    // Use this for initialization
    void Start() {

        map = gameObject.GetComponent<Map>();
    }

    public void init()
    {
        Debug.Log("init room");

        //--- マップ生成 ---//
        TextAsset mapjson = Resources.Load("JSON/UranohoshiClub") as TextAsset;
        gameObject.GetComponent<Map>().positioningBlocks(JsonUtility.FromJson<Information.mapinfo>(mapjson.text));


        //--- Unit配置 ---//
        gameObject.GetComponent<Map>().positioningAllyUnits(new int[0]);

        // カーソルを味方ユニットの位置に移動
        cursor.GetComponent<cursor>().moveCursorToAbs(map.x_mass, map.y_mass);


       StartCoroutine( "moveUnits");
    }


    IEnumerator moveUnits()
    {

        yield return new WaitForSeconds(0.1f);

        // map,unitのルーム用設定変更
        map.settingforRoom();
        
        while (true)
        {
            // ユニットの選定
            int randunit = Random.Range(0, map.allyUnitList.Count);
            Unit unit = map.allyUnitList[randunit].GetComponent<Unit>();


            // ユニットの移動
            unit.viewMovableArea();

            int randposx = Random.Range(-2, 2);
            int randposy = Random.Range(-2, 2);

            if (unit.canMove(unit.nowPosition[0] + randposx, unit.nowPosition[1] + randposy))
                unit.changePosition(unit.nowPosition[0] + randposx, unit.nowPosition[1] + randposy, true);

            unit.deleteReachArea();

            // ユニットの向き変更
            int randflip = Random.Range(1, 10);
            if (randflip > 4) randflip = 1;
            else randflip = -1;
            unit.changeSpriteFlip(randflip);

             yield return new WaitForSeconds(1.5f);
            
        }
    }



    //++++++++++++++++++++++//
    //+++ 以下ボタン処理 +++//
    //++++++++++++++++++++++//

    //--- 十字ボタンが押されたときの挙動 ---//
    public void pushArrow(int x, int y)
    {
        Debug.Log("pushArrow");

        cursor.GetComponent<cursor>().moveCursor(x, y);
    }


    //--- Aボタンが押されたときの挙動 ---//
    public void pushA()
    {
        Debug.Log("pushA");
    }


    //--- Bボタンが押されたとき＝キャンセル処理 ---//
    public void pushB()
    {
        Debug.Log("pushB");

    }



    //--- フィールドブロックが選択されたとき ---//
    public void pushBlock(int x, int y)
    {

        Debug.Log("pushBlock");

        cursor.GetComponent<cursor>().moveCursorToAbs(x, y);
        pushA();
        
    }


    public void pushR()
    {

    }

    public void pushL()
    {
        float nowCameraSize = gameObject.GetComponent<Camera>().orthographicSize;

        if (nowCameraSize == 1.5f)
        {
            gameObject.GetComponent<Camera>().orthographicSize = 3;
        }
        else
        {
            gameObject.GetComponent<Camera>().orthographicSize = 1.5f;
        }

    }

    //++++++++++++++++++++++//
    //+++ 以上ボタン処理 +++//
    //++++++++++++++++++++++//

}
