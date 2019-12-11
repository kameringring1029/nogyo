using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Information;
using General;


/*
 * SRPGゲーム中のMap上のブロックにアタッチされるクラス
 * Map上の位置管理とかクリックされたときの挙動とか
 */


[RequireComponent(typeof(Rigidbody))]

public class FieldBlock : MonoBehaviour {

    public GameObject GroundedUnit { get; set; }
    public int[] position = new int[2];
    
    public GROUNDTYPE blocktype;
    public BlockInfo blockInfo;
    

	// Use this for initialization
	void Start () {        

        // ブロックの情報をGeneralから取得
        switch (blocktype)
        {
            case GROUNDTYPE.NORMAL:
                blockInfo = new Kusa();
                break;
            case GROUNDTYPE.UNMOVABLE:
                blockInfo = new Unmovable();
                break;
            case GROUNDTYPE.SEA:
                blockInfo = new Sea();
                break;
            case GROUNDTYPE.HIGH:
                blockInfo = new High();
                break;

        }
	}
	

    //--- ブロックがクリックされたときの挙動 ---//
    public void onClick()
    {
        if(GameObject.Find("Main Camera").GetComponent<GameMgr>().enabled == true)
            GameObject.Find("Main Camera").GetComponent<GameMgr>().pushBlock(position[0], position[1]);
        else if (GameObject.Find("Main Camera").GetComponent<RoomMgr>().enabled == true)
            GameObject.Find("Main Camera").GetComponent<RoomMgr>().pushBlock(position[0], position[1]);
    }


    public string outputInfo()
    {
        string outinfo =
            position[0] + "-" + position[1] +"\n" +
            "地形：" + blockInfo.type() + "" + "\n" +
            "特殊効果；" + blockInfo.groundtype() + "\n";

        Debug.Log(outinfo);

        return outinfo;
    }
}
