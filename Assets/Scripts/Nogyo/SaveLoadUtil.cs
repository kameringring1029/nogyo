using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadUtil : MonoBehaviour
{
    public string SaveData { get; set; }
    private string SaveKey = "hoge";

    void Start()
    {
        // ロード
        SaveData = PlayerPrefs.GetString(SaveKey, "未定義の場合の初期値");
        Debug.Log(SaveData);
    }

    public void Save(string Data)
    {
        // セーブ
        SaveData = Data;
        PlayerPrefs.SetString(SaveKey, SaveData);
        PlayerPrefs.Save();
    }
}
