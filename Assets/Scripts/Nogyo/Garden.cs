using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using General;
using Information;

public class Garden : MonoBehaviour
{

    // マップ情報
    public int x_mass, y_mass;
    public Dictionary<coodinate, GameObject> FieldBlocks;
    public coodinate[] plantpos;
    private int[,] mapstruct;
    public GameObject cursor;
    public GameObject mapframe;

    public Garden()
    {
        cursor = GameObject.Find("spritecursor");
    }


    public void positioningPlants(int x_mass, int y_mass, coodinate[] plantpos)
    {
        this.x_mass = x_mass;
        this.y_mass = y_mass;

        this.plantpos = plantpos;

       // mapframe.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Map/mapframe/" + mapinformation.frame);
       

        // map作成
        FieldBlocks = new Dictionary<coodinate, GameObject>();

        for (int x = 0; x < x_mass; x++) // マップのx軸方向
        {
            for (int y = 0; y < y_mass; y++) // マップのy軸
            {
                for(int i=0; i<plantpos.Length; i++)
                {
                    if(plantpos[i].x == x && plantpos[i].y == y) // plantposとしてブロックの配置が指定されていたら
                        setPlant(GameObject.Find("Balcony_kadan_Position").transform.position, plantpos[i], i);
                }
            }
        }

    }

    /*
     * baseP : x=0,y=0の基準位置
     * x,y : 設置する位置
     */
    public void setPlant(Vector3 baseP, coodinate pos, int id)
    {
        // 配置位置の指定
        Vector3 position = baseP + new Vector3((pos.x - pos.y) / 2.0f,  -pos.y / 4.0f - pos.x / 4.0f, 0);
        coodinate cd = pos;//////////////////////////////////////////////////////////////////////////

        // ブロックの生成
        FieldBlocks.Add(cd, Instantiate(Resources.Load<GameObject>("Prefab/Nogyo/gardenBlock"), position, Quaternion.AngleAxis(0, new Vector3())));
        GameObject child = FieldBlocks[cd].transform.GetChild(0).gameObject;
        FieldBlocks[cd].name = pos.x + "_" + pos.y + "_block";

        // ブロックのidをScript上に記録
        FieldBlocks[cd].GetComponent<GardenBlock>().id = id;


        // Spriteの変更
        //FieldBlocks[cd].GetComponent<SpriteRenderer>().sprite = GardenInfoUtil.getGardenBlockType(x_mass, y_mass, x, y);
        FieldBlocks[cd].GetComponent<SpriteRenderer>().sprite = GardenInfoUtil.getGardenBlockType(plantpos, pos.x, pos.y);
        //child.GetComponent<SpriteRenderer>().sprite = GardenInfoUtil.getGardenProduceSpriteByType(Random.Range(0, 2)); // int引数のrandom.rangeはmaxを含まない

        // map上の表示順の設定
        int distance = (abs(pos.x) + abs(pos.y));
        distance += 100;
        FieldBlocks[cd].GetComponent<SpriteRenderer>().sortingOrder = distance;
        child.GetComponent<SpriteRenderer>().sortingOrder = distance + 1;
    }


    /*
     * 作物のSprite変更
     */
    public void renewProduce(coodinate pos, Produce.PRODUCE_TYPE type, Produce.PRODUCE_STATE state)
    {
        if(state != Produce.PRODUCE_STATE.Vanish && type != Produce.PRODUCE_TYPE.Not)
        {
            FieldBlocks[pos].transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite
                = GardenInfoUtil.getGardenProduceSpriteByType(type, state);
        }
        else
        {
            FieldBlocks[pos].transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite
            =null;
        }

    }

    /*
     * 散水したBlockのSprite Colorを調整する
     * water: true=散水、false:乾かす
     */
     public void wateringProduce(coodinate pos, bool water)
    {
        switch (water)
        {
            case true:
                FieldBlocks[pos].GetComponent<SpriteRenderer>().color = Color.HSVToRGB(1, 0, 0.9f);
                break;
            case false:
                FieldBlocks[pos].GetComponent<SpriteRenderer>().color = Color.HSVToRGB(1, 0, 1);
                break;
        }


    }


    /* カーソル位置更新 */
    public void renewCursor(coodinate pos)
    {
        cursor.transform.position
            = FieldBlocks[pos].transform.position + new Vector3(0, 0.2f, 0);
    }


    private int abs(int a)
    {
        if (a < 0) a = a * (-1);
        return a;
    }

}
