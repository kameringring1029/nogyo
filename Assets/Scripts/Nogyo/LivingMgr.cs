using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using TMPro;

public class LivingMgr : MonoBehaviour
{
    PlayerData playerdata;
    static int retainedday = 0;

    void Start()
    {
        // プレイヤーデータ
        playerdata = PlayerData.getinstance();

        //カレンダー日付の更新
        GameObject.Find("CalenderText").GetComponent<TextMeshProUGUI>().text = playerdata.day.ToString("D2");


        // 寝起き
        if(retainedday != playerdata.day)
        {        
            // クリア---
            if (playerdata.itembox.getItemById("Harv_Grape") != null)
            {
                playerdata.day = 10;
            }
            
            retainedday = playerdata.day;
            SceneManager.LoadScene("Event");
        }
        else
        {

            //タイムオーバー
            if (playerdata.day > 7)
            {
                SceneManager.LoadScene("GameMenu");
            }
        }



    }


    void Update()
    {
        
    }

    public void openItemListMenu()
    {
        GameObject ItemList = Instantiate(Resources.Load<GameObject>("Prefab/Nogyo/ItemMenuPanel"), GameObject.Find("LivingCanvas").transform);
        ItemList.GetComponent<ItemMenu>().Activate();
    }

    public void startNogyo()
    {
        SceneManager.LoadScene("Nogyo");
    }

    public void onClickHelp()
    {
        GameObject[] helps = GameObject.FindGameObjectsWithTag("HelpWindow");
        foreach(GameObject help in helps)
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
