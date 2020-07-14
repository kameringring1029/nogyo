using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliverBespa : SingletonMonoBehaviour<DeliverBespa>
{
    float distance = 2.8f;

    float movespeed = 0.03f;

    public enum MODE { bike, fly}
    public MODE mode { private set; get; }

    float elapsedtime = 0f;

    float[] deltaxy = new float[2];

    public Vector3 pos_orig;


    public PolygonCollider2D[] collider = new PolygonCollider2D[2];

    // Start is called before the first frame update
    void Start()
    {
        pos_orig = transform.position;

        setMode(MODE.bike);

    }

    // Update is called once per frame
    void Update()
    {

        // バイク置いてる状態
        if(mode==MODE.bike && DeliverGameManager.Instance.rider_state == DeliverGameManager.RIDE.WALK)
        {
            // ゲーム移動スピードとオブジェクトの設置距離からオブジェクトの移動量を計算し移動
            transform.Translate(DeliverGameManager.Instance.speed / distance, 0, 0);

        }
        // 飛んでる状態
        else if (mode == MODE.fly)
        {
            // riderに向かって動く

            // 一定時間ごとに移動方向ランダム決定
            elapsedtime += Time.deltaTime;
            if(elapsedtime > 0.2f)
            {
                deltaxy[0] = Random.Range(1.00f, 1.09f);
                deltaxy[1] = Random.Range(1.9f, 2.1f);

                elapsedtime = 0f;
            }

            //
            if(transform.position.x > DeliverRider.Instance.gameObject.transform.position.x + 2f)
            {
                movespeed = 0.05f;
            }
            else if(transform.position.x < DeliverRider.Instance.gameObject.transform.position.x + 2f &&
                transform.position.x > DeliverRider.Instance.gameObject.transform.position.x + 1.5f)
            {
                movespeed = 0.02f;
            }
            else
            {
                movespeed = 0.005f;
            }

            // 移動
            transform.position = Vector3.MoveTowards(
                transform.position,
                DeliverRider.Instance.gameObject.transform.position
                    + new Vector3(deltaxy[0], deltaxy[1], 0f),
                movespeed
            );
        }


    }

    /*
     * 画面外に出た時の処理
     */
    private void OnBecameInvisible()
    {
        // 透明化したときは例外
        if (GetComponent<SpriteRenderer>().enabled == false) return;

        // 処理
        switch (mode)
        {
            case MODE.bike:
                setMode(MODE.fly);
                break;
            case MODE.fly:
                setMode(MODE.bike);
                break;
        }

    }


    /*
     * mode変更
     */
    public void setMode(MODE modeto)
    {
        switch (modeto)
        {
            case MODE.bike:
                Debug.Log("bike");
                mode = MODE.bike;
                switchCollider(MODE.bike);
                transform.position = pos_orig;
                break;
            case MODE.fly:
                Debug.Log("fly");
                mode = MODE.fly;
                switchCollider(MODE.fly);
                break;
        }

        GetComponent<Animator>().SetInteger("mode", (int)mode);
    }

    /*
     * stateへColliderを変更
     */
    void switchCollider(MODE modeto)
    {
        switch (modeto)
        {
            case MODE.bike:
                collider[0].enabled = true;
                collider[1].enabled = false;
                break;

            case MODE.fly:
                collider[0].enabled = false;
                collider[1].enabled = true;
                break;
        }
    }

    /*
     * onclick
     */
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
