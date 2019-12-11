using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using General;


/*
 * SRPGゲーム中に表示するユニット行動メニューを制御する
 */

public class UnitMenu : MonoBehaviour {

    private int nowCursorPosition = 0;
    private List<ACTION> actionList;

    private List<GameObject> actionSelectBtn;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    

    public void init(Unit selectedUnit)
    {
        actionSelectBtn = new List<GameObject>();

        List<ACTION> unitActionList = selectedUnit.getActionableList();

        GameObject btnPref = Resources.Load<GameObject>("Prefab/UnitMenuButtonPrefab");

        //Content取得(ボタンを並べる場所)
        RectTransform content = gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).GetComponent<RectTransform>();

        //Contentの高さ決定
        //(ボタンの高さ+ボタン同士の間隔)*ボタン数
        float btnSpace = content.GetComponent<VerticalLayoutGroup>().spacing;
        float btnHeight = btnPref.GetComponent<LayoutElement>().preferredHeight;
        content.sizeDelta = new Vector2(0, (btnHeight + btnSpace) * unitActionList.Count);

        string[] menutexts = createMenuTexts(unitActionList);

        for (int no = 0; no < unitActionList.Count; no++)
        {
            //ボタン生成
            GameObject btn = (GameObject)Instantiate(btnPref,content);

            btn.GetComponent<RectTransform>().sizeDelta = new Vector2(btnPref.GetComponent<RectTransform>().sizeDelta[0] * 2 / 3, btnPref.GetComponent<RectTransform>().sizeDelta[1] * 2);
      

            //ボタンのテキスト変更
            btn.transform.GetComponentInChildren<Text>().text = menutexts[no];

            //色変更
            btn.GetComponent<Image>().color = new Color(92 / 255f, 92 / 255f, 128 / 255f, 255 / 255f);

            //ボタンのクリックイベント登録
            int tempno = no;
            btn.transform.GetComponent<Button>().onClick.AddListener(() => selectAction(tempno));

            actionSelectBtn.Add(btn);
        }
    }

    //--- UnitのアクションパターンリストからMenuのテキストを作成 ---//
    // selectedUnit: 現在選択中のUnit
    // return; アクションMenuのText
    private string[] createMenuTexts(List<ACTION> unitActionList)
    {
        string[] menutexts = new string[unitActionList.Count];
        actionList = new List<ACTION>();

        // アクションパターンリストを上から並べる
        for (int i = 0; i < unitActionList.Count; i++)
        {

            switch (unitActionList[i])
            {
                case ACTION.ATTACK:
                    menutexts[i] = "こうげき";
                    actionList.Add(ACTION.ATTACK);
                    break;

                case ACTION.HEAL:
                    menutexts[i] = "かいふく";
                    actionList.Add(ACTION.HEAL);
                    break;

                case ACTION.REACTION:
                    menutexts[i] = "うたう";
                    actionList.Add(ACTION.REACTION);
                    break;

                case ACTION.WAIT:
                    menutexts[i] = "たいき";
                    actionList.Add(ACTION.WAIT);
                    break;

                default:
                    break;
            }
        }
        
        return menutexts;
    }


    //--- Menu中のカーソルを移動 ---//
    // selectedUnit: 現在選択中のUnit
    public void moveCursor(int vector, Unit selectedUnit)
    {

        List<ACTION> unitActionList = selectedUnit.getActionableList();

        nowCursorPosition += vector;

        // カーソル位置がオーバーフローしたとき
        if (nowCursorPosition < 0) nowCursorPosition = unitActionList.Count - 1;
        if (nowCursorPosition > unitActionList.Count - 1) nowCursorPosition = 0;

        Debug.Log("nowcursor:" + nowCursorPosition);

        foreach(GameObject btn in actionSelectBtn)
        {
            if (btn == actionSelectBtn[nowCursorPosition])
            {
                btn.GetComponent<Image>().color = new Color(192 / 255f, 192 / 255f, 228 / 255f, 255 / 255f);
            }
            else
            {
                btn.GetComponent<Image>().color = new Color(192 / 255f, 192 / 255f, 228 / 255f, 192 / 255f);
            }
        }
    }


    /* 廃止

    //--- Menu中のカーソルを移動 ---//
    // selectedUnit: 現在選択中のUnit
    public void moveCursor(int vector, Unit selectedUnit){
        
        List<ACTION> unitActionList = selectedUnit.getActionableList();

        nowCursorPosition += vector;

        // カーソル位置がオーバーフローしたとき
        if (nowCursorPosition < 0) nowCursorPosition = unitActionList.Count -1;
        if (nowCursorPosition > unitActionList.Count -1) nowCursorPosition = 0;

        gameObject.GetComponent<Text>().text = createMenuText(unitActionList);
    }

    
    //--- UnitのアクションパターンリストからMenuのテキストを作成 ---//
    // selectedUnit: 現在選択中のUnit
    // return; アクションMenuのText
    private string createMenuText(List<ACTION> unitActionList)
    {
        string menutext = "";
        actionList.Clear();

        // アクションパターンリストを上から並べる
        for (int i = 0; i < unitActionList.Count; i++)
        {
            if (nowCursorPosition == i)
            {
                menutext += "⇒";
            }
            else
            {
                menutext += "　";
            }
                      

            switch (unitActionList[i])
            {
                case ACTION.ATTACK:
                    menutext += "こうげき";
                    actionList.Add(ACTION.ATTACK);
                    break;

                case ACTION.HEAL:
                    menutext += "かいふく";
                    actionList.Add(ACTION.HEAL);
                    break;

                case ACTION.REACTION:
                    menutext += "うたう";
                    actionList.Add(ACTION.REACTION);
                    break;

                case ACTION.WAIT:
                    menutext += "たいき";
                    actionList.Add(ACTION.WAIT);
                    break;

                default:
                    break;
            }

            if(i != unitActionList.Count - 1)
                menutext += "\n";
            

        }


        return menutext;
    }

    */

    public void selectAction(int actionid)
    {
        if (GameObject.Find("Main Camera").GetComponent<WebsocketAccessor>().enabled == true)
        {
            GameObject.Find("Main Camera").GetComponent<WebsocketAccessor>().sendws("SA-"+actionid);
        }

        nowCursorPosition = actionid;
        GameObject.Find("Main Camera").GetComponent<GameMgr>().pushA(true);
    }

    public ACTION getSelectedAction()
    {
        return actionList[nowCursorPosition];
    }
}
