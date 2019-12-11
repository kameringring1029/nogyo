using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingPanel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // ロードアニメーションの変更
        gameObject.GetComponent<Transform>().GetChild(1).gameObject.GetComponent<Animator>().SetInteger("id",Random.Range(1,3));

        // テスト用
        // Destroy(gameObject, 2.7f);
        //Destroy(gameObject, 0.7f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
