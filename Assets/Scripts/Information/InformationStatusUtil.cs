using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using General;


/*
 * ユニットステータス参照用のユーティリティ
 */

namespace Information
{

    public class UnitStatusUtil
    {
        //-- ユニットIDからステータスを呼び出す --
        public static statusTable search(int unitid)
        {

            switch (unitid)
            {
                case 2:
                    return new Eli_DS();
                case 4:
                    return new Umi_DG();
                case 5:
                    return new Rin_HN();
                case 8:
                    return new Hanayo_LB();

                case 11:
                    return new Chika_GF();
                case 12:
                    return new Riko_GF();
                case 13:
                    return new Kanan_TT();
                case 15:
                    return new You_GF();
                case 16:
                    return new Yohane_JA();
                //       case 13:
                //           return new Kanan_TT();


                case 1001:
                    return new Enemy1_Smile();
                case 1002:
                    return new Enemy1_Cool();

                case 1012:
                    return new Enemy2_Cool();

            }

            return null;
        }


        //-- ユニットIDをランダムで取得　＊arraynumの数だけ --
        // return: ユニットIDの配列
        // arraynum: 取得するユニットID数のサイズの配列
        public static int[] randunit(int arraynum)
        {
            List<int> randlist = new List<int>();

            for(int i=0; i<arraynum; i++)
            {
                int rand = 0;
                do
                {
                    rand = Random.Range(1, 18); // ユニットIDの範囲
                }
                while ((search(rand) is null) || randlist.Contains(rand));  // 登録のあるIDかつリスト中に同じIDが無いものが出るまで

                randlist.Add(rand);
            }

            return randlist.ToArray();
        }



        //-- ユニットIDからステータスの説明文を生成 --//
        public static string outputUnitInfo(int unitid)
        {
            statusTable unit = search(unitid);

            string hp = string.Format("<color=yellow>HP</color> {0,3}", unit.hp());
            string movable = string.Format("<color=yellow>MOV</color>{0,3}", unit.movable());
            string reach = string.Format("<color=yellow>RNG</color>{0,3}", unit.reach());
            string attack_phy = string.Format("<color=yellow>ATK</color>{0,3}", unit.attack_phy());
            string guard_phy = string.Format("<color=yellow>DEF</color>{0,3}", unit.guard_phy());
            string attack_magic = string.Format("<color=yellow>MAT</color>{0,3}", unit.attack_magic());
            string guard_magic = string.Format("<color=yellow>MDF</color>{0,3}", unit.guard_magic());
            string agility = string.Format("<color=yellow>AGL</color>{0,3}", unit.agility());
            string luck = string.Format("<color=yellow>LCK</color>{0,3}", unit.luck());

            string outinfo =
                "<b>" + unit.name() + "</b>" + " <size=20>(" + unit.subname() + ")" + "\n" + 
                " 【" + unit.job() + "  Lv: " + unit.level() + "】\n\n" +
                " 「" + unit.description() + "」</size>\n\n " +
                hp + "   " + movable + "   " + reach + "\n " +
               attack_phy + "   " + guard_phy + "   " + agility + "\n " +
               attack_magic + "   " + guard_magic + "   " + luck + "\n\n" +
            " <sprite=\"Group\" index="+unit.group_number()+"> " + " <sprite=\"UnitInGroup\" index="+ unit.unitingroup_number()+">";

            return outinfo;
        }


        //-- jobIDからjobプレハブを呼び出す --
        public static GameObject searchJobPrefab(string jobid)
        {
            return Resources.Load<GameObject>("Prefab/Units/"+jobid);
        }

    }
}
