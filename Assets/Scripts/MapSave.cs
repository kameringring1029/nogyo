using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Information;
using UnityEngine.UI;

public class MapSave : MonoBehaviour {

    public GameObject mapsavelogtext;

    private mapinfo mapinformation;

	// Use this for initialization
	void Start () {
		
	}

    public void init(mapinfo map)
    {
        this.mapinformation = map;
        gameObject.GetComponent<InputField>().text = mapinformation.name;

        // ユニットの初期配置ブロックがすべて配置されているか
        if (map.ally.Length != 3)
        {
            mapsavelogtext.GetComponent<Text>().text = "!!! 味方ユニットは必ず３つ配置してください。。。） !!!";
            StartCoroutine(endSave());
        } else if (map.enemy.Length < 1)
        {
            mapsavelogtext.GetComponent<Text>().text = "!!! 敵ユニットが配置されていません。。。 !!!";
            StartCoroutine(endSave());
        }
        else
        {
            mapsavelogtext.GetComponent<Text>().text = "マップを保存します。\nマップ名を入力してください。(※そのままだと上書き)";
        }

    }


    public void save()
    {
        mapinformation.name = gameObject.GetComponent<InputField>().text;
        StartCoroutine(postMapJson());
    }

    IEnumerator postMapJson()
    {
        var request = new UnityWebRequest("http://koke.link:3000/llsrpg/map/savejson", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(JsonUtility.ToJson(mapinformation));
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.Send();

        if(request.responseCode == 200)
        {
            mapsavelogtext.GetComponent<Text>().text = mapinformation.name + "がサーバに保存されました";
        }
        else
        {
            mapsavelogtext.GetComponent<Text>().text = "サーバに接続できませんでした\ncode:" + request.responseCode;
        }
        Debug.Log("Status Code: " + request.responseCode);

        StartCoroutine(endSave());
        
    }


    IEnumerator endSave()
    {
        yield return new WaitForSeconds(3);
        Destroy(GameObject.Find("MapSavePanel(Clone)"));
    }





}
