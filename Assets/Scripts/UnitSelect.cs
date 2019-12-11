using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using Information;
using General;
using UnityEngine.UI;

/*
 * ユニット選択画面のマネージャ用クラス
 * WholeMgrでインスタンス化
 */
public class UnitSelect : MonoBehaviour
{

    public List<int> selectedUnits = new List<int>();

    private List<GameObject> unitselectcursor = new List<GameObject>();

    GameObject wholecursorIcon;
    int wholecursor;
    UNITGROUP nowgroup = UNITGROUP.MUSE;

    private GameObject musePanel;
    private GameObject aqoursPanel;

    private GameObject[] unitButtons = new GameObject[20];
    private GameObject[] unitButtonsArea = new GameObject[20];

    private GameObject displayMuseButton;
    private GameObject displayAqoursButton;
    private GameObject unitSelectOkButton;

    private GameObject unitDiscriptionPanel;
    private GameObject unitDiscriptionTextStatus;
    private GameObject unitDiscriptionTextChara;
    private GameObject unitDiscriptionAnim;

    private GameObject joinStatus;



    public void init()
    {

        wholecursorIcon = Instantiate(Resources.Load<GameObject>("Prefab/wholecursor"), GameObject.Find("Canvas").transform);

        wholecursorIcon.GetComponent<RectTransform>().Rotate(new Vector3(0, 0, 1), -90);
        wholecursorIcon.GetComponent<RectTransform>().localScale = wholecursorIcon.GetComponent<RectTransform>().localScale / 2;
        

        // 各種ゲームオブジェクトの取得
        // ユニットボタンについては存在するものは代入されるけどそうじゃなければNULL

        // ユニットボタンのオブジェクトを配列の該当unitidの位置に準備
        for (int i = 1; i <= 9; i++)
        {
            unitButtons[i] = GameObject.Find("ButtonMuse0" + (i));
            unitButtons[i + 1 + 9] = GameObject.Find("ButtonAqours0" + (i));

            unitButtonsArea[i] = GameObject.Find("Muse0" + (i));
            unitButtonsArea[i + 1 + 9] = GameObject.Find("Aqours0" + (i));

        }

        displayMuseButton = GameObject.Find("DisplayMuseButton");
        displayAqoursButton = GameObject.Find("DisplayAqoursButton");
        unitSelectOkButton = GameObject.Find("UnitSelectOkButton");

        musePanel = GameObject.Find("UnitSelectMusePanel");
        aqoursPanel = GameObject.Find("UnitSelectAqoursPanel");

        unitDiscriptionPanel = GameObject.Find("UnitDiscriptionPanel");
        unitDiscriptionTextStatus = GameObject.Find("UnitDiscriptionTextStatus");
        unitDiscriptionTextChara = GameObject.Find("UnitDiscriptionTextChara");
        unitDiscriptionAnim = GameObject.Find("UnitDiscriptionAnim");

        joinStatus = GameObject.Find("JoinStatus");

        // カーソル初期化
        displayAqours();

    }


    //--- 指定したユニットが選択中かどうか確認して選択メソッドか選択外しメソッドに移行 ---//
    public void switchselectUnit(int unitid)
    {
        // ユニット説明ウィンドウの更新
        //unitDiscriptionAnim.GetComponent<Animator>().SetInteger("unitid", unitid);

        // 各メソッドへ移行
        if (!selectedUnits.Contains(unitid))
        {
            selectUnit(unitid);
        }
        else
        {
            unselectUnit(unitid);
        }

    }


    //--- 指定したユニットを選択中に ---//
    public void selectUnit(int unitid)
    {
        Debug.Log("select unitid :"+unitid);

        if (selectedUnits.Count < 3)
        {
            // リスト上の変更
            selectedUnits.Add(unitid);

            // 選択アニメーション遷移
            unitButtons[unitid].GetComponent<Animator>().SetBool("selected", true);

            // 選択中ユニットに選択中マークをつけるよ
            GameObject tmpcursor = Instantiate(Resources.Load<GameObject>("Prefab/unitcursor"), unitButtons[unitid].transform);
            tmpcursor.name = "unitselectcursor" + unitid;
            unitselectcursor.Add(tmpcursor);

            // Discription欄の参加中表示
            joinStatus.GetComponent<Image>().sprite = Resources.LoadAll<Sprite>("UI/Join")[1];

        }
    }

    //--- 指定したユニットを非選択中に ---//
    public void unselectUnit(int unitid)
    {
        Debug.Log("unselect unitid :" + unitid);

        // リスト上の変更
        selectedUnits.Remove(unitid);
        
        // 非選択アニメーション遷移
        unitButtons[unitid].GetComponent<Animator>().SetBool("selected", false);

        // 選択中マークを外すよ
        for (int i = 0; i < unitselectcursor.Count; i++)
        {
            GameObject tmpcursor = unitselectcursor[i];
            if (tmpcursor.name == "unitselectcursor" + unitid)
            {
                unitselectcursor.RemoveAt(i);
                Destroy(tmpcursor);
            }
        }

        // Discription欄の非参加表示
        joinStatus.GetComponent<Image>().sprite = Resources.LoadAll<Sprite>("UI/Join")[0];

    }


    // ユニット選択完了
    public void finishSelectUnit()
    {
        Destroy(wholecursorIcon);

        if (selectedUnits.Count == 0)
        {
            int[] rand = UnitStatusUtil.randunit(3);
            foreach (int r in rand) selectedUnits.Add(r);
        }


        switch(GameObject.Find("Main Camera").GetComponent<WebsocketAccessor>().enabled){
            case false:
                GameObject.Find("Main Camera").GetComponent<WholeMgr>().startGame();
                break;
            case true:
                string m = "";
                foreach(int unit in selectedUnits)
                {
                    if (m != "") m += ",";
                    m += unit;
                }
                GameObject.Find("Main Camera").GetComponent<WebsocketAccessor>().sendws("setUnits;" + m);
                Instantiate(Resources.Load<GameObject>("Prefab/LoadingPanel"), GameObject.Find("Canvas").transform);
                break;
        }
    }



    //--- ユニット選択タブの切り替え ---//
    public void displayMuse()
    {
        nowgroup = UNITGROUP.MUSE;

        // aqourspanelを親の中で最背面に, タブボタンを明るく
        aqoursPanel.transform.SetAsFirstSibling();
        displayAqoursButton.GetComponent<Image>().color = new Color(255f, 255f, 255f, 255f);
        displayMuseButton.GetComponent<Image>().color = new Color(255f, 255f, 255f, 0f);

        // カーソル移動処理
        wholecursor = -1; pushArrow(0, 1);
    }
    public void displayAqours()
    {
        nowgroup = UNITGROUP.AQOURS;

        // musepanelを親の中で最背面に, タブボタンを明るく
        musePanel.transform.SetAsFirstSibling();
        displayMuseButton.GetComponent<Image>().color = new Color(255f, 255f, 255f, 255f);
        displayAqoursButton.GetComponent<Image>().color = new Color(255f, 255f, 255f, 0f);

        // カーソル移動処理
        wholecursor = 18; pushArrow(0, 1);
    }


    private void OnDestroy()
    {

    }


    //==== コントローラ対応制御 ====//


    public void pushArrow(int x, int y)
    {

        GameObject nextCursorTarget = null;

        if (wholecursor < 100)
        {
            // 選択できるユニットが見つかるまでカーソル移動
            do
            {
                wholecursor += x + y;
                if (wholecursor <= 0) wholecursor = 9;
                else if (wholecursor == 10 && x + y > 0) wholecursor = 1;
                else if (wholecursor == 11 && x + y < 0) wholecursor = 19;
                else if (wholecursor > 19) wholecursor = 11;

            } while (unitButtons[wholecursor] == null);

            nextCursorTarget = unitButtons[wholecursor];

        }
        else
        {

        }
        

        pushUnitButton(wholecursor);

    }

    //-- 指定したUnit IDのSprite上にカーソル移動して説明ウィンドウを更新 --//
    public void pushUnitButton(int unitid)
    {
        wholecursor = unitid;
        GameObject nextCursorTarget =  unitButtons[unitid];

        // カーソル移動
        wholecursorIcon.GetComponent<RectTransform>().position
            = nextCursorTarget.GetComponent<RectTransform>().position + new Vector3(0, nextCursorTarget.GetComponent<RectTransform>().sizeDelta[1] / 6, 0);

        // ユニット説明ウィンドウの更新
        unitDiscriptionAnim.GetComponent<Animator>().SetInteger("unitid", wholecursor);
        unitDiscriptionTextStatus.GetComponent<TextMeshProUGUI>().text = UnitStatusUtil.outputUnitInfo(wholecursor);
        unitDiscriptionTextChara.GetComponent<TextMeshProUGUI>().text
            = "<size=24><b>とくちょう</b></size>\n\n" + UnitStatusUtil.search(wholecursor).status_description();

        // 参加中ボタンの表示を更新
        if (selectedUnits.Contains(unitid))
        {
            joinStatus.GetComponent<Image>().sprite = Resources.LoadAll<Sprite>("UI/Join")[1];
        }
        else
        {
            joinStatus.GetComponent<Image>().sprite = Resources.LoadAll<Sprite>("UI/Join")[0];
        }
    }


    public void pushA()
    {
        if (wholecursor < 100)
        {
            // ユニット選択

            switchselectUnit(wholecursor);
        }
        else if (wholecursor == 100)
        {
            // ユニット選択完了
            finishSelectUnit();
            
        }

    }

    public void pushB()
    {
        //TODO 決定キャンセルにしたい

        // 決定ボタンにカーソル移動
        wholecursor = 100; nowgroup = UNITGROUP.ENEMY;
        wholecursorIcon.GetComponent<RectTransform>().position = unitSelectOkButton.GetComponent<RectTransform>().position;

    }

    public void pushR()
    {
        // グループの切り替え　ENEMYは決定ボタン
        switch (nowgroup)
        {
            case UNITGROUP.MUSE:
                displayAqours();
                break;
            case UNITGROUP.AQOURS:
                wholecursor = 100; nowgroup = UNITGROUP.ENEMY;
                wholecursorIcon.GetComponent<RectTransform>().position = unitSelectOkButton.GetComponent<RectTransform>().position;
                break;
            case UNITGROUP.ENEMY:
                displayMuse();
                break;
        }

    }

    public void pushL()
    {
        // グループの切り替え(逆順)　ENEMYは決定ボタン
        switch (nowgroup)
        {
            case UNITGROUP.MUSE:
                wholecursor = 100; nowgroup = UNITGROUP.ENEMY;
                wholecursorIcon.GetComponent<RectTransform>().position = unitSelectOkButton.GetComponent<RectTransform>().position;
                break;
            case UNITGROUP.AQOURS:
                displayMuse();
                break;
            case UNITGROUP.ENEMY:
                displayAqours();
                break;
        }
    }


    //==== コントローラ対応制御ここまで ====//
}
