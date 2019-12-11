using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using Information;
using General;
using UnityEngine.UI;

public class StoryMgr : MonoBehaviour
{
    GameObject messagewindow;
    mapinfo mapinfo;
    int[] unitids; // 指定した参加ユニット
    int currentline = 0; // シナリオの現在の進行数

    bool inScenario; // シナリオ進行中フラグ


    private void Update()
    {
        // シナリオ進行中は排他
        if (!inScenario)
        {
            // 画面クリックされたら進行(シナリオ終了まで)
            if ((Input.GetMouseButtonDown(0) || Input.GetButtonDown("Fire2")) && currentline > 0)
            {
                if (currentline == mapinfo.mapscenarioarrays.Length)
                {
                    // ストーリー的な諸々の清算
                    endStory();

                    // SRPGの開始
                    gameObject.GetComponent<GameMgr>().startSRPG();
                }
                else
                {
                    // シナリオを一行分進行
                    StartCoroutine(advanceScenario());
                }
            }
        }

    }

    public void init(mapinfo mapinfo, int[] unitids)
    {
        this.mapinfo = mapinfo;
        if (unitids.Length == 0) unitids = UnitStatusUtil.randunit(2); // ユニット指定がされていなかったらランダムに
        this.unitids = unitids;

        messagewindow = Instantiate(Resources.Load<GameObject>("Prefab/MessageWindow"), GameObject.Find("Canvas").transform);
        
        // 最初のシナリオ行を実行
        currentline = 0;
        StartCoroutine( advanceScenario());
    }

    // シナリオを一行分進行
    IEnumerator advanceScenario()
    {
        inScenario = true;

        // 前回分の削除
        gameObject.GetComponent<EmotionController>().clearEmotion();

        // シナリオ一行の中の全アクション分を処理
        foreach (Mapscenario mapscenario in mapinfo.mapscenarioarrays[currentline].mapscenario)
        {
            updateMessageText(mapscenario);

            if (mapscenario.action == (int)STORYACTION.APPEAR)
            {
                // ユニット出現処理
                unitAppear(mapscenario);
                yield return new WaitForSeconds(0.05f);
                updateMessageSprite(null);
                yield return new WaitForSeconds(0.3f);
            }
            else
            {
                // アクション対象ユニットを取得
                GameObject actionunit = getActionUnit(mapscenario);

                // 
                gameObject.GetComponent<EmotionController>().updateEmotion(mapscenario.action, actionunit);

                // メッセージウィンドウのSpriteとカーソル位置を更新
                // 今のシナリオ中に複数アクションがある場合は最後のアクションのみ実行
                if (mapscenario == mapinfo.mapscenarioarrays[currentline].mapscenario[mapinfo.mapscenarioarrays[currentline].mapscenario.Length - 1])
                {
                    yield return new WaitForSeconds(0.05f);
                    updateMessageSprite(actionunit);
                    updateCursor(actionunit);
                }
            }

        }

        currentline++;

        inScenario = false;
    }

    // ユニット出現の処理
    void unitAppear(Mapscenario mapscenario)
    {
        switch (mapscenario.camp)
        {
            case 1:
                gameObject.GetComponent<Map>().setUnitFromId(unitids[mapscenario.unitno], CAMP.ALLY);
                gameObject.GetComponent<Map>().allyUnitList[gameObject.GetComponent<Map>().allyUnitList.Count - 1].GetComponent<Animator>().SetBool("inStory",true);
                break;
            case -1:
                string enemyinfo = gameObject.GetComponent<Map>().mapinformation.enemy[gameObject.GetComponent<Map>().enemyUnitList.Count];
                int enemyunitid = int.Parse(enemyinfo.Split('-')[2]);
                gameObject.GetComponent<Map>().setUnitFromId(enemyunitid, CAMP.ENEMY);
                gameObject.GetComponent<Map>().enemyUnitList[gameObject.GetComponent<Map>().enemyUnitList.Count - 1].GetComponent<Animator>().SetBool("inStory", true);
                break;

        }
    }

    // メッセージウィンドウのテキストを更新
    void updateMessageText(Mapscenario mapscenario)
    {

        string name = "";

        // メッセージに話者の名前を追加
        if (mapscenario.message != "")
        {
            GameObject actionunit = getActionUnit(mapscenario);

            name = actionunit.GetComponent<Unit>().unitInfo.name;
            if (mapscenario.action > 60 && mapscenario.action < 70)
            {
                name = name + "?";
            }

            Color color = actionunit.GetComponent<Unit>().unitInfo.color;
            name = "<color=#" + ColorUtility.ToHtmlStringRGB(color) + ">" + name + "</color>: ";
        }


        messagewindow.GetComponent<MessageManager>().setText(name , mapscenario.message);
    }

    // メッセージウィンドウの画像を更新
    void updateMessageSprite(GameObject actionunit)
    {
        GameObject image = messagewindow.GetComponent<Transform>().GetChild(1).gameObject.GetComponent<Transform>().GetChild(0).gameObject; //あほっぽいから修正したい
        if (actionunit != null)
        {
            image.GetComponent<Image>().sprite = actionunit.GetComponent<SpriteRenderer>().sprite;
        }
        else
        {
            // 無地イメージに(builtin resourcesはUnityEditorをusingしないと使えない⇔usingするとややこしいのでPrefab化して利用)
            image.GetComponent<Image>().sprite = Resources.Load<GameObject>("Prefab/UIMask").GetComponent<Image>().sprite;
        }

    }

    // カーソル位置を更新
    void updateCursor(GameObject actionunit)
    {
        gameObject.GetComponent<GameMgr>().cursor.GetComponent<cursor>().moveCursolToUnit(actionunit);
    }

    // ストーリー修了時の諸々の清算
    void endStory()
    {
        currentline = 0;
        Destroy(messagewindow);
        gameObject.GetComponent<EmotionController>().clearEmotion();

        foreach( GameObject ally in gameObject.GetComponent<Map>().allyUnitList)
        {
            ally.GetComponent<Animator>().SetInteger("storyState", 0);
            ally.GetComponent<Animator>().SetBool("inStory", false);
        }
        foreach (GameObject enemy in gameObject.GetComponent<Map>().enemyUnitList)
        {
            enemy.GetComponent<Animator>().SetInteger("storyState", 0);
            enemy.GetComponent<Animator>().SetBool("inStory", false);
        }
    }

    // シナリオ情報からアクションするユニットのゲームオブジェクトを取得
    GameObject getActionUnit(Mapscenario mapscenario)
    {
        switch (mapscenario.camp)
        {
            case 1:
                return gameObject.GetComponent<Map>().allyUnitList[mapscenario.unitno];
            case -1:
                return gameObject.GetComponent<Map>().enemyUnitList[mapscenario.unitno];
            default:
                return null;
        }
    }

}
