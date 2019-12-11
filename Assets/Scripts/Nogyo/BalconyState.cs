using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NogyoGeneral;

using Information;

/*
 * 農園の状況管理クラス
 */
[System.Serializable]
public class BalconyState
{
    public GardenInfoUtil.BALCONY btype = GardenInfoUtil.BALCONY.Balcony1;
    public coodinate[] plantpos;
    public Produce[] produces;

    public BalconyState()
    {
        int size = GardenInfoUtil.getGardenMass(btype)[0] * GardenInfoUtil.getGardenMass(btype)[1];
        produces = new Produce[size];
        for (int i = 0; i < produces.Length; i++) { produces[i] = new Produce(); }

    }

    public BalconyState(coodinate[] plantpos)
    {
        this.plantpos = plantpos;
        produces = new Produce[plantpos.Length];
        for(int i=0; i<produces.Length; i++){ produces[i] = new Produce(); }
    }

    /* 作物を植える */
    public void plantProduce(int position, Produce.PRODUCE_TYPE type, NogyoItem.NogyoItemGroup group)
    {
        produces[position] = new Produce(type, group);
    }

    /* 作物の状態を進める */
    public void proceedProduceState(int position)
    {
        if (produces[position].type != Produce.PRODUCE_TYPE.Not)
        {
            // proceedstate実行、Vanishなら作物を消す
            if (produces[position].endDay() == Produce.PRODUCE_STATE.Vanish)
            {
                removeProduce(position);
            }
        }

    }

    /* 作物を取り除く */
    public void removeProduce(int position)
    {
        produces[position] = new Produce();
        Debug.Log("remove:" + position);
    }

    /* 作物の収穫 */
    public Produce harvestProduce(int position)
    {
        if(produces[position].status == Produce.PRODUCE_STATE.Harvest)
        {
            Produce prod = produces[position];
            removeProduce(position);
            return prod; 
        }
        else
        {
            return null;
        }

    }
        


}
