using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Information;

/*
 * セーブデータ用のクラス
 * これもシングルトンにしました
 */
[System.Serializable]
public sealed class PlayerData:MonoBehaviour
{
    // 唯一のインスタンス
    private static PlayerData instance = new PlayerData(false);

    // インスタンスのゲッタ
    public static PlayerData getinstance()
    {
        return instance;
    }


    public int day;
    public Wallet wallet;
    public BalconyState[] balconies;
    public ItemBox itembox;

    private PlayerData(bool load)
    {
        switch (load)
        {
            case true:
                loadGame();
                break;
            case false:
                newGame();
                break;
        }
        
    }


    /* にゅーげーむ */
    public void newGame()
    {
        day = 1;
        wallet = new Wallet(1000);
        balconies = new BalconyState[1];
        itembox = new ItemBox();

        fortest();

        // newバルコニー作成
        coodinate[] plantpos = new coodinate[5] { new coodinate(0, 0), new coodinate(1, 1), new coodinate(0, 1),  new coodinate(2, 1), new coodinate(3, 1) };
     //   coodinate[] plantpos = new coodinate[3] { new coodinate(0, 0), new coodinate(1, 0), new coodinate(2, 0) };

        createBalcony(GardenInfoUtil.BALCONY.Balcony1, plantpos);
    }

    /* ろーどげーむ */
    void loadGame()
    {

    }



    /*
     * 新規に農園を用意
     */
    private void createBalcony(GardenInfoUtil.BALCONY name, coodinate[] plantpos)
    {
        balconies[(int)name] = new BalconyState(plantpos);
    }



    void fortest()
    {
        // てすとアイテムリストの入力
        itembox.changeItemNum(NogyoItemDB.getinstance().db["Water_Normal"], 30);
        itembox.changeItemNum(NogyoItemDB.getinstance().db["Chemi_Normal"], 5);
        itembox.changeItemNum(NogyoItemDB.getinstance().db["Seed_GMary"], 3);
        itembox.changeItemNum(NogyoItemDB.getinstance().db["Seed_WClover"], 1);
        itembox.changeItemNum(NogyoItemDB.getinstance().db["Seed_Carrot"], 1);
        itembox.changeItemNum(NogyoItemDB.getinstance().db["Harv_Mikan"], 5);
        itembox.changeItemNum(NogyoItemDB.getinstance().db["Harv_SBerry"], 5);
        itembox.changeItemNum(NogyoItemDB.getinstance().db["Harv_SMuscat"], 2);
        itembox.showItems();

    }
    


    /* 所持アイテム一覧を表示 */
    public void openItemListMenu()
    {
        GameObject ItemList = Instantiate(Resources.Load<GameObject>("Prefab/Nogyo/ItemMenuPanel"), GameObject.Find("NogyoCanvas").transform);
        ItemList.GetComponent<ItemMenu>().Activate();
    }
}
