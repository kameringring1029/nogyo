using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Information;
using General;
using UnityEngine.UI;
using System;


/*
 * SRPGゲーム中のユニット挙動用クラス
 * ジョブ毎に継承クラスあり
 */


public class Unit : MonoBehaviour {


    protected GameMgr GM;
    protected Map map;

    public GameObject hpgaugePrefab;
    private GameObject unithpbar;

    public GameObject shadePrefab;
    private GameObject unitshade;

    public UnitStatus unitInfo;
    public CAMP camp;
    public bool isActioned;

    public int staticMoveVelocity = 2;

    private int[] moveVector = new int[2];
    private List<int[]> moveTo = new List<int[]>();

    public int[] nowPosition = new int[2];
    private int[] prePosition = new int[2];

    public GameObject movableAreaPrefab;
    public List<GameObject> movableAreaList = new List<GameObject>();
    private List<FieldBlock> movableRoute = new List<FieldBlock>();
    private List<List<FieldBlock>> movableRouteList = new List<List<FieldBlock>>();
    private List<FieldBlock> movableBlockList = new List<FieldBlock>(); 
    public GameObject reachAreaPrefab;
    private List<GameObject> reachAreaList = new List<GameObject>();


    // Use this for initialization
    void Start () {
    }


    //--- 初期設定 ---//
    // x,y:初期位置  c:陣営(1=味方 ,-1=敵)
    public void init(int x, int y, CAMP c, statusTable statustable)
    {
        // gameobject系の取得　※Startで実施するとGameobjectのinitができていないためここでやります
        GM = GameObject.Find("Main Camera").GetComponent<GameMgr>();
        map = GameObject.Find("Main Camera").GetComponent<Map>();


        // Status設定
        unitInfo = new UnitStatus(statustable);

        // Resources/UnitAnimフォルダからグラをロード
        gameObject.GetComponent<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("UnitAnim/" + unitInfo.graphic_id);
        


        // 陣営設定とSprite反転
        camp = c;
        changeSpriteFlip(0);

        // 影の生成
        unitshade = Instantiate(shadePrefab, transform.position, transform.rotation);
        unitshade.transform.SetParent(gameObject.transform);

        // HPバーの生成
        unithpbar = Instantiate(Resources.Load<GameObject>("Prefab/hpbar_bg_sprite"), gameObject.transform);
        switch (camp)
        {
            case CAMP.ALLY:
                unithpbar.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = new Color(0.25f, 0.25f, 1f);
                break;
            case CAMP.ENEMY:
                unithpbar.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 0.25f, 0.25f);
                break;
        }


        // 変数初期化
        for (int i=0; i < 2; i++)
        {
            nowPosition[i] = 0;
            prePosition[i] = 0;
            moveVector[i] = 0;
        }

        // 初期位置へ移動、行動権取得
        changePosition(x, y, false);
        restoreActionRight();

        // Spriteの表示Orderを更新
        gameObject.GetComponent<SpriteRenderer>().sortingOrder = map.FieldBlocks[nowPosition[0], nowPosition[1]].GetComponent<SpriteRenderer>().sortingOrder + 101;

        // カーソルをここに移動
        GM.cursor.GetComponent<cursor>().moveCursolToUnit(gameObject);

        // 出現エフェクト
        gameObject.GetComponent<Animator>().SetTrigger("isAppearing");
        GameObject particle = Instantiate(Resources.Load<GameObject>("Prefab/UnitAppear_Particle"),gameObject.GetComponent<Transform>());
        if (unitInfo.member_number != 0)
        {
            string group = "";
            switch (unitInfo.group_number)
            {
                case 0: group = "Muse";break;
                case 1: group = "Aqours";break;
            }
            particle.GetComponent<Transform>().GetChild(0).GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite
                = (Resources.LoadAll<Sprite>("Unit/Icons/"+group+"Icon"))[unitInfo.member_number - 1];
        }
    }


    // Update is called once per frame
    void Update ()
    {
        // ユニットの速度設定（通常時は0）
        gameObject.GetComponent<Rigidbody2D>().velocity =
            new Vector2((moveVector[0] - moveVector[1])/2.0f * staticMoveVelocity, -(moveVector[0] / 4.0f + moveVector[1] / 4.0f) * staticMoveVelocity);

        // 影の追従
     //   unitshade.transform.position = gameObject.transform.position;

        unitMove();
    }


    //---- 移動処理（y軸移動->x軸移動）----//
    //  移動ベクトルの決定、目的地への移動、表示系の変更等
    private void unitMove()
    {
        if(moveTo.Count > 0) {
            
            // x軸移動
            if (moveTo[0][0] != -1)
            {

                // 移動方向の決定とUnitの向きの変更
                if (moveTo[0][0] - nowPosition[0] == 0)
                {
                    moveVector[0] = 0;
                }
                else if (moveTo[0][0] - nowPosition[0] > 0)
                {
                    moveVector[0] = 1;
                    changeSpriteFlip(1);
                }
                else
                {
                    moveVector[0] = -1;
                    changeSpriteFlip(-1);
                }


                // 目的座標まで動いたら
                //  Unitの現在座標と目的ブロックの座標を比較
                //  比較演算子の関係で移動方向を乗算
                if (gameObject.GetComponent<Transform>().position.x * moveVector[0]
                     >= map.FieldBlocks[moveTo[0][0], nowPosition[1]].GetComponent<Transform>().position.x * moveVector[0])
                {
                    moveVector[0] = 0;

                    nowPosition[0] = moveTo[0][0];
                    moveTo[0][0] = -1;


                }

            }
            // y軸移動
            else if (moveTo[0][1] != -1)
            {
                // 移動中(演出中)はユーザ操作不可
                if(GM.enabled == true)
                    GM.setInEffecting(true);


                // 移動方向の決定とUnitの向きの変更
                if (moveTo[0][1] - nowPosition[1] == 0)
                {
                    moveVector[1] = 0;
                }
                else if (moveTo[0][1] - nowPosition[1] > 0)
                {
                    moveVector[1] = 1;
                    changeSpriteFlip(-1);
                }
                else
                {
                    moveVector[1] = -1;
                    changeSpriteFlip(1);
                }


                // 目的座標まで動いたら
                //  Unitの現在座標と目的ブロックの座標を比較
                //  比較演算子の関係で移動方向を乗算
                if (gameObject.GetComponent<Transform>().position.y * moveVector[1]
                     <= map.FieldBlocks[nowPosition[0], moveTo[0][1]].GetComponent<Transform>().position.y * moveVector[1])
                {
                    moveVector[1] = 0;
                    nowPosition[1] = moveTo[0][1];

                    moveTo[0][1] = -1;

                    // Spriteの表示Orderを更新
                    gameObject.GetComponent<SpriteRenderer>().sortingOrder = map.FieldBlocks[nowPosition[0], nowPosition[1]].GetComponent<SpriteRenderer>().sortingOrder + 101;

                }

            }


            // X,Y軸移動が完了したのでキューからdequeue-----------
            if (moveTo[0][0] == -1 && moveTo[0][1] == -1)
            {
                moveTo.RemoveAt(0);
            }


            // 全移動キューが消化されたら
            if (moveTo.Count == 0)
            {

                // ブロックの直上に調整
                gameObject.GetComponent<Transform>().position
                     = map.FieldBlocks[nowPosition[0], nowPosition[1]].GetComponent<Transform>().position;
                // 移動先BlockにUnit情報を設定
                map.FieldBlocks[nowPosition[0], nowPosition[1]].GetComponent<FieldBlock>().GroundedUnit = gameObject;
                // Spriteの表示Orderを更新
                gameObject.GetComponent<SpriteRenderer>().sortingOrder = map.FieldBlocks[nowPosition[0], nowPosition[1]].GetComponent<SpriteRenderer>().sortingOrder + 101;


                if (GM.enabled == true)
                {
                    GM.setInEffecting(false);
                    GM.endUnitMoving();

                }
                

                changeSpriteFlip(0);

                //
                if(!(unitInfo.movetype == MOVETYPE.FLY &&
                    (map.FieldBlocks[nowPosition[0],nowPosition[1]].GetComponent<FieldBlock>().blockInfo.groundtype() == GROUNDTYPE.HIGH ||
                     map.FieldBlocks[nowPosition[0], nowPosition[1]].GetComponent<FieldBlock>().blockInfo.groundtype() == GROUNDTYPE.SEA) ))
                    gameObject.GetComponent<Animator>().SetBool("isWalking", false);

            }
        }



        // 移動中ではない
        else
        {
            // do notiong
            // ブロックの直上に調整
            gameObject.GetComponent<Transform>().position
                 = map.FieldBlocks[nowPosition[0], nowPosition[1]].GetComponent<Transform>().position;
        }
    }


    //---- 目的地への移動を設定する ----//
    // x,y: 目的座標
    // walkflg = 1の場合、移動先座標を指定（残りはUpdateとunitMoveで実施）
    //           0の場合、ワープ
    public void changePosition(int x, int y, bool walkflg)
    {
        // 移動キャンセル用に移動前の座標を格納
        prePosition[0] = nowPosition[0]; prePosition[1] = nowPosition[1];
        
        // 移動元BlockからUnit情報を削除
        map.FieldBlocks[nowPosition[0], nowPosition[1]].GetComponent<FieldBlock>().GroundedUnit = null;

        // 歩行の有無
        if (walkflg)
        {
            walk(x, y);
        }
        // 歩行しない場合
        else
        {

            gameObject.GetComponent<Transform>().position
                 = map.FieldBlocks[x, y].GetComponent<Transform>().position;

            nowPosition[0] = x;
            nowPosition[1] = y;

            // 移動先BlockにUnit情報を設定
            map.FieldBlocks[x, y].GetComponent<FieldBlock>().GroundedUnit = gameObject;
        }

    }


    //---- 歩行演出付きの移動 ----//
    // distx,disty: 目的座標
    // 目的座標までの移動ルートを移動座標キューmoveToに格納してきます
    //  移動ルート:
    // 　目的座標方向に移動可能ブロックを探索していき、移動可能でなくなったら回り道する感じ？
    private void walk(int distx, int disty)
    {
        gameObject.GetComponent<Animator>().SetBool("isWalking", true);
        

        //移動ルートの算出
        for(int i=0; i<movableRouteList.Count; i++)
        {
            // ルートリストの中から目的座標のものを検索
            if (movableRouteList[i][movableRouteList[i].Count-1].GetComponent<FieldBlock>().position[0] == distx &&
                movableRouteList[i][movableRouteList[i].Count - 1].GetComponent<FieldBlock>().position[1] == disty)
            {
                // ルート情報から移動座標を指定格納
                for (int j=1; j< movableRouteList[i].Count; j++)
                {
                    int[] tmp = { movableRouteList[i][j].GetComponent<FieldBlock>().position[0], movableRouteList[i][j].GetComponent<FieldBlock>().position[1] };
                    moveTo.Add(tmp);
                }

                // 現在位置から変わらない場合
                if (moveTo[moveTo.Count - 1][0] == nowPosition[0] && moveTo[moveTo.Count - 1][1] == nowPosition[1])
                {
                    moveTo.Clear();
                    int[] tmp = { -1, -1};
                    moveTo.Add(tmp);
                }

                break;

            }
        }

        // 残りの移動処理はUpdateにて //
    }



    //--- 移動キャンセル処理 ---//
    public void returnPrePosition()
    {
        // 移動元BlockからUnit情報を削除
        map.FieldBlocks[nowPosition[0], nowPosition[1]].GetComponent<FieldBlock>().GroundedUnit = null;

        gameObject.GetComponent<Transform>().position
             = map.FieldBlocks[prePosition[0], prePosition[1]].GetComponent<Transform>().position;

        nowPosition[0] = prePosition[0];  nowPosition[1] = prePosition[1];

        // 移動先BlockにUnit情報を設定
        map.FieldBlocks[nowPosition[0], nowPosition[1]].GetComponent<FieldBlock>().GroundedUnit = gameObject;

        
        gameObject.GetComponent<Animator>().SetBool("isWalking", false);

    }




    //--- 指定地点にUnitが移動できるか判定 ---//
    //  return 移動可否
    //  potision; ワールド座標
    public bool canMove(Vector3 position)
    {
        for(int i=0; i<movableAreaList.Count; i++)
        {
            if(movableAreaList[i].transform.position == position)
            {
                return true;
            }
        }

        return false;
    }

    public bool canMove(int x, int y)
    {
        if(x > -1 && y > -1 )
        {
            for (int i = 0; i < movableAreaList.Count; i++)
            {
                if (movableAreaList[i].transform.position == map.FieldBlocks[x, y].transform.position)
                {
                    return true;
                }
            }
        }


        return false;
    }

    //--- 指定地点をアクション対象とできるか判定 ---//
    //  return アクション対象可否
    //  potision; ワールド座標
    public bool canReach(Vector3 position)
    {
        for (int i = 0; i < reachAreaList.Count; i++)
        {
            if (reachAreaList[i].transform.position == position)
            {
                return true;
            }
        }

        return false;
    }

    

    //--- 移動/対象範囲の表示 ---//
    //  (1)viewMovableArea
    //   親 アルゴリズム参考(http://2dgames.jp/2012/05/22/%E6%88%A6%E8%A1%93slg%E3%81%AE%E4%BD%9C%E3%82%8A%E6%96%B9%EF%BC%88%E7%A7%BB%E5%8B%95%E7%AF%84%E5%9B%B2%E3%82%92%E6%B1%82%E3%82%81%E3%82%8B%EF%BC%89/)
    public void viewMovableArea()
    {
        // 移動可能ブロックの特定（⇒movableBlockListのAdd）
        movableRoute.Add(map.FieldBlocks[nowPosition[0], nowPosition[1]].GetComponent<FieldBlock>());
        searchAreaCross(unitInfo.movable[1], map.FieldBlocks[nowPosition[0], nowPosition[1]].GetComponent<FieldBlock>());

        
        // movableBlockListの重複削除
        //  tmplistに一回移す→一つずつ戻す→戻したものと同じものはtmplistから削除
        List<FieldBlock> tmplist = new List<FieldBlock>();
        for (int i = 0; i < movableBlockList.Count; i++)
            tmplist.Add(movableBlockList[i]);
        movableBlockList.Clear();
        while (tmplist.Count > 0)
        {
            movableBlockList.Add(tmplist[0]);
            tmplist.RemoveAll(p => p == movableBlockList[movableBlockList.Count-1]);
        }
        

        // moveableRouteListの重複を削除
        //  List内で同じ目的座標のルートを比較し、より短経路のものを残す
        // indexaを基準にindexbを比較していく
        int indexa = 0;
        while(indexa < movableRouteList.Count - 1)
        {
            int indexb = indexa + 1;
            while (indexb < movableRouteList.Count)
            {
                // 2つのルートで目的座標が一致したら
                if ((movableRouteList[indexa][movableRouteList[indexa].Count - 1]) ==
                    (movableRouteList[indexb][movableRouteList[indexb].Count - 1]))
                {
                    // indexaのほうが短経路なら
                    if (movableRouteList[indexa].Count < movableRouteList[indexb].Count)
                    {
                        movableRouteList.RemoveAt(indexb);
                    }
                    // indexbのほうが短経路ならindexaを削除、indexaの探索を終了
                    else
                    {
                        movableRouteList.RemoveAt(indexa);
                        indexa--;
                        break;
                    }
                }
                // 2つのルートで目的座標が別ならつぎのindexbへ
                else
                {
                    indexb++;
                }
            }

            indexa++;
        }
        

        // movableBlockListを参照してPrefabから移動可能範囲を表示
        for (int i=0; i< movableBlockList.Count; i++)
        {
            FieldBlock block = movableBlockList[i];
            Vector3 position = map.FieldBlocks[block.position[0], block.position[1]].GetComponent<Transform>().position;
            movableAreaList.Add(Instantiate(movableAreaPrefab, position, transform.rotation));
        }

        // 自分の今いるブロックを移動可能範囲に追加
        movableAreaList.Add(Instantiate(movableAreaPrefab, transform.position, transform.rotation));


        // 選択済みユニットを更新
        if (GM.enabled == true)
            GM.selectedUnit = gameObject;
    }

    
    //  (2)searchAreaCross
    //   targetBlockの上下左右のブロックに移動可能か調べる
    private void searchAreaCross(int movable, FieldBlock targetBlock)
    {
        int nextx, nexty;
        
        nextx = targetBlock.position[0]; nexty = targetBlock.position[1] - 1;
        if (nexty>=0 && nexty < map.y_mass * 2)
            searchArea(movable, map.FieldBlocks[nextx, nexty].GetComponent<FieldBlock>());

        nextx = targetBlock.position[0]; nexty = targetBlock.position[1] + 1;
        if (nexty >= 0 && nexty < map.y_mass * 2)
            searchArea(movable, map.FieldBlocks[nextx, nexty].GetComponent<FieldBlock>());

        nextx = targetBlock.position[0] - 1; nexty = targetBlock.position[1];
        if (nextx >= 0 && nextx < map.x_mass * 2)
            searchArea(movable, map.FieldBlocks[nextx, nexty].GetComponent<FieldBlock>());

        nextx = targetBlock.position[0] + 1; nexty = targetBlock.position[1];
        if (nextx >= 0 && nextx < map.x_mass * 2)
            searchArea(movable, map.FieldBlocks[nextx, nexty].GetComponent<FieldBlock>());


    }

    //  (3)searchArea
    //   今残っているmovableでblockに移動できるかを計算
    private void searchArea(int movable, FieldBlock block)
    {

        movableRoute.Add(block);
        
        // 地形とユニットの移動タイプからblockへの移動コストを計算
        int movecost = 0;
        switch (block.blockInfo.groundtype())
        {
            case GROUNDTYPE.NORMAL:
                movecost = 1;
                break;
            case GROUNDTYPE.HIGH:
                if (unitInfo.movetype == MOVETYPE.FLY) { movecost = 1; }
                else { movecost = 100; }
                break;
            case GROUNDTYPE.SEA:
                if (unitInfo.movetype == MOVETYPE.FLY || unitInfo.movetype == MOVETYPE.SWIM) { movecost = 1; }
                else { movecost = 100; }
                break;
            case GROUNDTYPE.UNMOVABLE:
                movecost = 100;
                break;
        }

        // blockに別陣営ユニットがいたらそこも通れないよ
        if(block.GetComponent<FieldBlock>().GroundedUnit != null)
        {
            if (block.GetComponent<FieldBlock>().GroundedUnit.GetComponent<Unit>().camp != camp)
                movecost = 100;
        }

        movable = movable - movecost;
        
        // blockに移動できる場合
        if (movable >= 0)
        {

            // ユニットが乗っかっていない場合移動可能としてリストに追加
            if (block.GroundedUnit == null)
            {
                movableBlockList.Add(block);
            }

            // blockまでのルート情報をリストに保存
            List<FieldBlock> tmplist = new List<FieldBlock>();
            for (int i = 0; i < movableRoute.Count; i++)
                tmplist.Add(movableRoute[i]);
            movableRouteList.Add(tmplist);
            

            // 再帰呼び出し
            if (movable > 0) 
                searchAreaCross(movable, block);
        }


        movableRoute.RemoveAt(movableRoute.Count -1); 
    }


    //--- 移動/対象範囲の表示ここまで ---//


    //--- アクション対象範囲の表示 ---//
    public void viewTargetArea()
    {
        // アクション対象範囲を表示
        int reach = unitInfo.reach[1];
        for (int y = -reach; y <= reach; y++)
        {
            for (int x = abs(y) -reach; x <= reach - abs(y); x++)
            {
                // マップエリア内
                if ((nowPosition[0] + x >= 0 && nowPosition[1] + y >= 0) &&
                    (nowPosition[0] + x < map.x_mass * 2 && nowPosition[1] + y < map.y_mass * 2))
                {
                    Vector3 position = map.FieldBlocks[nowPosition[0] + x, nowPosition[1] + y].GetComponent<Transform>().position;
                    reachAreaList.Add(Instantiate(reachAreaPrefab, position, transform.rotation));
                }
            }
        }
    }



    //--- 対象アクション範囲/移動範囲用のパネルオブジェクトを除去 ---//
    public void deleteReachArea()
    {
        // 移動範囲オブジェクトを削除
        for (int i = 0; i < movableAreaList.Count; i++)
        {
            Destroy(movableAreaList[i]);
        }
        movableAreaList.Clear();
        movableBlockList.Clear();
        movableRouteList.Clear();
        movableRoute.Clear();

        // 対象アクション範囲オブジェクトを削除
        for (int i = 0; i < reachAreaList.Count; i++)
        {
            Destroy(reachAreaList[i]);
        }
        reachAreaList.Clear();
    }


    //--- ダメージを受けた時の処理 ---//
    // damageval: ダメージ量
    // dealFrom: ダメージ源
    public void beDamaged(int damageval, GameObject dealFrom)
    {
        if(damageval > 0)
            unitInfo.hp[1] -= damageval;

        StartCoroutine("damageCoroutine", dealFrom);

    }


    //--- 回復を受けた時の処理 ---//
    // healval: 回復量
    // dealFrom: 回復源
    public void beHealed(int healval, GameObject dealFrom)
    {

        unitInfo.hp[1] += healval;
        if (unitInfo.hp[1] > unitInfo.hp[0])
        {
            unitInfo.hp[1] = unitInfo.hp[0];
        }

        StartCoroutine("damageCoroutine", dealFrom);

    }


    //--- ダメージ処理用コルーチン ---//
    // dealFrom: ダメージ源
    IEnumerator damageCoroutine(GameObject dealFrom)
    {
        if (GM.enabled == true)
            GM.setInEffecting(true);

        unithpbar.transform.GetChild(0).localScale = new Vector3((float)unitInfo.hp[1] / (float)unitInfo.hp[0], unithpbar.transform.GetChild(0).localScale.y, unithpbar.transform.GetChild(0).localScale.z);


        // Spriteの明滅演出
        for (int i = 0; i < 3; i++)
        {
            changeSpriteColor(255, 255, 255, 100f);
            yield return new WaitForSeconds(0.25f);
            changeSpriteColor(255, 255, 255, 255);
            yield return new WaitForSeconds(0.25f);
        }

        // 消滅処理
        if(unitInfo.hp[1] <= 0)
        {
            // Spriteを徐々に薄く
            for (int i = 0; i < 10; i++)
            {
                yield return new WaitForSeconds(0.1f);
                changeSpriteColor(255, 255, 255, (float)(250 - i * 250 / 10));
            }

            // BlockからUnit情報を削除
            map.FieldBlocks[nowPosition[0], nowPosition[1]].GetComponent<FieldBlock>().GroundedUnit = null;

            // Managerのリストから削除
            if (GM.enabled == true)
                GM.removeUnitByList(gameObject);

            Destroy(unitshade);
            Destroy(gameObject);
        }

                
        // ダメージ源がUnitの場合
        if(dealFrom.GetComponent<Unit>()!=null)
            dealFrom.GetComponent<Unit>().endAction();
    }


    //--- アクション呼び出し ---//
    // targetUnit: アクションの対象となるUnit
    public void doAction(GameObject targetUnit, ACTION actionPattern)
    {
        if (GM.enabled == true)
            GM.setInEffecting(true);

        switch (actionPattern)
        {
            case ACTION.ATTACK:
                targetAttack(targetUnit);
                break;

            case ACTION.HEAL:
                targetHeal(targetUnit);
                break;

            case ACTION.REACTION:
                targetReaction(targetUnit);
                break;

        }
    }


    //--- 攻撃アクション ---//
    // targetUnit: アクションの対象となるUnit
    public virtual void targetAttack(GameObject targetUnit)
    {
        // virtual
    }

    //--- 攻撃アクションによって発生するダメージ ---//
    // targetUnit: アクションの対象となるUnit
    public virtual int getAttackDamage(GameObject targetUnit)
    {
        // virtual
        int damage = unitInfo.attack_phy[1]
        - targetUnit.GetComponent<Unit>().unitInfo.guard_phy[1];
        if (damage < 0) damage = 0;
        return damage;
    }

    //--- 攻撃アクションのヒット率 ---//
    // targetUnit: アクションの対象となるUnit
    public virtual int getAttackHit(GameObject targetUnit)
    {
        // virtual
        int hitrate = 100 - (targetUnit.GetComponent<Unit>().unitInfo.agility[1]
            - unitInfo.agility[1] + 10)*2;
        if (hitrate > 100) hitrate = 100;
        return hitrate;
    }

    //--- 攻撃アクションのクリティカル率 ---//
    // targetUnit: アクションの対象となるUnit
    public virtual int getAttackCritical(GameObject targetUnit)
    {
        // virtual
        int criticalrate = unitInfo.luck[1]
        - targetUnit.GetComponent<Unit>().unitInfo.luck[1];
        if (criticalrate < 0) criticalrate = 0;
        return criticalrate;
    }


    //--- 乱数というか状況から数値生成 ---//
    public virtual int getRandomFromMapstate(GameObject targetunit)
    {
        int rand = 0;

        rand = (gameObject.GetComponent<Unit>().nowPosition[0] + gameObject.GetComponent<Unit>().nowPosition[1]) % 10 * 10
                    + targetunit.GetComponent<Unit>().unitInfo.hp[1] % 10;

        return rand;
    }



    //--- 回復アクション ---//
    // targetUnit: アクションの対象となるUnit
    public virtual void targetHeal(GameObject targetUnit)
    {
        // virtual
    }
    
    //--- 回復アクションによって発生する回復量 ---//
    // targetUnit: アクションの対象となるUnit
    public virtual int getHealVal(GameObject targetUnit)
    {
        // virtual
        int healval = unitInfo.attack_magic[1];
        return healval;
    }

    //--- 再行動アクション ---//
    // targetUnit: アクションの対象となるUnit
    public virtual void targetReaction(GameObject targetUnit)
    {
        // virtual
    }


    //--- Jobの対象指定アクションパターンを調べる ---//
    // return; 実施可能なアクションパターンのlist
    public virtual List<ACTION> getActionableList()
    {
        List<ACTION> actionlist = new List<ACTION>();
        actionlist.Add(ACTION.ATTACK);
        actionlist.Add(ACTION.WAIT);

        return actionlist;
    }



    //--- 行動終了処理 ---//
    public void endAction()
    {
        gameObject.GetComponent<Animator>().SetBool("isAttacking", false);
        gameObject.GetComponent<Animator>().SetBool("isAttacking2", false);
        gameObject.GetComponent<Animator>().SetBool("isHealing", false);

        isActioned = true;
        changeSpriteColor(180f,180f,180f,255f);
        changeSpriteFlip(0);


        if (GM.enabled == true)
        {
            GM.setInEffecting(false);
            GM.endUnitActioning();
        }



    }


    //--- 行動権取得処理 ---//
    public void restoreActionRight()
    {
        isActioned = false;
        changeSpriteFlip(0);
        changeSpriteColor(255f, 255f, 255f, 255f);
    }


    //--- 対象のユニットに近づくためにはどこのBlockへ移動すればよいかを返す ---//
    //  移動可能Blockのリストから対象のユニットに一番近いものを計算
    // targetunit:対象のユニット
    // return:移動先のBlock
    public FieldBlock getBlockToApproach(GameObject targetunit)
    {
        FieldBlock targetblock = movableBlockList[0];

        // 移動可能Blockをひとつずつ確認し、最もtargetunitに近いものがtargetblockに入る
        for (int i = 1; i < movableBlockList.Count; i++)
        {
            int distanceA = abs(targetunit.GetComponent<Unit>().nowPosition[0] - movableBlockList[i].GetComponent<FieldBlock>().position[0])
                                + abs(targetunit.GetComponent<Unit>().nowPosition[1] - movableBlockList[i].GetComponent<FieldBlock>().position[1]);
            int distanceB = abs(targetunit.GetComponent<Unit>().nowPosition[0] - targetblock.GetComponent<FieldBlock>().position[0])
                                + abs(targetunit.GetComponent<Unit>().nowPosition[1] - targetblock.GetComponent<FieldBlock>().position[1]);

            if (distanceA <= distanceB) targetblock = movableBlockList[i];
        }

        return targetblock;
    }


    //--- Spriteの反転処理 ---//
    //  1;右向き
    // -1;左向き
    //  0;ニュートラル（陣営による）
    public void changeSpriteFlip(int vector)
    {
        bool flipx = (camp == CAMP.ENEMY) ? true : false;
        switch (vector)
        {
            case 1:
                flipx = true;
                break;
            case -1:
                flipx = false;
                break;
            case 0:
                flipx = (camp == CAMP.ENEMY) ? true : false;
                break;
        }

        gameObject.GetComponent<SpriteRenderer>().flipX = flipx;
        
    }


    //--- Spriteの色の変更 ---//
    private void changeSpriteColor(float r, float g, float b, float a)
    {
        // Animatorが有効のままだと色が変更できないので一度外す
        RuntimeAnimatorController anim = gameObject.GetComponent<Animator>().runtimeAnimatorController;
        gameObject.GetComponent<Animator>().runtimeAnimatorController = null;
        // 色の変更
        gameObject.GetComponent<SpriteRenderer>().color = new Color(r / 255f, g / 255f, b / 255f, a / 255f);
        // Animatorをもとに戻す
        gameObject.GetComponent<Animator>().runtimeAnimatorController = anim;
    }

    //--- 絶対値取得 ---//
    protected int abs(int a)
    {
        if (a < 0) a = a * (-1);
        return a;
    }

}
