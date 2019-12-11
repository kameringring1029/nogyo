
/*
 * Map情報の定義
 */

using UnityEngine;

namespace Information
{

    [System.Serializable]
    public class mapinfo
    {
        public string name;
        public string frame;
        public int[] mapstruct;
        public string[] ally;
        public string[] enemy;
        public Mapscenarioarray[] mapscenarioarrays;
    }

    [System.Serializable]
    public class Mapscenarioarray
    {
        public Mapscenario[] mapscenario;
    }

    [System.Serializable]
    public class Mapscenario
    {
        public int camp;
        public int unitno;
        public int action;
        public string message;
    }

    [System.Serializable]
    public class coodinate
    {
        public int x;
        public int y;

        public coodinate(){}
        public coodinate(int x, int y) { this.x = x; this.y = y; }
    }


    public class MapInfoUtil
    {

        public static GameObject getBlockTypebyid(int typeno)
        {

            string block_id;

            switch (typeno)
            {
                case 1:
                    block_id = "normal"; break;
                case 2:
                    block_id = "unwalkable"; break;
                case 3:
                    block_id = "high"; break;
                case 4:
                    block_id = "sea"; break;

                case 11:
                    block_id = "otonoki-entrance"; break;
                case 12:
                    block_id = "otonoki-saku1"; break;
                case 13:
                    block_id = "otonoki-saku2"; break;

                case 21:
                    block_id = "uranohoshi_desk"; break;
                case 22:
                    block_id = "uranohoshiclub_fg"; break;

                case 102:
                    block_id = "unwalkable"; break;
                case 103:
                    block_id = "high"; break;

                case 111:
                    block_id = "cleaningtool"; break;
                case 112:
                    block_id = "otoshidama"; break;


                case 0:
                    block_id = "unitposition"; break;
                case -12:
                    block_id = "unitposition" + typeno; break;
                case -1001:
                    block_id = "unitposition_enemy_1_smile"; break;
                case -1002:
                    block_id = "unitposition_enemy_1_cool"; break;
                case -1012:
                    block_id = "unitposition_enemy_1_cool"; break;

                default:
                    block_id = "normal"; break;
            }

            return Resources.Load<GameObject>("Prefab/Block/" + block_id);
            
        }

        // MapEdit用、登録できるBlockIdの一覧から次のIDを返却
        public static int getNextBlockTypebyid(int preblockno)
        {
            int[] list = {1,2,3,4, -1001, -1002, -12, 0};

            int nextblockno = 0;
            for(int i=0; i<list.Length; i++) // リスト中を検索
            {
                if(preblockno == list[i]) // 一致するものが見つかったら
                {
                    if(i == list.Length - 1) // リストの最後であれば
                    {
                        nextblockno = 0;
                    }
                    else
                    {
                        nextblockno = i + 1;
                    }
                } 
            }

            return list[nextblockno];
        }

    }


    /* 廃止、JSONへ移行

    public class MapStructInfo
    {
        virtual public string mapStruct() { return null; }

    }

public class MapPlain: MapStructInfo
{
    override public string mapStruct()
    {
        return "{" +
            "\"name\":\"MAP1\"," +
            "\"frame\":\"map_frame\"," +
            "\"ally\":[\"5-2\",\"6-2\",\"7-2\"]," +
            "\"enemy\":[\"4-8-0\",\"5-8-1\",\"10-9-0\"]," +
            "\"mapstruct\":" +
            "[" +
            "1,1,1,1,1,1,1,1,1,1,1,1," +
            "1,1,1,1,1,1,1,1,1,1,1,1," +
            "1,1,1,1,1,1,1,1,1,1,1,1," +
            "1,1,1,1,1,1,1,1,1,1,1,1," +
            "1,1,1,1,1,1,1,1,1,1,1,1," +
            "1,1,1,1,1,1,1,1,1,1,1,1," +
            "1,1,1,1,1,1,1,1,1,1,1,1," +
            "1,1,1,1,1,1,1,1,1,1,1,1," +
            "1,1,1,1,1,1,1,1,1,1,1,1," +
            "1,1,1,1,1,1,1,1,1,1,1,1," +
            "1,1,1,1,1,1,1,1,1,1,1,1," +
            "1,1,1,1,1,1,1,1,1,1,1,1" +
            "]" +
            "}";
    }
}



public class MapOtonokiProof : MapStructInfo
{
    override public string mapStruct()
    {
        return "{" +
            "\"name\":\"OtonokiProof\"," +
            "\"frame\":\"map_otonokiproof\"," +
            "\"ally\":[\"3-4\",\"4-4\",\"10-10\"]," +
            "\"enemy\":[\"4-8-0\",\"5-8-1\",\"10-9-0\"]," +
            "\"mapstruct\":" +
            "[" +
            "1,1,1,1,1,1,1,1,2,2,2,3,3,3," +
            "1,1,1,1,1,1,1,1,2,2,2,3,3,3," +
            "1,1,1,1,1,1,1,1,2,2,2,3,3,3," +
            "1,1,1,1,1,1,1,1,2,2,2,2,3,3," +
            "1,1,1,1,1,1,1,1,2,2,2,2,3,3," +
            "1,112,1,1,1,1,1,1,11,2,2,2,3,3," +
            "1,1,1,1,1,1,1,1,1,1,2,2,3,3," +
            "1,1,1,1,1,1,1,1,1,1,2,3,3,3," +
            "1,1,1,1,1,1,1,1,1,1,2,12,3,3," +
            "1,1,1,111,1,1,1,1,1,1,1,1,2,3," +
            "1,1,1,1,1,1,1,1,1,1,1,1,2,3," +
            "1,1,1,1,1,1,1,1,1,1,1,1,2,3," +
            "1,1,1,1,1,1,1,1,1,1,2,2,3,3," +
            "1,1,1,1,1,1,1,1,1,1,13,2,3,3" +
            "]" +
            "}";
    }
}




public class MapPlainStory : MapStructInfo
{
    override public string mapStruct()
    {
        return "{" +
            "\"name\":\"MAP1\"," +
            "\"frame\":\"map_frame\"," +
            "\"ally\":[\"5-2\",\"6-2\",\"7-2\"]," +
            "\"enemy\":[\"4-8-0\",\"5-8-1\",\"10-9-0\"]," +
            "\"mapstruct\":" +
            "[" +
            "1,1,1,1,1,1,1,1,1,1,1,1," +
            "1,1,1,1,1,1,1,1,1,1,1,1," +
            "1,1,1,1,1,1,1,1,1,1,1,1," +
            "1,1,1,1,1,1,1,1,1,1,1,1," +
            "1,1,1,1,1,1,1,1,1,1,1,1," +
            "1,1,1,1,1,1,1,1,1,1,1,1," +
            "1,1,1,1,1,1,1,1,1,1,1,1," +
            "1,1,1,1,1,1,1,1,1,1,1,1," +
            "1,1,1,1,1,1,1,1,1,1,1,1," +
            "1,1,1,1,1,1,1,1,1,1,1,1," +
            "1,1,1,1,1,1,1,1,1,1,1,1," +
            "1,1,1,1,1,1,1,1,1,1,1,1" +
            "]," +
            "\"mapscenarioarrays\":" +
            "[{" +
              "\"mapscenario\":" +
              "[{" +
                "\"camp\":1," +
                "\"unitno\":1," +
                "\"action\":1," +
                "\"message\":\"てｓつ\"" +
              "}]" +
            "}," +
            "{" +
              "\"mapscenario\":" +
              "[{" +
                "\"camp\":1," +
                "\"unitno\":1," +
                "\"action\":1," +
                "\"message\":\"てｓつafseasfeasfseasefase\"" +
              "}]" +
            "}]" +
            "}";
    }
}



    public class MapUranohoshiClub : MapStructInfo
    {
        override public string mapStruct()
        {
            return "{" +
                "\"name\":\"UranohoshiClub\"," +
                "\"frame\":\"map_uranohoshiclub_bg\"," +
                "\"ally\":[\"1-1\",\"4-4\",\"10-10\"]," +
                "\"enemy\":[\"4-8-0\",\"5-8-1\",\"10-9-0\"]," +
                "\"mapstruct\":" +
                "[" +
                "2,2,2,2,2,2," +
                "1,1,2,2,2,2," +
                "1,2,22,2,2,2," +
                "1,21,2,2,2,2," +
                "1,2,1,2,2,2," +
                "1,1,1,2,2,2" +
                "]" +
                "}";
        }
    }
*/


}