using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Information;
using General;
using UnityEngine.UI;
using UnityEngine.Networking;

/*
 * SRPGゲーム部分のマネージャ
 * WholeMgrからinitされて開始
 */

public class GameMgr : MonoBehaviour
{

    private Map map;
    private int[] unitIdArray;
    private int[] pairUnitIdArray;
    private mapinfo gameMapinfo;

    public GameObject btnPref;  //ボタンプレハブ
    private List<mapinfo> mapinfos;

    public GameObject unitMenuPanel;
    public GameObject cursor;
    private cursor cursorComp;

    private GameObject infoPanel;
    private GameObject sceneBanner;


    // マップ上のユニット情報
    public GameObject selectedUnit;

    // 現在のシーン情報
    //  ユニット移動先選択中、ユニット行動中、敵ターン中など
    //  シーンに応じてボタンが押された時の処理を変更
    public CAMP gameTurn { get; private set; }
    public SCENE gameScene { get; private set; }
    private SCENE preScene;

    public bool playFirst;

    public ACTION selectedAction;

    // Use this for initialization
    void Start ()
    {

        map = gameObject.GetComponent<Map>();

        cursorComp = cursor.GetComponent<cursor>();

        playFirst = true;
    }

    public void setUnitIdArray(int[] units)
    {
        unitIdArray = units;

        Debug.Log("init gm");

    }
    public void setPairUnitIdArray(int[] units)
    {
        pairUnitIdArray = units;
    }

    public void setMapInfo(mapinfo mapinfo)
    {
        gameMapinfo = mapinfo;
    }


    public void startGame()
    {

        // ストーリー付きの場合
        if (gameMapinfo.mapscenarioarrays != null)
        {
            if(gameMapinfo.mapscenarioarrays.Length > 0)
            {
                StartCoroutine(fortest());
                return;
            }
        }

        settingSRPG();
        startSRPG();

    }

    IEnumerator fortest()
    {
        GameObject loadpanel = Instantiate(Resources.Load<GameObject>("Prefab/LoadingPanel"), GameObject.Find("Canvas").transform);

        //yield return new WaitForSeconds(2.5f);
        yield return new WaitForSeconds(0.5f);
        Destroy(loadpanel);

        settingStory();

    }


    private void settingStory()
    {
        setGameScene(SCENE.STORY);

        //--- マップ生成 ---//
        gameObject.GetComponent<Map>().positioningBlocks(gameMapinfo);

        //--- map,unitのSRPG用設定 ---//
        map.settingforGame();

        gameObject.GetComponent<StoryMgr>().init(gameMapinfo, unitIdArray);
    }

    private void settingSRPG()
    {
        //--- マップ生成 ---//
        gameObject.GetComponent<Map>().positioningBlocks(gameMapinfo);
        
        infoPanel = Instantiate(Resources.Load<GameObject>("Prefab/infoPanel"), GameObject.Find("Canvas").transform);
        sceneBanner = Instantiate(Resources.Load<GameObject>("Prefab/UI/SceneBanner"), GameObject.Find("Canvas").transform);

        //
        int[] enemyunitIdArray = new int[map.mapinformation.enemy.Length];
        for (int i = 0; i < map.mapinformation.enemy.Length; i++)
        {
            enemyunitIdArray[i] = int.Parse(map.mapinformation.enemy[i].Split('-')[2]);
        }

        //
        if (pairUnitIdArray != null) enemyunitIdArray = pairUnitIdArray;

        //
        if (gameObject.GetComponent<WebsocketAccessor>().enabled == true)
            playFirst = gameObject.GetComponent<WebsocketAccessor>().getPlayFirst();
        if (!playFirst)
        {
            string[] tmp = gameObject.GetComponent<Map>().mapinformation.ally;
            gameObject.GetComponent<Map>().mapinformation.ally = gameObject.GetComponent<Map>().mapinformation.enemy;
            gameObject.GetComponent<Map>().mapinformation.enemy = tmp;            
        }

        //--- Unit配置 ---//

        gameObject.GetComponent<Map>().positioningAllyUnits(unitIdArray);
        gameObject.GetComponent<Map>().positioningEnemyUnits(enemyunitIdArray);


        // カーソルを味方ユニットの位置に移動
        cursor.GetComponent<cursor>().moveCursolToUnit(map.allyUnitList[map.allyUnitList.Count - 1]);

    }


    public void startSRPG()
    {
        //--- map,unitのSRPG用設定 ---//
        map.settingforGame();

        if (infoPanel == null)
            infoPanel = Instantiate(Resources.Load<GameObject>("Prefab/infoPanel"), GameObject.Find("Canvas").transform);

        // 先攻か後攻か
        switch (playFirst)
        {
            case true:
                gameTurn = CAMP.ENEMY;
                gameScene = SCENE.MAIN;
                break;
            case false:
                gameTurn = CAMP.ALLY;
                gameScene = SCENE.MAIN;
                break;
        }
        

        switchTurn();
    }
    

    //--- ターン切り替え ---//
    private void switchTurn()
    {
        //SCENE nextScene = gameScene;

        // 次のターン指定、カーソルをターンユニットに移動
        switch (gameTurn)
        {
            case CAMP.ALLY:
                gameTurn = CAMP.ENEMY;
                cursor.GetComponent<cursor>().moveCursolToUnit(map.enemyUnitList[map.enemyUnitList.Count - 1]);
                gameObject.GetComponent<EnemyMgr>().startEnemyTurn();
                break;

            case CAMP.ENEMY:
                gameTurn = CAMP.ALLY;
                cursor.GetComponent<cursor>().moveCursolToUnit(map.allyUnitList[map.allyUnitList.Count - 1]);
                break;

            case CAMP.GAMEMASTER:
                break;
        }

        // バナー表示後ターン移行
        sceneBanner.GetComponent<SceneBanner>().activate(gameTurn);
    }



    //--- ユニット除外処理 ---//
    public void removeUnitByList(GameObject unit)
    {
        if(unit.GetComponent<Unit>().camp == CAMP.ALLY)
        {
            map.allyUnitList.Remove(unit);
        }
        else if(unit.GetComponent<Unit>().camp == CAMP.ENEMY)
        {
            map.enemyUnitList.Remove(unit);
        }


        // 全滅判定
        if(map.allyUnitList.Count == 0)
        {
            Debug.Log("Lose...");
            gameTurn = CAMP.GAMEMASTER;
            switchTurn();
        }
        else if (map.enemyUnitList.Count ==0)
        {
            Debug.Log("Win!");
            gameTurn = CAMP.GAMEMASTER;
            switchTurn();
        }
    }



    //--- 今のBlock上のアイテムを確認し表示に反映 ---//
    public void changeInfoWindow()
    {
        int[] nowCursolPosition = new int[2];

        nowCursolPosition[0] = cursor.GetComponent<cursor>().nowPosition[0];
        nowCursolPosition[1] = cursor.GetComponent<cursor>().nowPosition[1];

        infoPanel.GetComponent<DisplayInfo>().displayBlockInfo(map.FieldBlocks[nowCursolPosition[0], nowCursolPosition[1]].GetComponent<FieldBlock>());
        
    }




    //--- ユニット移動完了時の処理---//
    public void endUnitMoving()
    {
        gameScene = SCENE.UNIT_MENU;
        changeInfoWindow();
        unitMenuPanel = Instantiate(Resources.Load<GameObject>("Prefab/UnitMenuPanel"), GameObject.Find("Canvas").transform);
        unitMenuPanel.GetComponent<UnitMenu>().init(selectedUnit.GetComponent<Unit>());
        unitMenuPanel.GetComponent<UnitMenu>().moveCursor(0, selectedUnit.GetComponent<Unit>());
        selectedUnit.GetComponent<Unit>().deleteReachArea();
    }

    //--- ユニット行動完了時の処理---//
    // 全操作ユニットの行動完了状態を確認し、完了済みだったらターン移行
    // forceend : 強制ターン終了時にTrueで呼ぶとよい
    public void endUnitActioning(bool forceend = false)
    {
        gameScene = SCENE.MAIN;

        List<GameObject> unitList = map.allyUnitList;
        if(gameTurn == CAMP.ALLY)
        {
            unitList = map.allyUnitList;
        }else if(gameTurn == CAMP.ENEMY)
        {
            unitList = map.enemyUnitList; 
        }

        // 全ユニットの行動権確認
        bool allUnitActioned = true;
        for (int i=0; i< unitList.Count; i++)
        {
            if (!(unitList[i].GetComponent<Unit>().isActioned))
            {
                allUnitActioned = false;
                cursor.GetComponent<cursor>().moveCursolToUnit(unitList[i]);

            }           
        }

        // 全ユニットが行動完了したらターン移行
        if (allUnitActioned || forceend)
        {
            switchTurn();
            for (int i = 0; i < unitList.Count; i++)
            {
                unitList[i].GetComponent<Unit>().restoreActionRight();
            }
        }
    }



    public void setGameScene(SCENE gameScene)
    {
        this.gameScene = gameScene;
    }


    //--- 演出の設定 ---//
    // trueの場合、演出中
    // falseの場合、演出解除
    public void setInEffecting(bool inEffecting)
    {
        if (inEffecting)
        {
            if(gameScene != SCENE.GAME_INEFFECT)
            {
                preScene = gameScene;
                gameScene = SCENE.GAME_INEFFECT;

            }
        }
        else
        {
            gameScene = preScene;
        }
    }


    //++++++++++++++++++++++//
    //+++ 以下ボタン処理 +++//
    //++++++++++++++++++++++//

    //--- 十字ボタンが押されたときの挙動 ---//
    public void pushArrow(int x, int y)
    {
        /* for websocket*/
        // 自分のターンならコマンド発信
        if (gameObject.GetComponent<WebsocketAccessor>().enabled == true && gameTurn == CAMP.ALLY && gameScene != SCENE.GAME_INEFFECT)
        {
            if (x == 0 && y == 1)
            {
                gameObject.GetComponent<WebsocketAccessor>().sendws("U");
            }
            else if (x == 0 && y == -1)
            {
                gameObject.GetComponent<WebsocketAccessor>().sendws("D");
            }
            else if (x == 1 && y == 0)
            {
                gameObject.GetComponent<WebsocketAccessor>().sendws("R");
            }
            else if (x == -1 && y == 0)
            {
                gameObject.GetComponent<WebsocketAccessor>().sendws("L");
            }
        }
        


        Debug.Log("pushArrow");

        switch (gameScene)
        {
            // 演出中につき操作不可
            case SCENE.GAME_INEFFECT:
            case SCENE.STORY:
                break;

            case SCENE.UNIT_MENU:
                unitMenuPanel.GetComponent<UnitMenu>().moveCursor(y, selectedUnit.GetComponent<Unit>());
                break;

            case SCENE.UNIT_ACTION_FORECAST:
                break;

            default:
                cursorComp.moveCursor(x, y);
                break;
        }

    }




    //--- Aボタンが押されたときの挙動 ---//
    // throughOtherButton: 他のボタン押下を経由しているかどうか
    public void pushA(bool throughOtherButton=false)
    {
        // 演出中なら何もしない
        if (gameScene == SCENE.GAME_INEFFECT) return;

        /* for websocket*/
        // 自分のターンならコマンド発信

        if (gameObject.GetComponent<WebsocketAccessor>().enabled == true && gameTurn == CAMP.ALLY && !throughOtherButton)
        {
            gameObject.GetComponent<WebsocketAccessor>().sendws("A");
        }
        
    
        Debug.Log("pushA");

        int[] nowCursolPosition = { cursor.GetComponent<cursor>().nowPosition[0], cursor.GetComponent<cursor>().nowPosition[1] };
        GameObject nowBlock = map.FieldBlocks[nowCursolPosition[0], nowCursolPosition[1]];
        GameObject groundedUnit = nowBlock.GetComponent<FieldBlock>().GroundedUnit;

        switch (gameScene)
        {
            // 演出中につき操作不可
            case SCENE.STORY:
                break;


            case SCENE.MAIN:
                // Unitが配置されていたら&&Unitが未行動だったら
                if (groundedUnit != null && !groundedUnit.GetComponent<Unit>().isActioned)
                {
                    gameScene = SCENE.UNIT_SELECT_MOVETO;
                    groundedUnit.GetComponent<Unit>().viewMovableArea();
                }
                break;

            case SCENE.UNIT_SELECT_MOVETO:
                // 自軍ユニットであり、選択先が移動可能範囲であれば移動
                if (selectedUnit.GetComponent<Unit>().camp == gameTurn) {
                    if (selectedUnit.GetComponent<Unit>().canMove(cursor.transform.position))
                    {
                        selectedUnit.GetComponent<Unit>().changePosition(nowCursolPosition[0], nowCursolPosition[1], true);
                    }
                }
                // 他軍ユニットだったら移動範囲表示を終了
                else
                {
                    gameScene = SCENE.MAIN;
                    selectedUnit.GetComponent<Unit>().deleteReachArea();
                }
                break;

            case SCENE.UNIT_MENU:
                Destroy(unitMenuPanel);

                selectedAction = unitMenuPanel.GetComponent<UnitMenu>().getSelectedAction();

                // 待機の場合
                if (selectedAction  == ACTION.WAIT)
                {
                    selectedUnit.GetComponent<Unit>().endAction();
                }
                // 待機ではない場合（対象アクション実施）
                else
                {
                    gameScene = SCENE.UNIT_SELECT_TARGET;
                    selectedUnit.GetComponent<Unit>().viewTargetArea();
                }

                break;

            case SCENE.UNIT_SELECT_TARGET:
                // ユニットがいる＆対象可能範囲であれば
                if (groundedUnit != null && selectedUnit.GetComponent<Unit>().canReach(cursor.transform.position))
                {
                    gameScene = SCENE.UNIT_ACTION_FORECAST;
                    infoPanel.GetComponent<DisplayInfo>().displayBattleInfo(selectedUnit, groundedUnit, selectedAction);
                }
                break;


            case SCENE.UNIT_ACTION_FORECAST:
                // ユニットがいる＆対象可能範囲であれば
                if (groundedUnit != null && selectedUnit.GetComponent<Unit>().canReach(cursor.transform.position))
                {
                    cursor.GetComponent<cursor>().moveCursolToUnit(selectedUnit);
                    selectedUnit.GetComponent<Unit>().doAction(groundedUnit, selectedAction);
                }
                break;

        }

    }


    //--- Bボタンが押されたとき＝キャンセル処理 ---//
    public void pushB()
    {
        // 演出中なら何もしない
        if (gameScene == SCENE.GAME_INEFFECT) return;

        /* for websocket*/
        // 自分のターンならコマンド発信

        if (gameObject.GetComponent<WebsocketAccessor>().enabled == true && gameTurn == CAMP.ALLY)
        {
            gameObject.GetComponent<WebsocketAccessor>().sendws("B");
        }
        
        

        switch (gameScene)
        {
            // 演出中につき操作不可
            case SCENE.STORY:
                break;

            case SCENE.MAIN:
                // nothing to do
                break;

            case SCENE.UNIT_SELECT_MOVETO:
                gameScene = SCENE.MAIN;
                selectedUnit.GetComponent<Unit>().deleteReachArea();
                cursor.GetComponent<cursor>().moveCursolToUnit(selectedUnit);
                break;

            case SCENE.UNIT_MENU:
                Destroy(unitMenuPanel);
                gameScene = SCENE.UNIT_SELECT_MOVETO;
                selectedUnit.GetComponent<Unit>().returnPrePosition();
                selectedUnit.GetComponent<Unit>().viewMovableArea();

                break;

            case SCENE.UNIT_SELECT_TARGET:
                gameScene = SCENE.UNIT_MENU;
                unitMenuPanel = Instantiate(Resources.Load<GameObject>("Prefab/UnitMenuPanel"), GameObject.Find("Canvas").transform);
                unitMenuPanel.GetComponent<UnitMenu>().init(selectedUnit.GetComponent<Unit>());
                selectedUnit.GetComponent<Unit>().deleteReachArea();

                break;

            case SCENE.UNIT_ACTION_FORECAST:
                gameScene = SCENE.UNIT_SELECT_TARGET;
                changeInfoWindow();
                break;

        }

    }


    //--- フィールドブロックが選択されたとき ---//
    public void pushBlock(int x, int y)
    {
        // 演出中なら何もしない
        if (gameScene == SCENE.GAME_INEFFECT) return;

        /* for websocket*/
        // 自分のターンならコマンド発信

        if (gameObject.GetComponent<WebsocketAccessor>().enabled == true && gameTurn == CAMP.ALLY)
        {
            gameObject.GetComponent<WebsocketAccessor>().sendws("PB-"+x+"-"+y);
        }


        Debug.Log("pushBlock");


        switch (gameScene)
        {
            // 演出中につき操作不可
            case SCENE.STORY:
                break;
                               

            case SCENE.UNIT_ACTION_FORECAST:
                if (cursor.GetComponent<cursor>().nowPosition[0] == x
                    && cursor.GetComponent<cursor>().nowPosition[1] == y)
                {
                    pushA(true);
                }
                break;

            default:
                cursor.GetComponent<cursor>().moveCursorToAbs(x, y);
                pushA(true);
                break;

        }
    }


    public void pushR()
    {
        // 演出中なら何もしない
        if (gameScene == SCENE.GAME_INEFFECT) return;

        /* for websocket*/
        // 自分のターンならコマンド発信

        if (gameObject.GetComponent<WebsocketAccessor>().enabled == true && gameTurn == CAMP.ALLY)
        {
            gameObject.GetComponent<WebsocketAccessor>().sendws("LRR");
        }


        if (gameScene == SCENE.MAIN && gameTurn == CAMP.ALLY)
            endUnitActioning(true);
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
