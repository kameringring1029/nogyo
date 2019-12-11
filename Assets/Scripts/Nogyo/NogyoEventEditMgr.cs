using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.SceneManagement;

using Information;
using General;
using TMPro;


/*
 * イベントエディタだよ
 * 
 */

public class NogyoEventEditMgr : MonoBehaviour
{

    List<NogyoEvent> nogyoevents;
    GameObject loadevent;

    GameObject eventselecter;
    GameObject messagewindow;
    GameObject emotionselecter;

    int editingevent;

    NogyoEventscenarioArray[] scenarioarrays;
    int[] unitids; // 指定した参加ユニット
    int currentline = 0; // シナリオの現在の進行数
    
    EmotionController ec;

    public delegate void editend(NogyoEventScenario scenario);


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("NogyoStoryEditMgr");

        ec = new EmotionController();
        emotionselecter = GameObject.Find("EditEmotionSelecter");
        emotionselecter.SetActive(false);
        eventselecter = GameObject.Find("EditEventListScrollView");

        // シナリオをロード
        loadevent = Instantiate(Resources.Load<GameObject>("Prefab/Nogyo/NogyoLoading"), GameObject.Find("FrontCanvas").transform);
        NogyoEventLoadSave.serverEndWrapper func = loadEventEnd;
        loadevent.GetComponent<NogyoEventLoadSave>().getEventFromServer(loadEventEnd);
    }

    /* イベントのロードが終わったら呼ばれるよ */
    public void loadEventEnd(List<NogyoEvent> ev)
    {
        Destroy(loadevent);
        nogyoevents = ev;

        // 取得したイベントのリストをボタン化
        ButtonList.buttonexecWrapper func = selectEvent;
        ButtonList.setNogyoEventButtonList(nogyoevents.ToArray(), func, GameObject.Find("EditEventListContent").GetComponent<RectTransform>());
    }

    /* イベントの選択が終わったら呼ばれるよ */
    public void selectEvent(int no)
    {
        Debug.Log("selected event" + no + ":" + nogyoevents[no].name);
        editingevent = no;

        eventselecter.SetActive(false);
        init(nogyoevents[no].scenarioarrays, NogyoInfoUtil.getUnitIdArr_FromStr(nogyoevents[no].unit));
    }


    public void init(NogyoEventscenarioArray[] scenarioarrays, int[] unitids)
    {

        this.scenarioarrays = scenarioarrays;
        this.unitids = unitids;

        currentline = 0;

        // シナリオボタン一覧を更新
        renewScenarioButtonList();

    }

    // 編集リストからテキストボタンを選択
    public void onClickTextEdit()
    {
        if(messagewindow == null)
        {
            messagewindow = Instantiate(Resources.Load<GameObject>("Prefab/Nogyo/NogyoMessageWindowEdit"), GameObject.Find("FrontCanvas").transform);
            editend func = endEditScenarioParts;
            messagewindow.GetComponent<NogyoMessageWindowEdit>().init(getSelectedMessageScenario(), func);
        }
        else
        {
            messagewindow.GetComponent<NogyoMessageWindowEdit>().onClickEditEnd();
            messagewindow = null;
        }

    }

    // シナリオボタンリストからシナリオを選択したときの処理
    public void selectScenario(int no)
    {
        Debug.Log("selected scenario:" + no);
        currentline = no;

        // 選択中シナリオの表示を更新　10文字で区切る
        if(getSelectedMessageScenario().message.Length < 11)
        {
            GameObject.Find("SelectedScenario").GetComponent<TextMeshProUGUI>().text = getSelectedMessageScenario().message;
        }
        else
        {
            GameObject.Find("SelectedScenario").GetComponent<TextMeshProUGUI>().text
                = MessageManager.SubstringAtCount(getSelectedMessageScenario().message, 10)[0] + "...";
        }

        // メッセージウィンドウが表示されていればそちら更新
        if (messagewindow != null)
            messagewindow.GetComponent<NogyoMessageWindowEdit>().init(getSelectedMessageScenario());

        // Emotion更新
        renewEmotion();


    }

    // シナリオパーツの編集が完了したときのしょり
    public void endEditScenarioParts(NogyoEventScenario scenario)
    {
        setSelectedMessageScenario(scenario);

        // シナリオボタン一覧を更新
        renewScenarioButtonList();
    }

    // サーバーにデータセーブ
    public void save()
    {
        // 編集した分を格納
        nogyoevents[editingevent].scenarioarrays = scenarioarrays;

        // 名前を保管
        string eventname = GameObject.Find("SaveInputField").GetComponent<WebGLNativeInputField>().text;
        if (eventname != "")
            nogyoevents[editingevent].name = eventname;

        // セーブ
        loadevent = Instantiate(Resources.Load<GameObject>("Prefab/Nogyo/NogyoLoading"), GameObject.Find("FrontCanvas").transform);
        loadevent.GetComponent<NogyoEventLoadSave>().saveOnServer(nogyoevents[editingevent]);

    }

    
    public void loadViewer()
    {
        SceneManager.LoadScene("Event");
    }


    //----シナリオデータの編集関連ここから------//

    // シナリオ末尾に追加
    public void addScenario()
    {
        // 一度リスト化して追加
        List<NogyoEventscenarioArray> list = new List<NogyoEventscenarioArray>();
        list.AddRange(scenarioarrays);
        list.Add(NogyoInfoUtil.getNewNESA());
        scenarioarrays = list.ToArray();

        // ボタンリストを更新
        renewScenarioButtonList();
    }

    // 現在選択中ののシナリオを削除
    public void removeScenario()
    {
        // 一度リスト化して該当の要素を削除
        List<NogyoEventscenarioArray> list = new List<NogyoEventscenarioArray>();
        list.AddRange(scenarioarrays);
        list.RemoveAt(currentline);
        scenarioarrays = list.ToArray();

        // ボタンリストを更新
        renewScenarioButtonList();
    }

    // シナリオ順の変更
    public void moveScenario(bool heigher)
    {
        // シナリオを一時保管
        NogyoEventscenarioArray tmp = scenarioarrays[currentline];

        // 一度リスト化、該当シナリオを削除
        List<NogyoEventscenarioArray> list = new List<NogyoEventscenarioArray>();
        list.AddRange(scenarioarrays);
        list.RemoveAt(currentline);
        
        // 移動先に挿入
        switch (heigher)
        {
            case true:
                if (currentline > 0) currentline--;
                break;
            case false:
                if (currentline < scenarioarrays.Length - 1) currentline++;
                break;
        }

        list.Insert(currentline, tmp);
        scenarioarrays = list.ToArray();

        // ボタンリストを更新
        renewScenarioButtonList();
    }


    public void onClickEmotionSelecter()
    {
        if(emotionselecter.active == false)
        {
            emotionselecter.SetActive(true);
        }
        else
        {
            emotionselecter.SetActive(false);
        }
    }
    // emotion情報の変更
    public void selectEmotion(int action)
    {
        getSelectedMessageScenario().action = action;
        renewEmotion();
    }

    //----シナリオデータの編集関連ここまで------//



    // シナリオボタン一覧を更新
    void renewScenarioButtonList()
    {
        ButtonList.buttonexecWrapper func = selectScenario;
        ButtonList.setScenarioButtonList(scenarioarrays, func, GameObject.Find("ScenarioListContent").GetComponent<RectTransform>());

        selectScenario(currentline);
    }

    // emotion更新
    void renewEmotion()
    {
        ec.clearEmotion();
        ec.updateEmotion(getSelectedMessageScenario().action, GameObject.Find("player" + getSelectedMessageScenario().unitno));
    }


    // 現在選択中のシナリオarrayの中のメッセージが含まれるやつに関するgetter, seter
    NogyoEventScenario getSelectedMessageScenario()
    {
        return scenarioarrays[currentline].scenario[scenarioarrays[currentline].scenario.Length - 1];
    }
    void setSelectedMessageScenario(NogyoEventScenario scenario)
    {
        scenarioarrays[currentline].scenario[scenarioarrays[currentline].scenario.Length - 1] = scenario;
    }
}
