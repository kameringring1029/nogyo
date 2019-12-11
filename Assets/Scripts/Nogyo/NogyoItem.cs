using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NogyoGeneral;

/*
 * アイテムのデータクラス
 */
[System.Serializable]
public class NogyoItem
{
    public enum NogyoItemGroup { Null, Seed, Flower, Vegi, Fruit, Soil, Chemi, Water, Harvest}

    public string id;
    public string name;
    public NogyoItemGroup group;
    public string explain;
    public int price_sell;
    public int price_buy;
    public Produce.PRODUCE_TYPE producetype;
    public NogyoItemStatus status;

    public int qty;

    public NogyoItem(string id, string name, string explain,  NogyoItemGroup group, int price_sell, int price_buy = -1, NogyoItemStatus status = null, Produce.PRODUCE_TYPE producetype=Produce.PRODUCE_TYPE.Not)
    {
        this.id = id;
        this.name = name;
        this.explain = explain;
        this.group = group;
        this.price_sell = price_sell;
        this.price_buy = price_buy;
        if (price_buy == -1) this.price_buy = price_sell * 3; //特に買値設定されていなければ売値の3倍
        this.producetype = producetype;
        this.status = status;
        if (status == null) this.status = new NogyoItemStatus();

        qty = 0;
    }
    public NogyoItem()
    {
        id = ""; name = ""; group = NogyoItemGroup.Null; explain = ""; price_sell = 0; price_buy = 0; producetype = Produce.PRODUCE_TYPE.Not; status = new NogyoItemStatus(); qty = 0;
    }

    /* アイテムの説明情報を返す */
    public string shapingExplain()
    {
        string expstr = "";

        expstr =
            "<size=30><color=\"white\"><b>" + name + "</b></color></size>\n" +
            explain;

        return expstr;
    }

}
