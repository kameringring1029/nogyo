using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 背景にアタッチして動かすやつ */

public class DeliverBackground : MonoBehaviour
{
    float distance = 0f; // 手前からの距離

    // Start is called before the first frame update
    void Start()
    {
        // オブジェクトによって手前からの距離を設定
        switch (gameObject.name.Split('_')[1])
        {
            case "g": // ガードレール
                distance = 2.8f; break;
            case "w": // かべ
                distance = 5f; break;
            case "f": // ビル前景
                distance = 100f; break;

        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(DeliverGameManager.Instance.speed / distance, 0, 0); // ゲーム移動スピードとオブジェクトの設置距離からオブジェクトの移動量を計算し移動
        if (transform.position.x > 17.5f) // 背景ループ用処理
        {
            transform.position = new Vector3(-17.5f, 0, 0);
            randSprite();
        }
    }


    /* ランダムに背景Spriteを変える処理にしたいな
     * 
     */
    void randSprite() {

        Sprite bef = gameObject.GetComponent<SpriteRenderer>().sprite;
        Sprite[] spr = Resources.LoadAll<Sprite>("deliver/" + bef.name.Split('_')[0]+"_" +bef.name.Split('_')[1]);

        Sprite aft = bef;
        int rand = Random.Range(0, 100);
		// syori

        gameObject.GetComponent<SpriteRenderer>().sprite = spr[Random.Range(0, spr.Length)];

    }
}
