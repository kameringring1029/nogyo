using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Information;
using General;
using System;


/*
 * WebSocketのタスクスタックアイテム用クラス 
 */
public class WSStackItem
{
    public WSITEMSORT sort;
    public mapinfo map;
    public int[] units;

    public WSStackItem(WSITEMSORT sort=WSITEMSORT.NONE, string option="")
    {
        this.sort = sort;

        switch (sort)
        {
            case WSITEMSORT.ESTROOM: // ルーム確立時にmap情報を格納
                map = JsonUtility.FromJson<mapinfo>(option);
                break;
            case WSITEMSORT.ESTUNIT:
                string[] unitstr = option.Split(',');
                units = new int[unitstr.Length];
                for (int i = 0; i < unitstr.Length; i++) units[i] = Int32.Parse(unitstr[i]);
                break;
        }

    }
}
