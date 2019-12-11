
/*
 * Nogyo情報の定義
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Information
{

    [System.Serializable]
    public class NogyoEvent
    {
        public string name;
        public string place;
        public string[] unit;
        public NogyoEventscenarioArray[] scenarioarrays;
    }


    [System.Serializable]
    public class NogyoEventscenarioArray
    {
        public NogyoEventScenario[] scenario;
    }

    [System.Serializable]
    public class NogyoEventScenario
    {
        public int unitno;
        public int action;
        public string message;
    }


    public class NogyoInfoUtil
    {

        /* JSONのString[x-y-id]からユニットのIDを返却 */
        public static int getUnitId_FromStr(string str)
        {
            return int.Parse( str.Split('-')[2]);
        }
        public static int[] getUnitIdArr_FromStr(string[] str)
        {
            int[] units = new int[str.Length];
            for(int i=0; i<str.Length; i++)
            {
                units[i] = getUnitId_FromStr(str[i]);
            }
            return units;
        }


        // シナリオ情報からアクションするユニットのゲームオブジェクトを取得
        public static GameObject getActionUnit(NogyoEventScenario scenario)
        {
            /*
            switch (mapscenario.camp)
            {
                case 1:
                    return gameObject.GetComponent<Map>().allyUnitList[mapscenario.unitno];
                case -1:
                    return gameObject.GetComponent<Map>().enemyUnitList[mapscenario.unitno];
                default:
                    return null;
            }
            */

            return GameObject.Find("player" + scenario.unitno);
        }

        // メッセージウィンドウとかに話者の名前を表示するための文字列を返すやつ
        public static string getTalkerNameText(NogyoEventScenario scenario)
        {
            string name = "?";

            // メッセージに話者の名前を追加
            if (scenario.message != "")
            {
                GameObject actionunit = getActionUnit(scenario);

                name = NogyoProfileUtil.getProfile(scenario.unitno).name();
                if (scenario.action > 60 && scenario.action < 70)
                {
                    name = name + "?";
                }

                Color color = NogyoProfileUtil.getProfile(scenario.unitno).color();
                name = "<color=#" + ColorUtility.ToHtmlStringRGB(color) + ">" + name + "</color>: ";
            }

            return name;
        }

        // カラのシナリオを作成
        public static NogyoEventscenarioArray getNewNESA()
        {
            NogyoEventscenarioArray array = new NogyoEventscenarioArray();
            array.scenario = new NogyoEventScenario[1];
            array.scenario[0] = new NogyoEventScenario();
            array.scenario[0].action = (int)General.STORYACTION.TALK;
            array.scenario[0].unitno = 0;
            array.scenario[0].message = "";
            return array;
        }
    }

}