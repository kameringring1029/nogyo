using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Information;

/*
 * マップエディットモードを制御するやつ
 * 
 */ 

public class EditMapMgr : MonoBehaviour {

    public GameObject cursor;
    private Map map;
    private int nowblocktype = 0;

    private GameObject infoPanel;
    public GameObject btnPref;  //ボタンプレハブ

    private GameObject mapSavePanel;

    private List<mapinfo> maps;

    private List<int[]> allypos = new List<int[]>();
    private List<int[]> enemypos = new List<int[]>();



    // Use this for initialization
    void Start() {

        cursor = GameObject.Find("cursor");
        map = gameObject.GetComponent<Map>();
    }

	
	// Update is called once per frame
	void Update () {
		
	}


    

    public void startEditMap(mapinfo mapinfo)
    {
        Debug.Log("EditMap");

        Debug.Log(mapinfo);

        //--- マップ生成 ---//
        gameObject.GetComponent<Map>().positioningBlocks(mapinfo);
        gameObject.GetComponent<Map>().settingforEditMap();

        infoPanel = Instantiate(Resources.Load<GameObject>("Prefab/infoPanel"), GameObject.Find("Canvas").transform);

        // カーソルのSprite変更
        nowblocktype = 2;
        cursor.GetComponent<SpriteRenderer>().sprite
            = MapInfoUtil.getBlockTypebyid(nowblocktype).GetComponent<SpriteRenderer>().sprite;
        cursor.GetComponent<cursor>().moveCursorToAbs(map.x_mass, map.y_mass);


        // allypos, enemyposでUnitの初期配置情報を所持する
        // 元のMapの初期配置情報をこれらにコピー
        for (int i = 0; i < mapinfo.ally.Length; i++)
        {
            allypos.Add(new int[] { int.Parse( mapinfo.ally[i].Split('-')[0]),
                int.Parse(mapinfo.ally[i].Split('-')[1])});
        }
        for (int i = 0; i < mapinfo.enemy.Length; i++)
        {
            enemypos.Add(new int[] { int.Parse(mapinfo.enemy[i].Split('-')[0]),
                int.Parse(mapinfo.enemy[i].Split('-')[1]),
                int.Parse(mapinfo.enemy[i].Split('-')[2])});
        }

    }


    // 編集中のMApを保存する保存用パネルのActive化とmap情報の引き渡し
    public void saveMap()
    {

        mapSavePanel = Instantiate(Resources.Load<GameObject>("Prefab/UI/MapSavePanel"),GameObject.Find("Canvas").transform);

        // allypos, enemyposでUnitの初期配置情報を所持しているので
        // Map情報にこれらを埋め込んで引き渡し
        mapinfo tmpmap = map.mapinformation;

        List<string> tmplist = new List<string>();
        for (int i = 0; i < allypos.Count; i++)
            tmplist.Add(allypos[i][0].ToString() + "-" + allypos[i][1].ToString());
        tmpmap.ally = tmplist.ToArray();

        tmplist = new List<string>();
        for (int i = 0; i < enemypos.Count; i++)
            tmplist.Add(enemypos[i][0].ToString() + "-" + enemypos[i][1].ToString() + "-" + enemypos[i][2].ToString());
        tmpmap.enemy = tmplist.ToArray();


        GameObject.Find("MapSaveInputField").GetComponent<MapSave>().init(tmpmap);

    }
    


    //--- 今のBlock上のアイテムを確認し表示に反映 ---//
    public void changeInfoWindow()
    {
        int[] nowCursolPosition = new int[2];

        nowCursolPosition[0] = cursor.GetComponent<cursor>().nowPosition[0];
        nowCursolPosition[1] = cursor.GetComponent<cursor>().nowPosition[1];

        infoPanel.GetComponent<DisplayInfo>().displayBlockInfo(map.FieldBlocks[nowCursolPosition[0], nowCursolPosition[1]].GetComponent<FieldBlock>());
        
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

        int x = cursor.GetComponent<cursor>().nowPosition[0];
        int y = cursor.GetComponent<cursor>().nowPosition[1];


        // Unitの初期配置がされているか確認し、されていれば消去
        for(int i=0; i<allypos.Count; i++)
            if (x == allypos[i][0] && y == allypos[i][1])
                allypos.RemoveAt(i);
        for (int i = 0; i <enemypos.Count; i++)
            if (x == enemypos[i][0] && y == enemypos[i][1])
                enemypos.RemoveAt(i);


        if (nowblocktype > 0) // 通常ブロックを配置する場合
        {
            map.mapinformation.mapstruct[y * map.y_mass * 2 + x] = nowblocktype;
        }
        else // unitの初期配置ブロックを配置する場合
        {
            // unitの初期配置情報を更新
            if (nowblocktype == 0) {
                allypos.Add(new int[] { x, y });
            }
            else
            {
                enemypos.Add(new int[] { x, y, (-1) * nowblocktype });
            }


            map.mapinformation.mapstruct[y * map.y_mass * 2 + x] = 1;
        }

        map.setBlock(nowblocktype, x, y);
    }


    //--- Bボタンが押されたとき ---//
    public void pushB()
    {
        Debug.Log("pushB");

        if (!GameObject.Find("MapSaveInputField"))
        {
            saveMap();
        }
        else
        {
             Destroy(mapSavePanel);
        }

        
        //Application.LoadLevel("Main"); // Reset
    }


    //--- フィールドブロックが選択されたとき ---//
    public void pushBlock(int x, int y)
    {

        Debug.Log("pushBlock");

        cursor.GetComponent<cursor>().moveCursorToAbs(x, y);
        
    }


    public void pushR()
    {

        if (nowblocktype > 100) nowblocktype -= 100; // 向きが変更されている場合

        nowblocktype = MapInfoUtil.getNextBlockTypebyid(nowblocktype);

        cursor.GetComponent<SpriteRenderer>().sprite 
            = MapInfoUtil.getBlockTypebyid(nowblocktype).GetComponent<SpriteRenderer>().sprite;

        // 敵配置ブロックなら反転
        if(nowblocktype < 0)
            cursor.GetComponent<SpriteRenderer>().flipX = true;
        else 
            cursor.GetComponent<SpriteRenderer>().flipX = false;

    }

    public void pushL()
    {
        // ブロックの向きの変更
        if(nowblocktype > 101)
        {
            nowblocktype -= 100;
            cursor.GetComponent<SpriteRenderer>().flipX = false;
        }
        else if(nowblocktype > 1 && nowblocktype < 4)
        {
            nowblocktype += 100;
            cursor.GetComponent<SpriteRenderer>().flipX = true;

        }


    }

    //++++++++++++++++++++++//
    //+++ 以上ボタン処理 +++//
    //++++++++++++++++++++++//


}
