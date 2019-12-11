using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using General;


/*
 * SRPGゲーム中のInformationパネルにアタッチされる
 * UnitとかBlockの情報表示なり戦闘情報表示なり
 */


public class DisplayInfo : MonoBehaviour {

    private GameObject infotext;
    private GameObject hpbar;



    //--- blockの情報を表示する ---//
    // block: 情報を表示するブロック
    public void displayBlockInfo(FieldBlock block)
    {
        infotext = gameObject.transform.GetChild(0).gameObject;
        hpbar = gameObject.transform.GetChild(1).gameObject;

        GameObject groundedUnit = block.GroundedUnit;

        // Unitが配置されていたら
        if (groundedUnit != null)
        {
            // InfoのHPバーを表示
            hpbar.SetActive(true);
            hpbar.transform.GetChild(0).GetComponent<Image>().fillAmount = (float)groundedUnit.GetComponent<Unit>().unitInfo.hp[1] / (float)groundedUnit.GetComponent<Unit>().unitInfo.hp[0];
            switch (groundedUnit.GetComponent<Unit>().camp)
            {
                case CAMP.ALLY:
                    hpbar.transform.GetChild(0).GetComponent<Image>().color = new Color(0.25f, 0.25f, 1f);
                    break;
                case CAMP.ENEMY:
                    hpbar.transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 0.25f, 0.25f);
                    break;
            }

            // InfoのPanelを陣営色に
            if (groundedUnit.GetComponent<Unit>().camp == CAMP.ALLY)
            {
                gameObject.GetComponent<Image>().color = new Color(220.0f / 255.0f, 220.0f / 255.0f, 255.0f / 255.0f, 220.0f / 255.0f);
            }
            else if (groundedUnit.GetComponent<Unit>().camp == CAMP.ENEMY)
            {
                gameObject.GetComponent<Image>().color = new Color(255.0f / 255.0f, 220.0f / 255.0f, 220.0f / 255.0f, 220.0f / 255.0f);

            }

            // InfoText更新
            infotext.GetComponent<TextMeshProUGUI>().text = groundedUnit.GetComponent<Unit>().unitInfo.outputInfo();
            Debug.Log(groundedUnit.GetComponent<Unit>().unitInfo.outputInfo());

        }
        // なにもアイテムがなかったら
        else
        {
            // InfomationにBlock情報を表示
            gameObject.GetComponent<Image>().color = new Color(255.0f / 255.0f, 255.0f / 255.0f, 255.0f / 255.0f, 220.0f / 255.0f);
            infotext.GetComponent<TextMeshProUGUI>().text = block.outputInfo();

            hpbar.SetActive(false);
        }

    }


    //--- 戦闘情報を表示する ---//
    public void displayBattleInfo(GameObject sourceunit, GameObject targetunit, ACTION selectedAction)
    {
        hpbar.SetActive(false);
        
        string text = "";

        if(selectedAction == ACTION.ATTACK)
        {
            int damage = sourceunit.GetComponent<Unit>().getAttackDamage(targetunit);
            int damagedhp = targetunit.GetComponent<Unit>().unitInfo.hp[1] - damage;
            if (damagedhp < 0) damagedhp = 0;
            text = "<b>ダメージ予測</b>\n\n" +
                          "<size=11>" +
                          "<color=yellow>HP</color> " + targetunit.GetComponent<Unit>().unitInfo.hp[1] + "->" + damagedhp + 
                          " / "+ targetunit.GetComponent<Unit>().unitInfo.hp[0] + "\n" +
                          "<color=yellow>ダメージ</color> " + damage + "\n" +
                          "<color=yellow>命中率</color> " + sourceunit.GetComponent<Unit>().getAttackHit(targetunit) + "%" + "\n" +
                          "<color=yellow>クリティカル</color> " + sourceunit.GetComponent<Unit>().getAttackCritical(targetunit) + "%" +
                          "</size>";   

        }
        else if(selectedAction == ACTION.HEAL)
        {
            int heal = sourceunit.GetComponent<Unit>().getHealVal(targetunit);
            int healedhp = targetunit.GetComponent<Unit>().unitInfo.hp[1] + heal;
            if (healedhp > targetunit.GetComponent<Unit>().unitInfo.hp[0]) healedhp = targetunit.GetComponent<Unit>().unitInfo.hp[0];
            text = "<b>回復予測</b>\n\n" +
                          "<size=11>" +
                          "<color=yellow>HP</color> " + targetunit.GetComponent<Unit>().unitInfo.hp[1] + "->" + heal + "\n" +
                          "<color=yellow>回復量</color> " + heal +
                          "</size>";

        }
        else if (selectedAction == ACTION.REACTION)
        {
            text = "＜うたう＞\n\n対象のユニットを再行動可能にします";

        }

        infotext.GetComponent<TextMeshProUGUI>().text = text;
    }

}
