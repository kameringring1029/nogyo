using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {
        // プレイヤーデータ
        PlayerData.getinstance().newGame();
    }


    public void onClickStart()
    {
        SceneManager.LoadScene("NogyoLiving");
    }
}
