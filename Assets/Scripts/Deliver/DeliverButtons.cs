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
	}
}
