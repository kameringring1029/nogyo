using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/*
 * SRPGゲームに入る前のユニット選択画面での処理
 */

public class SelectUnitButtons : MonoBehaviour {

    public int unitid;

    private void Start()
    {
    }

    public void onClicked()
    {
        GameObject.Find("Main Camera").GetComponent<WholeMgr>().unitSelect.pushUnitButton(unitid);
    }

    public void onClickOk()
    {        
        GameObject.Find("Main Camera").GetComponent<WholeMgr>().unitSelect.finishSelectUnit();
    }

    public void onClickMuse()
    {
        GameObject.Find("Main Camera").GetComponent<WholeMgr>().unitSelect.displayMuse();
    }

    public void onClickAqours()
    {
        GameObject.Find("Main Camera").GetComponent<WholeMgr>().unitSelect.displayAqours();
    }

    public void onClickJoin()
    {
        GameObject.Find("Main Camera").GetComponent<WholeMgr>().unitSelect.pushA();

    }
}
