using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using General;
using System;


/*
 * CPUの挙動
 * GameMgrからEnemyターンに呼び出し
 */

public class EnemyMgr : MonoBehaviour {

    GameMgr GM;
    Map map;

    private Queue<string> recvMsg = new Queue<string>();

	// Use this for initialization
	void Start () {

        GM = gameObject.GetComponent<GameMgr>();
        map = gameObject.GetComponent<Map>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void startEnemyTurn()
    {
        
       recvMsg = new Queue<string>();

        switch (gameObject.GetComponent<WebsocketAccessor>().enabled)
        {
            case false:
                StartCoroutine("enemyTurnAuto");
                break;

            case true:
                StartCoroutine("enemyTurnWeb");
                break;
        }
    }

    //-- 通信対戦用 --//
    IEnumerator enemyTurnWeb()
    {
     //   yield return new WaitForSeconds(1f);
     //   while (GM.gameScene == SCENE.GAME_INEFFECT) yield return new WaitForSeconds(0.05f);


        while (GM.gameTurn == CAMP.ENEMY)
        {
            string order = "";
            if (recvMsg.Count > 0)
            {
                Debug.Log("[EM]Deque:"+order);
                order = recvMsg.Dequeue();

                switch (order)
                {
                    case "A":
                        GM.pushA(); break;
                    case "B":
                        GM.pushB(); break;
                    case "U":
                        GM.pushArrow(0, 1); break;
                    case "D":
                        GM.pushArrow(0, -1); break;
                    case "L":
                        GM.pushArrow(-1, 0); break;
                    case "R":
                        GM.pushArrow(1, 0); break;
                    case "LRR":
                        GM.pushR(); break;
                    default:
                        if(order.StartsWith("PB-"))
                            GM.pushBlock(Int32.Parse( order.Split('-')[1]), Int32.Parse(order.Split('-')[2]));
                        if (order.StartsWith("SA-"))
                            GM.unitMenuPanel.GetComponent<UnitMenu>().selectAction(Int32.Parse(order.Split('-')[1]));
                        break;
                }

            }
            yield return new WaitForSeconds(0.05f);
            while (GM.gameScene == SCENE.GAME_INEFFECT) yield return new WaitForSeconds(0.05f);
        }
        

    }


    //-- AI用 --//
    IEnumerator enemyTurnAuto()
    {
        yield return new WaitForSeconds(1f);
        while (GM.gameScene == SCENE.GAME_INEFFECT) yield return new WaitForSeconds(0.5f);

        // 全ユニットが行動完了してGameMgrがターンを遷移するまで続く
        // ユニットごとにループするイメージ
        while (GM.gameTurn == CAMP.ENEMY)
        {

            // カーソルはユニットに置かれるからとりあえず動くためにAボタン押す
            GM.pushA();
            yield return new WaitForSeconds(0.5f);
            while (GM.gameScene == SCENE.GAME_INEFFECT) yield return new WaitForSeconds(0.5f);

            // とりあえず攻撃したいので敵ユニットに近づく
            GameObject nearestAlly = map.getNearAllyUnit(GM.selectedUnit);
            FieldBlock moveto = GM.selectedUnit.GetComponent<Unit>().getBlockToApproach(nearestAlly);
            GM.pushBlock(moveto.position[0], moveto.position[1]);
            yield return new WaitForSeconds(1f);
            while (GM.gameScene == SCENE.GAME_INEFFECT) yield return new WaitForSeconds(0.5f);


            // 敵ユニットに攻撃が届くか確認
            int distanceToTargetUnit =
                abs(nearestAlly.GetComponent<Unit>().nowPosition[0] - GM.selectedUnit.GetComponent<Unit>().nowPosition[0]) +
                abs(nearestAlly.GetComponent<Unit>().nowPosition[1] - GM.selectedUnit.GetComponent<Unit>().nowPosition[1]);
            

            // 攻撃が届けば殴る
            if (distanceToTargetUnit <= GM.selectedUnit.GetComponent<Unit>().unitInfo.reach[1]) {

                // Menuカーソルをこうげき(0)に移動
                if (GM.unitMenuPanel.GetComponent<UnitMenu>().getSelectedAction() != 0)
                {
                    GM.pushArrow(0, 1);
                    yield return new WaitForSeconds(0.5f);
                }

                // こうげきを選択
                GM.pushA();
                yield return new WaitForSeconds(0.5f);
                while (GM.gameScene == SCENE.GAME_INEFFECT) yield return new WaitForSeconds(0.5f);

                // 敵ユニットを選択
                GM.pushBlock(nearestAlly.GetComponent<Unit>().nowPosition[0], nearestAlly.GetComponent<Unit>().nowPosition[1]);
                yield return new WaitForSeconds(0.5f);
                while (GM.gameScene == SCENE.GAME_INEFFECT) yield return new WaitForSeconds(0.5f);

                // 攻撃指定
                GM.pushA();
                yield return new WaitForSeconds(0.5f);
                while (GM.gameScene == SCENE.GAME_INEFFECT) yield return new WaitForSeconds(0.5f);
            }
            // 攻撃が届かない場合
            else
            {
                // たいきにMenuカーソルを移動
                if (GM.unitMenuPanel.GetComponent<UnitMenu>().getSelectedAction() == 0)
                {
                    GM.pushArrow(0, 1);
                    yield return new WaitForSeconds(0.5f);
                }

                // 待機に決定
                GM.pushA();
                yield return new WaitForSeconds(0.5f);
                while (GM.gameScene == SCENE.GAME_INEFFECT) yield return new WaitForSeconds(0.5f);

            }
            
        }


        yield return new WaitForSeconds(0.5f);
        while (GM.gameScene == SCENE.GAME_INEFFECT) yield return new WaitForSeconds(0.5f);


    }


    //--- 移動先の特定  ---//
    // 現状最も近い敵ユニットを見つけ、自分の移動範囲内で近づくようにしてるよ
    // selectedunit: 移動するUnit
    // return: 移動先のBlock
    private FieldBlock detectMoveTo(GameObject selectedunit)
    {
        GameObject nearestAlly = map.getNearAllyUnit(selectedunit);

        return selectedunit.GetComponent<Unit>().getBlockToApproach(nearestAlly);
        
    }



    public void enqRecvMsg(string msg)
    {
        recvMsg.Enqueue(msg);
    }



    //--- 絶対値取得 ---//
    private int abs(int a)
    {
        if (a < 0) a = a * (-1);
        return a;
    }
}
