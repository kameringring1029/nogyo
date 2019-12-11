using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NogyoGeneral;

/*
 * 植えられた作物のクラス
 */
[System.Serializable]
public class Produce
{
    public enum PRODUCE_STATE { Seed, Growth, Harvest, Dead, Vanish }
    public enum PRODUCE_TYPE { Not, GMary, WClover, Carrot, Mikan, SBerry, SMuscat, Grape, Apple, Negi }

    public PRODUCE_TYPE type;
    public NogyoItem.NogyoItemGroup group;
    public PRODUCE_STATE status;
    public NogyoItem water;
    int waterremain;
    int grouthlevel;
    public NogyoItem chemi;

    public Produce(PRODUCE_TYPE type, NogyoItem.NogyoItemGroup group)
    {
        this.type = type;
        this.group = group;
        status = PRODUCE_STATE.Seed;

        this.chemi = new NogyoItem();
        this.water = new NogyoItem();
        waterremain = 0;
        grouthlevel = 0;
    }
    public Produce()
    {
        type = PRODUCE_TYPE.Not;
        group = NogyoItem.NogyoItemGroup.Null;
        status = PRODUCE_STATE.Vanish;

        chemi = new NogyoItem();
        water = new NogyoItem();
        waterremain = 0;
        grouthlevel = 0;
    }
    
    /*
     * 状態の進行
     */
    public PRODUCE_STATE proceedState()
    {

        updateState();
        
        if (chemi.id != "" && status < PRODUCE_STATE.Dead) // 肥料パワー
        {
            updateState();
        }

        Debug.Log("produce " + status);
                
        return status;
    }

    void updateState()
    {
        if (status == PRODUCE_STATE.Growth)
        {
            grouthlevel++;

            switch (NogyoItemDB.getGroupFromType(type))
            {
                case NogyoItem.NogyoItemGroup.Flower:
                    if (grouthlevel > 0) status++;
                    break;
                default:
                    if (grouthlevel > 1) status++;
                    break;
            }
        }
        else
        {
            status++;
        }
    }

    /*
     * 散水
     */
    public void watering(NogyoItem water)
    {
        this.water = water;
        waterremain = 2;
    }

    /*
     * 施肥
     */
    public void fertilize(NogyoItem chemi)
    {
        this.chemi = chemi;
    }

    /*
     * 一日の終り
     */
    public PRODUCE_STATE endDay()
    {
        proceedState();

        if (status != PRODUCE_STATE.Vanish && waterremain <= 0)
        {
            status = PRODUCE_STATE.Dead;
        }

        water = new NogyoItem();
        waterremain -= 1;
        chemi = new NogyoItem();

        return status;
    }

}
