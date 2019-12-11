using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

static public class LocalStorage 
{


    // ローカルストレージから読み込み
    static public void LoadLocalStageData()
    {
        string myJson = "[{\"NUM\":\"1\",\"TEXT\":\"HELLO\"},{\"NUM\":\"2\",\"TEXT\":\"BONJOUR\"},]";
        string dataName = "data.txt";

        Debug.Log(GetPath());
        string savePath = GetPath();

        // ディレクトリが無い場合はディレクトリを作成して保存
        if (!Directory.Exists(savePath))
        {
            // ディレクトリ作成
            Directory.CreateDirectory(savePath);
            // ローカルに保存
            SaveToLocal(myJson, dataName);
        }
        else
        {
            // ローカルからデータを取得
            string json = LoadFromLocal(dataName);
            Debug.Log(json);
        }
    }

    // 保存
    static public void SaveToLocal(string json, string dataName)
    {
        // jsonを保存
        File.WriteAllText(GetPath() + dataName, json);
    }

    // 取得
    static public string LoadFromLocal(string jsonpath)
    {
        // jsonを読み込み
        string json = File.ReadAllText(jsonpath);

        return json;
    }

    // パス取得
    static public string GetPath()
    {
        return Application.persistentDataPath + "/AppData/";
    }


    // ディレクトリ以下のファイルのパス一覧取得
    static public List<string> GetFileNames(string dir, string extention)
    {
        List<string> filelist = new List<string>();

        string[] path_array = Directory.GetFiles(dir, "*." + extention);
        for (int i = 0; i < path_array.Length; i++)
        {
            filelist.Add(path_array[i]);
            Debug.Log(path_array[i]);
        }

        return filelist;
    }
}