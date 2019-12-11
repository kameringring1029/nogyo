using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using General;

/*
 * SRPGゲーム中のコントローラボタンにアタッチされて処理する用のやつ
 */


public class ControllerButtons : MonoBehaviour {

    WholeMgr WM;
    GameMgr GM;
    RoomMgr RM;
    EditMapMgr EM;

    public GameObject GC;


    float vertical, horizonal;

    // Use this for initialization
    void Start ()
    {
        WM = GameObject.Find("Main Camera").GetComponent<WholeMgr>();
        GM = GameObject.Find("Main Camera").GetComponent<GameMgr>();
        RM = GameObject.Find("Main Camera").GetComponent<RoomMgr>();
        EM = GameObject.Find("Main Camera").GetComponent<EditMapMgr>();
    }



    public void onClickUp()
    {
        switch (WM.wholemode)
        {
            case WHOLEMODE.ROOM:
                RM.pushArrow(0, -1); break;
            case WHOLEMODE.GAME:
                if(GM.gameTurn == CAMP.ALLY) GM.pushArrow(0, -1);
                break;
            case WHOLEMODE.SELECTMODE:
            case WHOLEMODE.SELECTMAP:
            case WHOLEMODE.SELECT_GAMEROOM:
                WM.pushArrow(0, -1); break;
            case WHOLEMODE.SELECTUNIT:
                WM.unitSelect.pushArrow(0, -1); break;
            case WHOLEMODE.MAPEDIT:
                EM.pushArrow(0, -1); break;

        }

    }

    public void onClickDown()
    {
        switch (WM.wholemode)
        {
            case WHOLEMODE.ROOM:
                    RM.pushArrow(0, 1); break;
            case WHOLEMODE.GAME:
                if (GM.gameTurn == CAMP.ALLY)
                    GM.pushArrow(0, 1);
                break;
            case WHOLEMODE.SELECTMODE:
            case WHOLEMODE.SELECTMAP:
            case WHOLEMODE.SELECT_GAMEROOM:
                WM.pushArrow(0, 1); break;
            case WHOLEMODE.SELECTUNIT:
                WM.unitSelect.pushArrow(0, 1); break;
            case WHOLEMODE.MAPEDIT:
                EM.pushArrow(0, 1); break;
        }
    }

    public void onClickRight()
    {
        switch (WM.wholemode)
        {
            case WHOLEMODE.ROOM:
                RM.pushArrow(1, 0); break;
            case WHOLEMODE.GAME:
                if (GM.gameTurn == CAMP.ALLY)
                    GM.pushArrow(1, 0);
                break;
            case WHOLEMODE.SELECTMODE:
            case WHOLEMODE.SELECTMAP:
            case WHOLEMODE.SELECT_GAMEROOM:
                WM.pushArrow(1, 0); break;
            case WHOLEMODE.SELECTUNIT:
                WM.unitSelect.pushArrow(1, 0); break;
            case WHOLEMODE.MAPEDIT:
                EM.pushArrow(1, 0); break;
        }
    }

    public void onClickLeft()
    {
        switch (WM.wholemode)
        {
            case WHOLEMODE.ROOM:
                RM.pushArrow(-1, 0); break;
            case WHOLEMODE.GAME:
                if (GM.gameTurn == CAMP.ALLY)
                    GM.pushArrow(-1, 0); break;
            case WHOLEMODE.SELECTMODE:
            case WHOLEMODE.SELECTMAP:
            case WHOLEMODE.SELECT_GAMEROOM:
                WM.pushArrow(-1, 0); break;
            case WHOLEMODE.SELECTUNIT:
                WM.unitSelect.pushArrow(-1, 0); break;
            case WHOLEMODE.MAPEDIT:
                EM.pushArrow(-1, 0); break;
        }
    }
    
    public void onClickA()
    {
        switch (WM.wholemode)
        {
            case WHOLEMODE.ROOM:
                RM.pushA(); break;
            case WHOLEMODE.GAME:
                if (GM.gameTurn == CAMP.ALLY)
                    GM.pushA(); break;
            case WHOLEMODE.SELECTMODE:
            case WHOLEMODE.SELECTMAP:
            case WHOLEMODE.SELECT_GAMEROOM:
                WM.pushA(); break;
            case WHOLEMODE.SELECTUNIT:
                WM.unitSelect.pushA(); break;
            case WHOLEMODE.MAPEDIT:
                EM.pushA(); break;
        }
    }

    public void onClickB()
    {
        switch (WM.wholemode)
        {
            case WHOLEMODE.ROOM:
                RM.pushB(); break;
            case WHOLEMODE.GAME:
                if (GM.gameTurn == CAMP.ALLY)
                    GM.pushB(); break;
            case WHOLEMODE.SELECTMODE:
                WM.pushB(); break;
            case WHOLEMODE.SELECTUNIT:
                WM.unitSelect.pushB(); break;
            case WHOLEMODE.MAPEDIT:
                EM.pushB(); break;
        }
    }


    public void onClickR()
    {
        switch (WM.wholemode)
        {
            case WHOLEMODE.ROOM:
                RM.pushR(); break;
            case WHOLEMODE.GAME:
                if (GM.gameTurn == CAMP.ALLY)
                    GM.pushR(); break;
            case WHOLEMODE.SELECTMODE:
                WM.pushR(); break;
            case WHOLEMODE.SELECTUNIT:
                WM.unitSelect.pushR(); break;
            case WHOLEMODE.MAPEDIT:
                EM.pushR(); break;
        }
    }

    public void onClickL()
    {
        switch (WM.wholemode)
        {
            case WHOLEMODE.ROOM:
                RM.pushL(); break;
            case WHOLEMODE.GAME:
                if (GM.gameTurn == CAMP.ALLY)
                    GM.pushL(); break;
            case WHOLEMODE.SELECTMODE:
                WM.pushL(); break;
            case WHOLEMODE.SELECTUNIT:
                WM.unitSelect.pushL(); break;
            case WHOLEMODE.MAPEDIT:
                EM.pushL(); break;
        }
    }

    public void onClickStart()
    {
        Application.LoadLevel("Main"); // Reset
    }

    public void onClickSelect()
    {
        switch (GC.activeInHierarchy)
        {
            case true:
                GC.SetActive(false);
                break;
            case false:
                GC.SetActive(true);
                break;
        }
    }
}
