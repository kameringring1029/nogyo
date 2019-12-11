using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using General;
using Information;

public class GardenInfoUtil
{
    public enum BALCONY { Balcony1, Balcony2, Proof }

    /* 花壇のブロックの配置からSpriteを選択して返すやつ
     * mass_x/y: 花壇のサイズ、x/y：ブロックの位置
     * たとえば3,3,1,2ならyの手前側の辺だけがあるブロックSpriteを返す
     * 2,2,0,0なら奥側の角の2辺があるブロックSpriteを返す
     */
     /*
    static public Sprite getGardenBlockType(int mass_x, int mass_y, int x, int y)
    {
        int id = 0;

        if (x == 0) id += 1;
        if (y == 0) id += 2;
        if (x == mass_x - 1) id += 4;
        if (y == mass_y - 1) id += 8;


        switch (id)
        {
            case 1: id = 0; break;
            case 2: id = 1; break;
            case 4: id = 2; break;
            case 8: id = 3; break;

            case 3: id = 4; break;
            case 6: id = 5; break;
            case 9: id = 6; break;
            case 12: id = 7; break;

            case 7: id = 8; break;
            case 11: id = 9; break;
            case 13: id = 10; break;
            case 14: id = 11; break;

            case 5: id = 12; break;
            case 10: id = 13; break;
            case 15: id = 14; break;
            case 0: id = 15; break;
        }

        return Resources.LoadAll<Sprite>("Nogyo/kadan")[id] ;
    }
    */

    /* 花壇のブロックの配置からSpriteを選択して返すやつ
     * plantpos: 植える位置候補、x/y：ブロックの位置
     * たとえばx,y = 1,2ならyの手前側の辺だけがあるブロックSpriteを返す
     * x,y = 0,0なら奥側の角の2辺があるブロックSpriteを返す
     */
    static public Sprite getGardenBlockType(coodinate[] plantpos, int x, int y)
    {
        int id = 15;

        foreach(coodinate pos in plantpos)
        {
            if(x - pos.x == 0)
            {
                switch (pos.y - y)
                {
                    case -1:
                        id -= 2;
                        break;
                    case 1:
                        id -= 8;
                        break;
                }
            }
            else if(y - pos.y == 0)
            {
                switch (x - pos.x)
                {
                    case -1:
                        id -= 4;
                        break;
                    case 1:
                        id -= 1;
                        break;
                }
            }
        }

        switch (id)
        {
            case 1: id = 0; break;
            case 2: id = 1; break;
            case 4: id = 2; break;
            case 8: id = 3; break;

            case 3: id = 4; break;
            case 6: id = 5; break;
            case 9: id = 6; break;
            case 12: id = 7; break;

            case 7: id = 8; break;
            case 11: id = 9; break;
            case 13: id = 10; break;
            case 14: id = 11; break;

            case 5: id = 12; break;
            case 10: id = 13; break;
            case 15: id = 14; break;
            case 0: id = 15; break;
        }

        return Resources.LoadAll<Sprite>("Nogyo/kadan")[id];
    }


    /*
    * 作物Spriteを返すやつ
    * name: 
    */
    static public Sprite getGardenProduceSpriteByType(Produce.PRODUCE_TYPE type, Produce.PRODUCE_STATE state)
    {

        Debug.Log("de " + "Nogyo/produce/produce_" + type.ToString() +""+ (int)state);

        switch (type)
        {
            case Produce.PRODUCE_TYPE.Not:
                return null;
            default:
                return Resources.LoadAll<Sprite>("Nogyo/produce/produce_" + type.ToString())[(int)state];
        }
        

        switch (state){
            case 0:
                return Resources.LoadAll<Sprite>("Nogyo/produce/produce_" + type.ToString())[(int)state];
            default:
                return Resources.LoadAll<Sprite>("Nogyo/produce/produce_" + type.ToString())[(int)state - 1];
                
        }
        
    }

    /*
     * バルコニーのマップサイズを返却
     */
     static public int[] getGardenMass(BALCONY btype)
    {
        switch (btype)
        {
            case BALCONY.Balcony1:
                return new int[2] { 5, 2 };

        }
        return new int[2] { 3, 1 };
    }
}
