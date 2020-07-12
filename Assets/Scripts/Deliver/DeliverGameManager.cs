using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliverGameManager : SingletonMonoBehaviour<DeliverGameManager>
{
    public enum RIDE { RIDE, WALK };
    public RIDE rider_state = RIDE.RIDE;

    public float speed { get; private set; } //今のスピード
    bool flg_break; //ブレーキかけてるフラグ

    float speed_max = 2f; // 最高速度
    float speed_acce = 0.01f; // 加速度

    // Start is called before the first frame update
    void Start()
    {

        flg_break = false;
        speed = 0;
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
            case true: /*減速時*/

                //
                if (speed <= 0) {
                    speed = 0;
                    break;
                }

                // 減速
                if (speed > 0f) speed -= speed_acce*2;

               break;

            case false: /*加速時*/
                
                // 加速
                if (speed < speed_max) speed += speed_acce;

                break;
        }


        DeliverRider.Instance.GetComponent<Animator>().SetFloat("speed", speed);

    }

    /*
     * ブレーキかける/加速するトリガー処理
     */
    public void trgBrake(){
        flg_break = !flg_break;

        switch (flg_break)
        {
            case true:
                DeliverRider.Instance.GetComponent<Animator>().SetBool("isBreaking", true);
                break;
            case false:
                DeliverRider.Instance.GetComponent<Animator>().SetBool("isBreaking", false);
                break;
        }
    }

    /*
     *
     */
    public void changeRiderState(RIDE state)
    {
        if (state == rider_state) return; // 今のstateと変化なければなにもしない

        switch (state)
        {
            case RIDE.RIDE:
                Debug.Log("change to Ride");
                speed_max = 2.0f;
                speed_acce = 0.01f;
                DeliverRider.Instance.GetComponent<Animator>().SetInteger("state",0);

                DeliverBespa.Instance.GetComponent<SpriteRenderer>().enabled = false;

                DeliverBespa.Instance.mode = DeliverBespa.MODE.bike;

                break;

            case RIDE.WALK:
                Debug.Log("change to walk");
                speed_max = 0.5f;
                speed_acce = 0.003f;
                DeliverRider.Instance.GetComponent<Animator>().SetInteger("state", 1);

                DeliverBespa.Instance.GetComponent<SpriteRenderer>().enabled = true;

                break;
        }

        rider_state = state;
    }
}
