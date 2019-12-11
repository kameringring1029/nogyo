using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Information;
using General;


/*
 * SRPGゲーム中のMap生成とMap情報管理用
 */

public class Map : MonoBehaviour
{
    private WHOLEMODE mapsettingtype;

    // マップ情報
    public mapinfo mapinformation;
    public int x_mass, y_mass;
    public GameObject[,] FieldBlocks;
    private int[,] mapstruct;
    public GameObject cursor;
    public GameObject mapframe;

    // マップ上のユニット情報
    public List<GameObject> allyUnitList = new List<GameObject>();
    public List<GameObject> enemyUnitList = new List<GameObject>();



    public void positioningBlocks(mapinfo mapInfo)
    {
        // map情報の読み込み 

        mapinformation = mapInfo;
        x_mass = (int)System.Math.Sqrt(mapinformation.mapstruct.Length)/2;
        y_mass = (int)System.Math.Sqrt(mapinformation.mapstruct.Length)/2;

        mapframe.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Map/mapframe/" + mapinformation.frame);

        Debug.Log(mapinformation.mapstruct.Length + " " + x_mass + " " + y_mass);


        // map作成
        FieldBlocks = new GameObject[x_mass * 2, y_mass * 2];

        for (int x = 0; x < x_mass * 2; x++)
        {
            for (int y = 0; y < y_mass * 2; y++)
            {
                setBlock(mapinformation.mapstruct[y * y_mass * 2 + x], x, y);
                //Debug.Log(FieldBlocks[x, y]);
            }
        }
        
    }


    public void setBlock(int blockid, int x, int y)
    {
        if (FieldBlocks[x, y]) Destroy(FieldBlocks[x, y]);
        
        Vector3 position = new Vector3((x - y)/2.0f, y_mass - y / 4.0f - x / 4.0f, 0);

        FieldBlocks[x, y] = Instantiate(MapInfoUtil.getBlockTypebyid(blockid), position, transform.rotation);
        FieldBlocks[x, y].GetComponent<FieldBlock>().position[0] = x;
        FieldBlocks[x, y].GetComponent<FieldBlock>().position[1] = y;

        FieldBlocks[x, y].name = x + "_" + y + "_block";

        // Sprite反転情報の適用
        if (blockid > 101 && blockid < 104) FieldBlocks[x, y].GetComponent<SpriteRenderer>().flipX = true;

        // map上の表示順の設定
        int distance = (abs(x) + abs(y));
        // 障害物 or ユニット初期配置ブロックの場合、Spriteが上に飛び出るのでSotingをUnitに合わせる
        if (FieldBlocks[x, y].GetComponent<FieldBlock>().blocktype == GROUNDTYPE.UNMOVABLE || blockid<0) distance += 100;
        FieldBlocks[x, y].GetComponent<SpriteRenderer>().sortingOrder = distance;

        if (mapsettingtype == WHOLEMODE.SELECTMAP || mapinformation.frame == "map_frame")
            FieldBlocks[x, y].GetComponent<SpriteRenderer>().enabled = true;
    }




    public void positioningAllyUnits(int[] units)
    {
        if(units.Length == 0)
        {
            units = UnitStatusUtil.randunit(2);
        }

        for (int i=0;i<units.Length; i++)
        {
            Debug.Log("positioningAllyUnits unitid :" + units[i]);
            setUnitFromId(units[i], CAMP.ALLY);
        }
        
    }


    public void positioningEnemyUnits(int[] units)
    {

        for(int i=0; i<units.Length; i++)
        {
            setUnitFromId(units[i], CAMP.ENEMY);
        }


    }



    //--- 指定したunitidのユニットを配置 ---//
    public void setUnitFromId(int unitid, CAMP camp)
    {
        int[] position = getNextUnitInitPosition(camp);

        statusTable status = UnitStatusUtil.search(unitid);
        GameObject jobprefab = UnitStatusUtil.searchJobPrefab(status.job_id());

        positioningUnit(position[0], position[1], jobprefab, status, camp);
        
    }


    //--- 次に配置するユニットの初期位置を取得 ---//
    // jsonから読み込んだmap情報に既定の初期位置が含まれる
    // camp: ユニットの陣営
    // return: 初期位置（X,Y）
    // 
    private int[] getNextUnitInitPosition(CAMP camp)
    {
        string positionstr = "";
        int[] position = new int[2];

        switch (camp)
        {
            case CAMP.ALLY:
                positionstr = mapinformation.ally[allyUnitList.Count];
                position[0] = int.Parse(positionstr.Split('-')[0]);
                position[1] = int.Parse(positionstr.Split('-')[1]);
                return position;
                
            case CAMP.ENEMY:
                positionstr = mapinformation.enemy[enemyUnitList.Count];
                position[0] = int.Parse(positionstr.Split('-')[0]);
                position[1] = int.Parse(positionstr.Split('-')[1]);
                return position;
        }

        return null;
    }




    //--- ユニット配置 ---//
    // x,y:初期位置
    // jobprefab:ジョブのぷれふぁぶ
    // status:ステータス
    private void positioningUnit(int x, int y, GameObject jobprefab, statusTable status, CAMP camp)
    {
        List<GameObject> unitlist = new List<GameObject>();

        switch (camp)
        {
            case CAMP.ALLY: unitlist = allyUnitList; break;
            case CAMP.ENEMY: unitlist = enemyUnitList; break;
        }

        unitlist.Add(Instantiate(jobprefab, new Vector3(0, 0, 0), transform.rotation));
        unitlist[unitlist.Count - 1].GetComponent<Unit>().init(x, y, camp, status);
    }



    //--- 最も近い位置にいるAllyユニットを検索 ---//
    // selectedunit:検索対象
    // return: 最も近くにいるAllyユニット
    public GameObject getNearAllyUnit(GameObject selectedunit)
    {
        GameObject nearestunit = allyUnitList[0];

        for(int i=1; i<allyUnitList.Count; i++)
        {
            int distanceA = abs(selectedunit.GetComponent<Unit>().nowPosition[0] - allyUnitList[i].GetComponent<Unit>().nowPosition[0])
                                + abs(selectedunit.GetComponent<Unit>().nowPosition[1] - allyUnitList[i].GetComponent<Unit>().nowPosition[1]);
            int distanceB = abs(selectedunit.GetComponent<Unit>().nowPosition[0] - nearestunit.GetComponent<Unit>().nowPosition[0])
                                + abs(selectedunit.GetComponent<Unit>().nowPosition[1] - nearestunit.GetComponent<Unit>().nowPosition[1]);

            if (distanceA < distanceB) nearestunit = allyUnitList[i];
        }

        return nearestunit;
    }


    //--- ルーム用の設定 ---//
    // ユニットのランダム配置、移動可能範囲の不可視化、歩行スピードの減速
    public void settingforRoom()
    {
        mapsettingtype = WHOLEMODE.ROOM;

        // int unitnum = allyUnitList.Count;
        
        int unitnum = 1;

        for (int i = 0; i < unitnum; i++)
        {
            Unit unit = allyUnitList[i].GetComponent<Unit>();

            unit.movableAreaPrefab.GetComponent<SpriteRenderer>().enabled = false;
            unit.staticMoveVelocity = 1;

            int randx = Random.Range(1, 1); // UranohoshiClub用の設定
            int randy = Random.Range(1, 1); // UranohoshiClub用の設定

            // ランダム用の設定
            /*
            int randx = Random.Range(0, x_mass * 2 - 1);
            int randy = Random.Range(0, y_mass * 2 - 1);
            while (FieldBlocks[randx, randy].GetComponent<FieldBlock>().blocktype != GROUNDTYPE.NORMAL)
            {
                randx = Random.Range(0, x_mass * 2 - 1);
                randy = Random.Range(0, y_mass * 2 - 1);
            }
            */

            unit.changePosition(randx, randy, false);
        }

    

        cursor.GetComponent<SpriteRenderer>().color = new Color(255f, 255f, 255f, 0);
        cursor.GetComponent<SpriteRenderer>().sortingOrder = 100;

        GameObject.Find("mapframe").GetComponent<SpriteRenderer>().sortingOrder = 0;

        // カメラを引きに
        gameObject.GetComponent<Camera>().orthographicSize = 3;
    }

    //--- SRPGゲーム用の設定 ---//
    // 移動可能範囲の可視化、歩行スピードの設定
    public void settingforGame()
    {
        mapsettingtype = WHOLEMODE.GAME;

        for (int i = 0; i < allyUnitList.Count; i++)
        {
            Unit unit = allyUnitList[i].GetComponent<Unit>();

            unit.movableAreaPrefab.GetComponent<SpriteRenderer>().enabled = true;
            unit.staticMoveVelocity = 3;

        }

        cursor.GetComponent<SpriteRenderer>().color = new Color(0, 0, 255f);
        cursor.GetComponent<SpriteRenderer>().sortingOrder = 100;
        cursor.GetComponent<Animator>().enabled = true;

        GameObject.Find("mapframe").GetComponent<SpriteRenderer>().sortingOrder = 0;

        // カメラを寄りに
        gameObject.GetComponent<Camera>().orthographicSize = 1.5f;
    }

    //--- MapEdit用の設定 ---//
    public void settingforEditMap()
    {
        mapsettingtype = WHOLEMODE.SELECTMAP;

        cursor.GetComponent<SpriteRenderer>().color = new Color(255f, 255f, 255f);
        cursor.GetComponent<SpriteRenderer>().sortingOrder = 999;
        cursor.GetComponent<Animator>().enabled = false;

       //GameObject.Find("mapframe").GetComponent<SpriteRenderer>().sortingOrder = 998;

        // カメラを引きに
        gameObject.GetComponent<Camera>().orthographicSize = 3;


        // Unitの初期配置位置の表示をうちっちーに変更
        for (int i=0; i<mapinformation.ally.Length; i++)
        {
            setBlock(0,
                int.Parse(mapinformation.ally[i].Split('-')[0]),
                int.Parse(mapinformation.ally[i].Split('-')[1]));
        }

        // Enemyの初期配置位置の表示を変更
        for (int i = 0; i < mapinformation.enemy.Length; i++)
        {
            setBlock((-1)*(int.Parse(mapinformation.enemy[i].Split('-')[2])),
                int.Parse(mapinformation.enemy[i].Split('-')[0]),
                int.Parse(mapinformation.enemy[i].Split('-')[1]));
        }
    }





    private int abs(int a)
    {
        if (a < 0) a = a * (-1);
        return a;
    }

}
