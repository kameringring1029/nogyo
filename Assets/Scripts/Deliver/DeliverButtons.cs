using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliverButtons : MonoBehaviour
{
    //
    public void onClickCurve() {
        DeliverRider.Instance.setCurve();
    }

    public void onClickBrake()
	{
        DeliverGameManager.Instance.trgBrake();
        switch (DeliverGameManager.Instance.flg_break)
        {
            case true:
                GetComponent<Animator>().SetBool("isBreaking", true);
                break;
            case false:
                GetComponent<Animator>().SetBool("isBreaking", false);
                break;
        }
	}

    public void onClickRide()
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
