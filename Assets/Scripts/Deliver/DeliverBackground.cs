using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 背景にアタッチして動かすやつ */

public class DeliverBackground : MonoBehaviour
{
    public GameObject pair; // ループ用のペアオブジェクト、Inspectorから指定しちゃう
    float distance = 0f; // 手前からの距離

    Sprite[] sprsheet;
    int spr_num_bef = 0; // 前回表示のSpriteばんごう


    // Start is called before the first frame update
    void Start()
    {
        distance = getDistanceByLayer(gameObject.layer);

        // Spriteシートを読み込み
        Sprite bef = gameObject.GetComponent<SpriteRenderer>().sprite;
        sprsheet = Resources.LoadAll<Sprite>("deliver/" + bef.name.Split('_')[0] + "_" + bef.name.Split('_')[1]);


    }

    // Update is called once per frame
    void Update()
    {
        // ゲーム移動スピードとオブジェクトの設置距離からオブジェクトの移動量を計算し移動
        transform.Translate(DeliverGameManager.Instance.speed / distance, 0, 0); 

    }


    /* 画面外に出た時の処理
     * 背景ループするようにしようね
     */
    private void OnBecameInvisible()
    { 
            transform.position = pair.transform.position - new Vector3( pair.transform.localScale.x * 2.5f,0,0) ;
            randSprite();
       
    }


    /* ランダムに背景Spriteを変える処理にしたいな
     * 
     */
    void randSprite() {

        // 設定確率によってSprite番号をランダム設定
        int rand = Random.Range(0, 100);
        if (rand > getRandSpritePercentByLayer(gameObject.layer) || // 確率取得しrandと比較
            sprsheet.Length == 1 || // イレギュラーSpriteがない場合はそのまま0
            spr_num_bef != 0) // イレギュラーSpriteが連続しない様に
        {
            rand = 0; //標準Sprite
        }
        else //イレギュラーSprite
        {
            rand = Random.Range(1, sprsheet.Length);
        }

        // Spriteけってい
        gameObject.GetComponent<SpriteRenderer>().sprite = sprsheet[rand];

        // 前回のSprite番号として保管
        spr_num_bef = rand;
    }


    /*
     * レイヤ番号から奥行き距離を取得
     * layer: レイヤ番号
     * return: 奥行き距離
     */
    static float getDistanceByLayer(int layer)
    {
        float dist = 0;
        string layername = LayerMask.LayerToName(layer);

        // オブジェクトによって手前からの距離を設定
        switch (layername)
        {
            case "guardrail": // ガードレール
                dist = 2.8f; break;
            case "wall": // かべ
                dist = 5f; break;
            case "background_fore": // ビル前景
                dist = 100f; break;

        }

        return dist;
    }

    /*
     *
     */
    static int getRandSpritePercentByLayer(int layer)
    {

        int percent = 0;
        string layername = LayerMask.LayerToName(layer);

        // オブジェクトによって
        switch (layername)
        {
            case "guardrail": // ガードレール
                percent = 5; break;
            case "wall": // かべ
                percent = 20; break;
            case "background_fore": // ビル前景
                percent = 30; break;

        }

        return percent;
    }
}
