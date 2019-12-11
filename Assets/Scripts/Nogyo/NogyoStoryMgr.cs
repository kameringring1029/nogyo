using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Information;
using General;

public class NogyoStoryMgr : MonoBehaviour
{
    List<NogyoEvent> nogyoevents;
    GameObject loadevent;
    GameObject eventselecter;

    GameObject messagewindow;
    NogyoEventscenarioArray[] scenarioarrays;
    int[] unitids; // 指定した参加ユニット
    int currentline = 0; // シナリオの現在の進行数

    bool inScenario; // シナリオ進行中フラグ

    int selectedScenarioId;
    bool scenarioUnselected;

    EmotionController ec;


    private void Start()
    {
        Debug.Log("NogyoStoryMgr");

        scenarioUnselected = false;

        eventselecter = GameObject.Find("EventListScrollView");

        loadevent = Instantiate(Resources.Load<GameObject>("Prefab/Nogyo/NogyoLoading"), GameObject.Find("FrontCanvas").transform);

        NogyoEventLoadSave.serverEndWrapper func = loadEventEnd;
        loadevent.GetComponent<NogyoEventLoadSave>().getEventFromServer(loadEventEnd);

        Debug.Log("NogyoStoryMgr in");
    }

    /* イベントのロードが終わったら呼ばれるよ */
    public void loadEventEnd(List<NogyoEvent> ev)
    {
        Destroy(loadevent);
        nogyoevents = ev;

        switch (scenarioUnselected)
        {
            case true:
                // 取得したイベントのリストをボタン化
                ButtonList.buttonexecWrapper func = selectEvent;
                ButtonList.setNogyoEventButtonList(nogyoevents.ToArray(), func, GameObject.Find("EventListContent").GetComponent<RectTransform>());
                break;

            case false:
                selectEvent("ep" + PlayerData.getinstance().day.ToString("D2"));
                break;
        }

    }

    /* イベントの選択が終わったら呼ばれるよ */
    public void selectEvent(int no) //
    {
        Debug.Log("selected event" + no + ":" + nogyoevents[no].name);

        if(eventselecter != null) eventselecter.SetActive(false);
        init(nogyoevents[no].scenarioarrays, NogyoInfoUtil.getUnitIdArr_FromStr(nogyoevents[no].unit));
    }
    public void selectEvent(string name) // なまえからけんさくするばあい, データの日付から実施
    {
        NogyoEvent ev;

        if (getScenarioIdByName(name) != -1)
        {
           ev = nogyoevents[getScenarioIdByName(name)];
        }
        else //該当のイベントがない場合
        {
            Debug.Log("該当イベントなし");
            loadLiving();
            return;
        }

        Debug.Log("selected event " + name + ":" + ev.name);

        if (eventselecter != null) eventselecter.SetActive(false);
        init(ev.scenarioarrays, NogyoInfoUtil.getUnitIdArr_FromStr(ev.unit));
    }

    // 名前からシナリオidを検索
    public int getScenarioIdByName(string name)
    {
        for(int i=0; i<nogyoevents.Count; i++)
        {
            if (nogyoevents[i].name == name) return i;
        }

        return -1;
    }


    private void Update()
    {
        // シナリオ進行中は排他
        if (!inScenario)
        {
            // 画面クリックされたら進行(シナリオ終了まで)
            if ((Input.GetMouseButtonDown(0) || Input.GetButtonDown("Fire2")) && currentline > 0)
            {
                if (currentline == scenarioarrays.Length)
                {
                    // ストーリー的な諸々の清算
                    endStory();

                }
                else
                {
                    // シナリオを一行分進行
                    StartCoroutine(advanceScenario());
                }
            }
        }

    }

    public void init(NogyoEventscenarioArray[] scenarioarrays, int[] unitids)
    {
        ec = new EmotionController();

        this.scenarioarrays = scenarioarrays;
        this.unitids = unitids;

        messagewindow = Instantiate(Resources.Load<GameObject>("Prefab/Nogyo/NogyoMessageWindow"), GameObject.Find("NogyoEventCanvas").transform);

        // 最初のシナリオ行を実行
        currentline = 0;
        StartCoroutine(advanceScenario());
    }


    // シナリオを一行分進行
    IEnumerator advanceScenario()
    {

        inScenario = true;

        // 前回分の削除
        ec.clearEmotion();

        // シナリオ一行の中の全アクション分を処理
        foreach (NogyoEventScenario scenario in scenarioarrays[currentline].scenario)
        {
            updateMessageText(scenario);


            if (scenario.action == (int)STORYACTION.APPEAR)
            {
                // ユニット出現処理
                unitAppear(scenario);
                yield return new WaitForSeconds(0.05f);
                updateMessageSprite(null);
                yield return new WaitForSeconds(0.3f);
            }
            else
            {
                // アクション対象ユニットを取得
                GameObject actionunit =　NogyoInfoUtil.getActionUnit(scenario);

                // 
                ec.updateEmotion(scenario.action, actionunit);

                // メッセージウィンドウのSpriteとカーソル位置を更新
                // 今のシナリオ中に複数アクションがある場合は最後のアクションのみ実行
                if (scenario == scenarioarrays[currentline].scenario[scenarioarrays[currentline].scenario.Length - 1])
                {
                    yield return new WaitForSeconds(0.05f);
                    updateMessageSprite(actionunit);
                   // updateCursor(actionunit);
                }
            }

        }

        currentline++;

        inScenario = false;
    }

    //
    public void skipEvent()
    {
        endStory();
    }

    // ユニット出現の処理
    void unitAppear(NogyoEventScenario mapscenario)
    {
        /*
        switch (mapscenario.camp)
        {
            case 1:
              //  gameObject.GetComponent<Map>().setUnitFromId(unitids[mapscenario.unitno], CAMP.ALLY);
                gameObject.GetComponent<Map>().allyUnitList[gameObject.GetComponent<Map>().allyUnitList.Count - 1].GetComponent<Animator>().SetBool("inStory", true);
                break;
            case -1:
                string enemyinfo = gameObject.GetComponent<Map>().mapinformation.enemy[gameObject.GetComponent<Map>().enemyUnitList.Count];
                int enemyunitid = int.Parse(enemyinfo.Split('-')[2]);
              //  gameObject.GetComponent<Map>().setUnitFromId(enemyunitid, CAMP.ENEMY);
                gameObject.GetComponent<Map>().enemyUnitList[gameObject.GetComponent<Map>().enemyUnitList.Count - 1].GetComponent<Animator>().SetBool("inStory", true);
                break;

        }
        */
    }

    // メッセージウィンドウのテキストを更新
    void updateMessageText(NogyoEventScenario scenario)
    {
        string name = "";

        Debug.Log(scenario.message);

        // メッセージに話者の名前を追加
        if (scenario.message != "")
        {
            name = NogyoProfileUtil.getProfile(scenario.unitno).name();
            if (scenario.action > 60 && scenario.action < 70)
            {
                name = name + "?";
            }

            Color color = NogyoProfileUtil.getProfile(scenario.unitno).color();
            name = "<color=#" + ColorUtility.ToHtmlStringRGB(color) + ">" + name + "</color>: ";
        }
                       
        messagewindow.GetComponent<MessageManager>().setText(name , scenario.message);
    }

    // メッセージウィンドウの画像を更新
    void updateMessageSprite(GameObject actionunit)
    {
        GameObject image = messagewindow.transform.GetChild(1).transform.GetChild(0).gameObject; //あほっぽいから修正したい
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
        ec.clearEmotion();

        loadLiving();
    }


    public void loadEditor()
    {
        SceneManager.LoadScene("EdiEvent");
    }

    
    void loadLiving()
    {
        SceneManager.LoadScene("NogyoLiving");
    }
}
