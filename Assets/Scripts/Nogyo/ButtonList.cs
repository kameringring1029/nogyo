using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Information;

public class ButtonList : MonoBehaviour
{
    
    /* delegate用のやつ */
    public delegate void buttonexecWrapper(int x);
    public delegate void buttonStrExecWrapper(string x);

       
    // アイテム用
    //
    static public void setItemButtonList(ItemBox itemlist , buttonStrExecWrapper func, RectTransform content)
    {
        GameObject btnPref = Resources.Load<GameObject>("Prefab/Nogyo/ItemMenuButtonPrefab");

        //Content取得(ボタンを並べる場所)
        //RectTransform content = GameObject.Find("Content").GetComponent<RectTransform>();

        //Contentの高さ決定
        //(ボタンの高さ+ボタン同士の間隔)*ボタン数
        float btnSpace = content.GetComponent<GridLayoutGroup>().flexibleWidth;
        float btnHeight = btnPref.GetComponent<LayoutElement>().preferredHeight;
        content.sizeDelta = new Vector2(0, (btnHeight + btnSpace) * itemlist.items.Length);

        foreach(NogyoItem item in itemlist.items)
        {
            //ボタン生成
            GameObject btn = (GameObject)Instantiate(btnPref);
            btn.name = item.id + "." + btn.name;

            //ボタンをContentの子に設定
            btn.transform.SetParent(content, false);

            //ボタンのテキスト変更
            btn.transform.GetComponentInChildren<TextMeshProUGUI>().text =  item.qty.ToString();
            //btn.GetComponent<Image>().color = new Color(192 / 255f, 192 / 255f, 228 / 255f, 192 / 255f);
            btn.transform.GetChild(0).GetComponent<Image>().sprite
                  = Resources.Load<Sprite>("Nogyo/item/" + item.id);

            //ボタンのクリックイベント登録
            btn.transform.GetComponent<Button>().onClick.AddListener(() => func(item.id));

        }

    }


    // シナリオ用
    //
    static public void setScenarioButtonList(NogyoEventscenarioArray[] itemlist, buttonexecWrapper func, RectTransform content)
    {
        destroyAllButtons(); // 現在のボタンを削除

        GameObject btnPref = Resources.Load<GameObject>("Prefab/ScrollViewButtonPrefab");
        
        //Contentの高さ決定
        //(ボタンの高さ+ボタン同士の間隔)*ボタン数
        float btnSpace = content.GetComponent<VerticalLayoutGroup>().spacing;
        float btnHeight = btnPref.GetComponent<LayoutElement>().preferredHeight;
        content.sizeDelta = new Vector2(0, (btnHeight + btnSpace) * itemlist.Length);

        for(int i=0; i<itemlist.Length; i++)
        {
            //ボタン生成
            GameObject btn = (GameObject)Instantiate(btnPref);
            btn.name = i + "_" + btn.name;

            //ボタンをContentの子に設定
            btn.transform.SetParent(content, false);

            //ボタンのテキスト変更
            btn.transform.GetComponentInChildren<Text>().text = itemlist[i].scenario[itemlist[i].scenario.Length-1].message;
            btn.GetComponent<Image>().color = new Color(192 / 255f, 192 / 255f, 228 / 255f, 192 / 255f);

            //ボタンのクリックイベント登録
            int tmpno = i;
            btn.transform.GetComponent<Button>().onClick.AddListener(() => func(tmpno));

        }

    }


    // NogyoEvent用
    //
    static public void setNogyoEventButtonList(NogyoEvent[] itemlist, buttonexecWrapper func, RectTransform content)
    {
        destroyAllButtons(); // 現在のボタンを削除

        GameObject btnPref = Resources.Load<GameObject>("Prefab/ScrollViewButtonPrefab");

        //Contentの高さ決定
        //(ボタンの高さ+ボタン同士の間隔)*ボタン数
        float btnSpace = content.GetComponent<VerticalLayoutGroup>().spacing;
        float btnHeight = btnPref.GetComponent<LayoutElement>().preferredHeight;
        content.sizeDelta = new Vector2(0, (btnHeight + btnSpace) * itemlist.Length);

        for (int i = 0; i < itemlist.Length; i++)
        {
            //ボタン生成
            GameObject btn = (GameObject)Instantiate(btnPref);
            btn.name = i + "_" + btn.name;

            //ボタンをContentの子に設定
            btn.transform.SetParent(content, false);

            //ボタンのテキスト変更
            btn.transform.GetComponentInChildren<Text>().text = itemlist[i].name;
            btn.GetComponent<Image>().color = new Color(192 / 255f, 192 / 255f, 228 / 255f, 192 / 255f);

            //ボタンのクリックイベント登録
            int tmpno = i;
            btn.transform.GetComponent<Button>().onClick.AddListener(() => func(tmpno));

        } 

    }


    //
    static public void destroyAllButtons() //http://yusuke-hata.hatenablog.com/entry/2013/05/22/232820
    {
        var target = GameObject.FindGameObjectsWithTag("ScrollViewButton");
        foreach(GameObject b in target)
        {
            Destroy(b);
        }
    }
}
