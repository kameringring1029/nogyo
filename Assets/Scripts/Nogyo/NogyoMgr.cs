using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using TMPro;

using Information;

public class NogyoMgr : MonoBehaviour
{
    PlayerData playerdata;
    Garden garden;

    int actbalcony = 0;

    public int selectedBlockId;

    public enum NOWMODE { Main, OpeningMenu, InEffect}
    NOWMODE mode;

    GameObject openingItemMenu;

    // Start is called before the first frame update
    void Start()
    {
        mode = NOWMODE.Main;
        openingItemMenu = null;

        // プレイヤーデータ新規作成
        playerdata = PlayerData.getinstance();

        // プラントSpriteを配置
        garden = new Garden();
        int[] size = GardenInfoUtil.getGardenMass(GardenInfoUtil.BALCONY.Balcony1);
        garden.positioningPlants(size[0], size[1], playerdata.balconies[actbalcony].plantpos);

        // Sprite更新
        selectPlant(0);
        renewBalcorySprites(actbalcony);

        //カレンダー日付の更新
        GameObject.Find("CalenderText").GetComponent<TextMeshProUGUI>().text = playerdata.day.ToString("D2");
    }

    void Update()
    {
        // itemメニュー画面が開いているとき
        if (openingItemMenu)
        {
            //itemが指定されたら
            if (openingItemMenu.GetComponent<CareMenu>().selected != "")
            {
                // キャンセルじゃなかったら
                if (openingItemMenu.GetComponent<CareMenu>().selected != "END") 
                {
                    // アイテムIDからアイテム情報を取得
                    NogyoItem selecteditem = null; 
                   foreach (NogyoItem item in playerdata.itembox.items)
                   {
                      if (item.id == openingItemMenu.GetComponent<CareMenu>().selected) selecteditem = item;
                   }

                    // アイテム数を減少
                    int num = playerdata.itembox.changeItemNum(selecteditem, -1);
                   if (num == -1) return; // 持ってないのに使えないよ

                    // care種別で処理分岐
                    switch (openingItemMenu.GetComponent<CareMenu>().care)
                    {
                       case NogyoItem.NogyoItemGroup.Seed: // 種植え
                            playerdata.balconies[actbalcony].plantProduce(selectedBlockId, selecteditem.producetype, selecteditem.group);
                            renewBalcorySprites(actbalcony);
 
                            break;

                        case NogyoItem.NogyoItemGroup.Water: // 散水
                            // 作物に散水処理
                            if(playerdata.balconies[actbalcony].produces[selectedBlockId].type != Produce.PRODUCE_TYPE.Not)
                                playerdata.balconies[actbalcony].produces[selectedBlockId].watering(selecteditem);

                            // Spriteを更新
                            coodinate pos = playerdata.balconies[actbalcony].plantpos[selectedBlockId];
                            garden.wateringProduce(pos, true);

                            break;

                        case NogyoItem.NogyoItemGroup.Chemi: // 肥料まき
                            // 作物に施肥処理
                            if (playerdata.balconies[actbalcony].produces[selectedBlockId].type != Produce.PRODUCE_TYPE.Not)
                                playerdata.balconies[actbalcony].produces[selectedBlockId].fertilize(selecteditem);
                            break;
                    }
               }

                // メニューを削除
                Destroy(openingItemMenu.transform.parent.gameObject);
                openingItemMenu = null;

                mode = NOWMODE.Main;
            }
        }
    }



    /*対象のBalconyすべての作物のSriteをユーザデータをもとに更新 */
    void renewBalcorySprites(int balconyno)
    {
        for (int i = 0; i < playerdata.balconies[balconyno].plantpos.Length; i++)
        {
            BalconyState balcony = playerdata.balconies[balconyno];
            if (balcony.produces[i].type != Produce.PRODUCE_TYPE.Not)
            {
                garden.renewProduce(balcony.plantpos[i], balcony.produces[i].type, balcony.produces[i].status);
            }
            else
            {
                garden.renewProduce(balcony.plantpos[i], Produce.PRODUCE_TYPE.Not, Produce.PRODUCE_STATE.Vanish);
            }
        }

        renewViewInfo();
    }

    /* プラント選択 */
    public void selectPlant(int selected)
    {
        selectedBlockId = selected;

        garden.renewCursor(playerdata.balconies[actbalcony].plantpos[selectedBlockId]);
        renewViewInfo();
    }


    /* 一日の終りだよ */
    void endDay()
    {
        Debug.Log(JsonUtility.ToJson(playerdata));

        //
        GameObject blindpanel =
            Instantiate(Resources.Load<GameObject>("Prefab/BlindPanel"), GameObject.Find("NogyoCanvas").transform);

        BlindPanel.atBlind func = startDay;
        blindpanel.GetComponent<BlindPanel>().initfornogyo(func);

        playerdata.day += 1;
    }
    /* 一日の始まりだよ */
    public void startDay()
    {
        // バルコニー内の全作物を成長、乾かす
        for (int i = 0; i < playerdata.balconies[actbalcony].plantpos.Length; i++)
        {
            playerdata.balconies[actbalcony].proceedProduceState(i);

            coodinate pos = playerdata.balconies[actbalcony].plantpos[i];
            garden.wateringProduce(pos, false);
        }

        // Spite情報を更新
        renewBalcorySprites(actbalcony);

        SceneManager.LoadScene("NogyoLiving");
    }


    /* ViewPanelの情報更新 */
    void renewViewInfo()
    {
        /* 選択Blockのposition */
        coodinate pos = playerdata.balconies[actbalcony].plantpos[selectedBlockId];

        Debug.Log(playerdata.balconies[actbalcony].produces[selectedBlockId]);

        Produce prod = null;
        //対象の作物が存在している場合
        if (playerdata.balconies[actbalcony].produces[selectedBlockId].type != Produce.PRODUCE_TYPE.Not)
            prod = playerdata.balconies[actbalcony].produces[selectedBlockId];
        //更新
        GameObject.Find("ProduceViewPanel").GetComponent<ProduceView>().renew(garden.FieldBlocks[pos], prod);

        // CareButtonをアレする
        if(prod == null)
        {
            GameObject.Find("SeedButton").transform.GetChild(1).GetComponent<Text>().text = "たね";
            GameObject.Find("SeedButton").GetComponent<Image>().color = new Color(1f, 1f, 1f);
        }
        else if(prod.status != Produce.PRODUCE_STATE.Harvest) //植わってるけどまだ収穫できないよ
        {
            GameObject.Find("SeedButton").transform.GetChild(1).GetComponent<Text>().text = "成長中";
            GameObject.Find("SeedButton").GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
        }
        else
        {
            GameObject.Find("SeedButton").transform.GetChild(1).GetComponent<Text>().text = "収穫";
            GameObject.Find("SeedButton").GetComponent<Image>().color = new Color(1f, 1f, 1f);
        }
    }


    
    /* おせわめにゅーひらく */
    void openCareMenu(NogyoItem.NogyoItemGroup care)
    {
        if(mode == NOWMODE.Main)
        {
            mode = NOWMODE.OpeningMenu;
            GameObject ItemMenuPanel = Instantiate(Resources.Load<GameObject>("Prefab/Nogyo/CareMenuPanel"), GameObject.Find("NogyoCanvas").transform);
            openingItemMenu = ItemMenuPanel.transform.GetChild(0).gameObject;
            openingItemMenu.GetComponent<CareMenu>().Activate(care, playerdata.itembox);
        }
        else
        {
            openingItemMenu.GetComponent<CareMenu>().cancel();
        }

    }
    public void openSeedMenu()
    {
        // 作物が植えられていない場合のみ
        if (playerdata.balconies[actbalcony].produces[selectedBlockId].type ==  Produce.PRODUCE_TYPE.Not) 
        {
            openCareMenu(NogyoItem.NogyoItemGroup.Seed);
        }
        //収穫
        else if (playerdata.balconies[actbalcony].produces[selectedBlockId].status == Produce.PRODUCE_STATE.Harvest) 
        {
            Produce harv = playerdata.balconies[actbalcony].harvestProduce(selectedBlockId);
            playerdata.itembox.changeItemNum(NogyoItemDB.getinstance().getItemFromPType(NogyoItemDB.getGroupFromType(harv.type), harv.type), 1);
            renewBalcorySprites(actbalcony);
        }
    }
    public void openWaterMenu()
    {
        openCareMenu(NogyoItem.NogyoItemGroup.Water);
    }
    public void openChemiMenu()
    {
        openCareMenu(NogyoItem.NogyoItemGroup.Chemi);
    }

    public void openItemListMenu()
    {
        GameObject ItemList = Instantiate(Resources.Load<GameObject>("Prefab/Nogyo/ItemMenuPanel"), GameObject.Find("NogyoCanvas").transform);
        ItemList.GetComponent<ItemMenu>().Activate();
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
