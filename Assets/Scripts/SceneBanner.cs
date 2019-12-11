using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using General;


/*
 * ターン遷移時とか結果表示のバナーにアタッチされる
 */

public class SceneBanner : MonoBehaviour {
    

	// Use this for initialization
	void Start () {

	}

    public void activate(CAMP turn)
    {

        switch (turn)
        {
            case CAMP.ALLY:
                // Resources/Bannerフォルダからグラをロード
                gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Banner/" + "AllyTurn");
                break;

            case CAMP.ENEMY:
                gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Banner/" + "EnemyTurn");
                break;

            case CAMP.GAMEMASTER:
                if (GameObject.Find("Main Camera").GetComponent<Map>().allyUnitList.Count > 0)
                {
                    gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Banner/" + "YouWin");
                }
                else
                {
                    gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Banner/" + "YouLose");
                }
                break;
        }


        // ユーザ操作不能にし、バナー表示⇒消失⇒シーン変更
        GameObject.Find("Main Camera").GetComponent<GameMgr>().setInEffecting(true);

        gameObject.SetActive(true);
        StartCoroutine("inactiveAfterTimer");
    }


    //--- 時間後にバナー消失⇒シーン移行 ---//
    IEnumerator inactiveAfterTimer()
    {
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
        GameObject.Find("Main Camera").GetComponent<GameMgr>().setInEffecting(false);
    }

}
