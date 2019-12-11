using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class CommCtrl : MonoBehaviour
{

    bool inComm;
    ItemMenu.STATUS status;
    int messageid;

    // Start is called before the first frame update
    void Start()
    {
        inComm = true;
    }

    public void init(ItemMenu.STATUS status)
    {
        this.status = status;
        StartCoroutine(initproc());

    }
    IEnumerator initproc()
    {
        gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "<color=red>ルル</color>:\n" + getRandomMessage();
        yield return new WaitForSeconds(1f);

        gameObject.transform.GetChild(0).GetComponent<Animator>().SetBool("inComm", true);

        while (inComm)
        {
            gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "<color=red>ルル</color>:\n"+getRandomMessage();

            yield return new WaitForSeconds(4f);
        }
    }

    public void destroy()
    {
        StartCoroutine(destroyproc());
    }
    IEnumerator destroyproc()
    {
        gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "<color=red>ルル</color>:\n" + "まかせて!";

        gameObject.transform.GetChild(0).GetComponent<Animator>().SetBool("inComm", false);
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }

    string getRandomMessage()
    {
        string message = "まかせて!";

        // random
        int rand;
        do
        {
            rand = Random.Range(1, 28);
        } while (rand == messageid);
        messageid = rand;



        // メッセージ決定
        switch (messageid)
        {
            case 1: return "おなかすいたーー!";
            case 2: return "あっ、ねこ";
            case 3: return "スズちゃんのオデンたべたいーーー";
            case 4: return "たまにはアルちゃんに運動させなきゃ";
            case 5: return "ベス子のエンジンまた調子悪いんだー";
            case 6: return "本日はおヒガラもよくー";
            case 7: return "はーしーれはしーれー　ルル子ーのベスー子ー";
            case 8: return "さっき見かけたイノシシ、持って帰ったら鍋にしてくれたり?";
            case 9: return "アキバの闇市、また怪しくなってたよ";
            case 10: return "野良ねこ野良いぬはダメでも、野良パンダならいい? だめ?";
            case 11: return "今度ヨコハマの方にも行ってみようかなー 人、いるかなー";
            case 12: return "イケブクロの八百屋さん、アメくれるから好き";
            case 13: return "キョウジュが 早めに計測データ出せー だってさー";
            case 14: return "今日の配達分はまだまだー";
            default:
                // デフォメッセージ
                switch (status)
                {
                    case ItemMenu.STATUS.inSelectShip:
                        return "どれを出荷する?";
                    case ItemMenu.STATUS.inShop:
                        return "何を買ってくる?";
                    default:
                        return "";

                }

        }

        return message;
    }



}
