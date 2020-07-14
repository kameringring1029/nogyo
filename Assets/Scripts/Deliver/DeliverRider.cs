using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliverRider : SingletonMonoBehaviour<DeliverRider>
{

    int vect_curving = 0; // カーブの向き{0:直進, 1:手前へ, -1:奥へ}
    static float scale_orig; // スタート時のScaleを格納しておく
    static float scale_incre = 0.075f; // カーブする時のインクリメント分

    // Start is called before the first frame update
    void Start()
    {
        
        scale_orig = gameObject.GetComponent<Transform>().localScale.x;
        
    }

    // Update is called once per frame
    void Update()
    {

        if (vect_curving != 0) curve(); //直進以外はカーブの動き


    }


    /* カーブ移動の処理（Updateで都度呼ばれる） */
    void curve()
    {
        // 今のサイズ
        float scale_now = gameObject.GetComponent<Transform>().localScale.x;

        switch (vect_curving)
        {
            case 0: break; // 直進
            case 1: // 手前へ
                if (scale_now >= scale_orig + scale_incre * 10) // 手前に動ききったら
                {
                    vect_curving = 0;
                    gameObject.layer = LayerMask.NameToLayer("forelane_fore");
                }
                else
                {
                    gameObject.GetComponent<Transform>().localScale += new Vector3(scale_incre, scale_incre, 0);
                }
                break;
            case -1: // 奥へ
                if (scale_now <= scale_orig) // 奥に動ききったら
                {
                    vect_curving = 0;
                    gameObject.layer = LayerMask.NameToLayer("forelane_back");
                }
                else
                {
                    gameObject.GetComponent<Transform>().localScale -= new Vector3(scale_incre, scale_incre, 0);
                }
                break;
        }
    }


    /* カーブの向きを設定する（カーブの実移動はUpdateで実行）
     * vect : カーブの向き（default : 向きにかかわらず切り返し）
     */
    public void setCurve(int vect=0) {

        if(vect == 0) {
            switch (vect_curving)
            {
                case 0: // 直進中の時、今のレーンによって手前へ移動するか奥へ移動するか
                    if (gameObject.GetComponent<Transform>().localScale.x > scale_orig)
                    {
                        vect = -1;
                    }
                    else
                    {
                        vect = 1;
                    }
                    break;
                case 1: // 切り返し
                    vect = -1;
                    break;
                case -1: // 切り返し
                    vect = 1;
                    break;

            }
        }


        vect_curving = vect;
        Debug.Log("curve:"+vect_curving);
    }


    public void onClick()
    {
        switch (DeliverGameManager.Instance.rider_state)
        {
            case DeliverGameManager.RIDE.RIDE:
                DeliverGameManager.Instance.changeRiderState(DeliverGameManager.RIDE.WALK);
                break;
            case DeliverGameManager.RIDE.WALK:
                DeliverGameManager.Instance.changeRiderState(DeliverGameManager.RIDE.RIDE);
                break;
        }
    }
}

