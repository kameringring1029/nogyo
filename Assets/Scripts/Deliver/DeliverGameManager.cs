using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliverGameManager : SingletonMonoBehaviour<DeliverGameManager>
{
    public float speed { get; private set; } //今のスピード
    bool flg_break; //ブレーキかけてるフラグ

    float speed_max = 2f; // 最高速度
    float speed_acce = 0.01f; // 加速度

    // Start is called before the first frame update
    void Start()
    {
        flg_break = false;
        speed = speed_max;
    }

    // Update is called once per frame
    void Update()
    {
        accel_brake();
    }


    /*
     * 加速/減速処理
     */
    void accel_brake()
    {

        switch (flg_break)
        {
            case true: //減速
                if (speed <= 0) { speed = 0; break; }
                if (speed > 0f) speed -= speed_acce;
               break;

            case false: //加速
                if (speed < speed_max) speed += speed_acce;
                break;
        }

    }

    /*
     * ブレーキかける/加速するトリガー処理
     */
    public void trgBrake(){
        flg_break = !flg_break;
    }
}
