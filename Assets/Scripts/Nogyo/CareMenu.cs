using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CareMenu : MonoBehaviour
{
    public string selected { get; private set; }
    public NogyoItem.NogyoItemGroup care;

    public void Activate(NogyoItem.NogyoItemGroup care, ItemBox itembox)
    {
        this.care = care;
        selected = "";

        // アイテムボタンリスト制作

        // 表示アイテムのリスト
        ItemBox itemlist = new ItemBox();

        // itemboxの中から今回のitemリストの種別に合うものを抽出
        foreach (NogyoItem item in itembox.items)
        {
            if(item.group == care)
                itemlist.changeItemNum(item, item.qty);
        }

        ButtonList.buttonStrExecWrapper carefunc = selectItem;

        ButtonList.setItemButtonList(itemlist, carefunc, GameObject.Find("CareMenuContent").GetComponent<RectTransform>());

    }

    //itemボタンが押下されたときに呼ばれることになる関数
    public void selectItem(string itemid)
    {
        selected = itemid;
    }
    public void cancel()
    {
        selected = "END";
    }
}
