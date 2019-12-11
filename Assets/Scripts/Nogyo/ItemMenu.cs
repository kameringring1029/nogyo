using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class ItemMenu : MonoBehaviour
{
    ItemBox itembox;
    ItemBox itembox_bup;

    Wallet wallet;

    int come;

    GameObject commPanel;

    public enum STATUS { Normal, inSelectShip, inShop}
    STATUS status;

    public void Activate()
    {
        this.itembox = PlayerData.getinstance().itembox;
        this.wallet = PlayerData.getinstance().wallet;

        GameObject.Find("moneyText").GetComponent<TextMeshProUGUI>().text = wallet.money.ToString("D7");

        ButtonList.buttonStrExecWrapper itemfunc = selectItem;
        ButtonList.setItemButtonList(itembox, itemfunc, GameObject.Find("ItemListContent").GetComponent<RectTransform>());

        status = STATUS.Normal;
    }

    // itemボタンが押下されたときに呼ばれることになる関数
    public void selectItem(string itemid)
    {
        renewExplain(itemid);
    }

    
    // フィルタボタンが押下されたときに呼ばれることになる関数
    public void onFiltering(Dropdown dropdown)
    {
        int group = dropdown.value;

        ItemBox filterbox = itembox.filterByItemgroupForTab((NogyoItem.NogyoItemGroup)Enum.ToObject(typeof(NogyoItem.NogyoItemGroup), group));

        ButtonList.destroyAllButtons();
        ButtonList.buttonStrExecWrapper itemfunc = selectItem;
        ButtonList.setItemButtonList(filterbox, itemfunc, GameObject.Find("ItemListContent").GetComponent<RectTransform>());

        if (status == STATUS.inSelectShip) activateShipOrShop();
    }

    // 出荷ボタン押した挙動
    public void onClickShip()
    {
        come = 0;

        switch (status)
        {
            case STATUS.Normal: //出荷用表示
                status = STATUS.inSelectShip;
                activateShipOrShop();

                GameObject.Find("moneyText").GetComponent<TextMeshProUGUI>().text = "<color=red>+" + come + "</color>\n" + wallet.money.ToString("D7");

                //
                GameObject.Find("ShipButton").transform.GetChild(0).GetComponent<Text>().text = "決定";
                GameObject.Find("ShipButton").GetComponent<Image>().color = new Color(1f, 0.75f, 0.75f);

                // 電話画面開始
                activateComm();
                break;

            case STATUS.inSelectShip: // 出荷処理
                GameObject[] target = GameObject.FindGameObjectsWithTag("ScrollViewButton");
                foreach (GameObject b in target)
                {
                    // 出荷がある場合
                    if(int.Parse(b.GetComponentInChildren<TextMeshProUGUI>().text.Split('/')[0]) > 0)
                    {
                        NogyoItem item = itembox.getItemById(b.name.Split('.')[0]);
                        int shipnum = int.Parse(b.GetComponentInChildren<TextMeshProUGUI>().text.Split('/')[0]);
                        // オカネモラウ
                        wallet.money += item.price_sell * shipnum ;
                        GameObject.Find("moneyText").GetComponent<TextMeshProUGUI>().text = wallet.money.ToString("D7");
                        // 所持数から減らす
                        itembox.changeItemNum(item, -shipnum);

                        // 電話画面を終了
                        commPanel.GetComponent<CommCtrl>().destroy();
                    }
                }

                cancel();
                break;


            default: // ほかの状態のときは何もしない
                break;
        }

    }

    /* 購入ボタン押したとき */
    public void onClickShop()
    {
        come = 0;

        switch (status)
        {
            case STATUS.Normal: // 購入モードに移行
                status = STATUS.inShop;
                itembox_bup = itembox;
                itembox = new ItemBox();
                itembox.forShop();
                GameObject.Find("ItemFilterDropdown").GetComponent<Dropdown>().value = 0;
                onFiltering(GameObject.Find("ItemFilterDropdown").GetComponent<Dropdown>());
                activateShipOrShop();

                //
                GameObject.Find("BuyButton").transform.GetChild(0).GetComponent<Text>().text = "決定";
                GameObject.Find("BuyButton").GetComponent<Image>().color = new Color(1f, 0.75f, 0.75f);

                // 電話画面開始
                activateComm();
                break;


            case STATUS.inShop: // 購入決定
                if (come > wallet.money) return; //オカネナイヨ

                itembox = itembox_bup;

                GameObject[] target = GameObject.FindGameObjectsWithTag("ScrollViewButton");
                foreach (GameObject b in target)
                {
                    // k購入がある場合
                    if (int.Parse(b.GetComponentInChildren<TextMeshProUGUI>().text.Split('/')[0]) > 0)
                    {
                        NogyoItem item = NogyoItemDB.getinstance().getItemById(b.name.Split('.')[0]);
                        int buynum = int.Parse(b.GetComponentInChildren<TextMeshProUGUI>().text.Split('/')[0]);
                        // オカネモラウ
                        wallet.comeMoney(-1 * item.price_buy * buynum);
                        GameObject.Find("moneyText").GetComponent<TextMeshProUGUI>().text = wallet.money.ToString("D7");
                        // 所持数
                        itembox.changeItemNum(item, buynum);


                        // 電話画面を終了
                        commPanel.GetComponent<CommCtrl>().destroy();
                    }
                }

                cancel();

                GameObject.Find("ItemFilterDropdown").GetComponent<Dropdown>().value = 0;
                onFiltering(GameObject.Find("ItemFilterDropdown").GetComponent<Dropdown>());
                break;


            default: // ほかの状態のときは何もしない
                break;
        }

    }


    // 取引用の+-ボタン表示とか
    void activateShipOrShop()
    {
        GameObject[] target = GameObject.FindGameObjectsWithTag("ScrollViewButton");
        foreach (GameObject b in target)
        {
            // パネルのactivate
            b.transform.GetChild(2).gameObject.SetActive(true);
            // onclickの追加
            b.transform.GetChild(2).GetChild(0).gameObject.GetComponent<Button>().onClick.AddListener(() => onClickChangeNum(b, 1)); //
            b.transform.GetChild(2).GetChild(1).gameObject.GetComponent<Button>().onClick.AddListener(() => onClickChangeNum(b, -1)); //
                                                                                                                                    // 出荷数テキストの追加
            b.GetComponentInChildren<TextMeshProUGUI>().text = "0/" + b.GetComponentInChildren<TextMeshProUGUI>().text;

        }
    }

    // 取引のアイテムの数調整
    public void onClickChangeNum(GameObject b ,int vector)
    {
        int transnum = int.Parse(b.GetComponentInChildren<TextMeshProUGUI>().text.Split('/')[0]) + vector;
        int posessionnum = int.Parse(b.GetComponentInChildren<TextMeshProUGUI>().text.Split('/')[1]);


        if (transnum < 0 || transnum > posessionnum) // オーバーフロー分は無視
        {
            return;
        }else if (transnum == 1) // 取引することにしたらわかりやすく
        {
            b.GetComponent<Image>().color
                = new Color(0.76f, b.GetComponent<Image>().color.g, b.GetComponent<Image>().color.b);
        }
        else if (transnum == 0) // 取引しないことにしたら
        {
            b.GetComponent<Image>().color
                = new Color(0.36f, b.GetComponent<Image>().color.g, b.GetComponent<Image>().color.b);
        }

        // 表示の数の変更
        b.GetComponentInChildren<TextMeshProUGUI>().text
         = transnum.ToString() + "/" + posessionnum;

        switch (status)
        {
            case STATUS.inSelectShip:
                come += vector * itembox.getItemById(b.name.Split('.')[0]).price_sell;
                GameObject.Find("moneyText").GetComponent<TextMeshProUGUI>().text = "<color=red>+" + come + "</color>\n" + wallet.money.ToString("D7");

                break;
            case STATUS.inShop:
                come -= vector * itembox.getItemById(b.name.Split('.')[0]).price_buy;
                GameObject.Find("moneyText").GetComponent<TextMeshProUGUI>().text = "<color=blue>" + come + "</color>\n" + wallet.money.ToString("D7");

                break;
        }

    }


    /**/
    void activateComm()
    {
        commPanel = Instantiate(Resources.Load<GameObject>("Prefab/Nogyo/CommPanel"), gameObject.transform);
        commPanel.GetComponent<CommCtrl>().init(status);
    }


    /* 説明分と画像を更新 */
    public void renewExplain(string itemid)
    {
        Debug.Log("selectitem" + itemid);
        GameObject.Find("ItemExplainView").GetComponent<Image>().sprite
            = Resources.Load<Sprite>("Nogyo/item/" + NogyoItemDB.getinstance().db[itemid].id);

      //  GameObject.Find("ItemExplainText").GetComponent<TextMeshProUGUI>().text = NogyoItemDB.getinstance().db[itemid].shapingExplain();
        GameObject.Find("ItemExplainTitle").GetComponent<TextMeshProUGUI>().text = NogyoItemDB.getinstance().db[itemid].name;
        GameObject.Find("ItemExplainText").GetComponent<TextMeshProUGUI>().text = NogyoItemDB.getinstance().db[itemid].explain;
        GameObject.Find("ItemExplainNum").GetComponent<TextMeshProUGUI>().text = itembox.getItemById(itemid).qty.ToString();

        renewStatus(itemid);
    }
    

    /* ステータス表示の更新 */
    public void renewStatus(string itemid)
    {
        GameObject panel = GameObject.Find("ItemExlpainStatusPanel");

        for(int i=0; i<6; i++)
        {
            int value = NogyoItemDB.getinstance().db[itemid].status.getValueByNum((NogyoItemStatus.STSVALUE)Enum.ToObject(typeof(NogyoItemStatus.STSVALUE), i));
            panel.transform.GetChild(i).GetComponent<Image>().sprite = Resources.LoadAll<Sprite>("Nogyo/testtube/testtube_" + i)[value];
        }

    }

    /* 各状況からのキャンセル */
    public void cancel()
    {

        switch (status)
        {
            case STATUS.inSelectShip:
            case STATUS.inShop:

                GameObject[] target = GameObject.FindGameObjectsWithTag("ScrollViewButton");
                foreach (GameObject b in target)
                {
                    // パネルのdeactivate
                    b.transform.GetChild(2).gameObject.SetActive(false);
                    // 出荷数テキストの削除
                    b.GetComponentInChildren<TextMeshProUGUI>().text = b.GetComponentInChildren<TextMeshProUGUI>().text.Split('/')[1];
                    // 色をもとに戻す
                    b.GetComponent<Image>().color
                        = new Color(0.36f, b.GetComponent<Image>().color.g, b.GetComponent<Image>().color.b);
                    // リスナー削除
                    b.transform.GetChild(2).GetChild(0).gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
                    b.transform.GetChild(2).GetChild(1).gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
                }
                // 表示し直し
                if (status == STATUS.inShop) itembox = itembox_bup;
                onFiltering(GameObject.Find("ItemFilterDropdown").GetComponent<Dropdown>());
                GameObject.Find("moneyText").GetComponent<TextMeshProUGUI>().text = wallet.money.ToString("D7");

                // 出荷ボタンの表示初期化
                GameObject.Find("ShipButton").transform.GetChild(0).GetComponent<Text>().text = "出荷";
                GameObject.Find("ShipButton").GetComponent<Image>().color = new Color(1f, 1f, 1f);

                // おかいものボタンの表示初期化
                GameObject.Find("BuyButton").transform.GetChild(0).GetComponent<Text>().text = "おみせ";
                GameObject.Find("BuyButton").GetComponent<Image>().color = new Color(1f, 1f, 1f);


                // 電話画面を終了
                commPanel.GetComponent<CommCtrl>().destroy();

                status = STATUS.Normal;

                return;



            case STATUS.Normal: // アイテムメニュー自体の削除
                Destroy(gameObject);
                break;
        }
    }


    public void onClickHelp()
    {
        GameObject[] helps = GameObject.FindGameObjectsWithTag("HelpWindow");
        foreach (GameObject help in helps)
        {
            switch (help.GetComponent<HelpWindow>().active)
            {
                case true:
                    help.GetComponent<HelpWindow>().Activate(false); break;
                case false:
                    help.GetComponent<HelpWindow>().Activate(true); break;
            }
        }
    }
}
